using UnityEngine;

namespace Doom_Dude.EnemyASI
{
    public enum AttackType
    {
        Melee,
        RangedRaycast,
        RangedProjectile
    }

    public class UniversalEnemyAttack : EnemyAttack
    {
        [Header("Universal Attack Settings")]
        public AttackType attackType = AttackType.Melee;
        [SerializeField] private LayerMask targetMask;

        [Header("Melee Settings")]
        [SerializeField] private Transform meleeHitPoint;
        [SerializeField] private float meleeHitRadius = 0.5f;

        [Header("Ranged Settings")]
        [SerializeField] private Transform gunBarrelPoint;
        [SerializeField] private GameObject projectilePrefab;
        [SerializeField] private float projectileSpeed = 20f;
        [SerializeField] private GameObject muzzleFlashPrefab;

        private Transform currentTarget;

        protected override void ExecuteAttack(Transform target)
        {
            // Store target for when the animation event fires
            currentTarget = target;
            
            // The actual attack animation is triggered by EnemyAI.
            // We wait for the animation event to call TriggerAttackEvent().
        }

        public override void TriggerAttackEvent()
        {
            if (currentTarget == null) return;

            switch (attackType)
            {
                case AttackType.Melee:
                    PerformMeleeAttack();
                    break;
                case AttackType.RangedRaycast:
                    PerformRaycastAttack();
                    break;
                case AttackType.RangedProjectile:
                    PerformProjectileAttack();
                    break;
            }
        }

        private void PerformMeleeAttack()
        {
            Vector3 hitCenter = meleeHitPoint != null ? meleeHitPoint.position : transform.position + transform.forward;
            Collider[] hitTargets = Physics.OverlapSphere(hitCenter, meleeHitRadius, targetMask);
            
            foreach (var hit in hitTargets)
            {
                IDamageable damageable = hit.GetComponent<IDamageable>();
                if (damageable != null)
                {
                    damageable.TakeDamage(attackDamage);
                    Debug.Log("Universal Enemy hit target with Melee!");
                }
            }
        }

        private void PerformRaycastAttack()
        {
            Vector3 spawnPos = gunBarrelPoint != null ? gunBarrelPoint.position : transform.position + transform.forward + Vector3.up;
            Vector3 aimDirection = (currentTarget.position - spawnPos).normalized;

            if (muzzleFlashPrefab != null)
            {
                Instantiate(muzzleFlashPrefab, spawnPos, Quaternion.LookRotation(aimDirection));
            }

            if (Physics.Raycast(spawnPos, aimDirection, out RaycastHit hit, attackRange, targetMask))
            {
                IDamageable damageable = hit.collider.GetComponent<IDamageable>();
                if (damageable != null)
                {
                    damageable.TakeDamage(attackDamage);
                    Debug.Log("Universal Enemy hit target with Raycast!");
                }
            }
            Debug.DrawRay(spawnPos, aimDirection * attackRange, Color.red, 2f);
        }

        private void PerformProjectileAttack()
        {
            Vector3 spawnPos = gunBarrelPoint != null ? gunBarrelPoint.position : transform.position + transform.forward + Vector3.up;
            Vector3 aimDirection = (currentTarget.position - spawnPos).normalized;

            if (muzzleFlashPrefab != null)
            {
                Instantiate(muzzleFlashPrefab, spawnPos, Quaternion.LookRotation(aimDirection));
            }

            if (projectilePrefab != null)
            {
                GameObject projectile = Instantiate(projectilePrefab, spawnPos, Quaternion.LookRotation(aimDirection));
                Rigidbody rb = projectile.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.linearVelocity = aimDirection * projectileSpeed;
                }
            }
        }

        protected override void OnDrawGizmosSelected()
        {
            base.OnDrawGizmosSelected();

            if (attackType == AttackType.Melee)
            {
                Gizmos.color = Color.red;
                Vector3 hitCenter = meleeHitPoint != null ? meleeHitPoint.position : transform.position + transform.forward;
                Gizmos.DrawWireSphere(hitCenter, meleeHitRadius);
            }
        }
    }
}
