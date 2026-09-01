using UnityEngine;

namespace Doom_Dude.EnemyASI
{
    public class NormalEnemyAttack : EnemyAttack
    {
        [Header("Melee Settings")]
        [SerializeField] private Transform meleeHitPoint;
        [SerializeField] private float meleeHitRadius = 0.5f;
        [SerializeField] private LayerMask targetMask;

        protected override void ExecuteAttack(Transform target)
        {
            // The actual attack animation is triggered by EnemyAI.
            // Damage is now dealt via Animation Event calling TriggerMeleeDamage().
        }

        public override void TriggerAttackEvent()
        {
            Vector3 hitCenter = meleeHitPoint != null ? meleeHitPoint.position : transform.position + transform.forward;
            
            Collider[] hitTargets = Physics.OverlapSphere(hitCenter, meleeHitRadius, targetMask);
            
            foreach (var hit in hitTargets)
            {
                IDamageable damageable = hit.GetComponent<IDamageable>();
                if (damageable != null)
                {
                    damageable.TakeDamage(attackDamage);
                    Debug.Log("Normal Enemy hit the target via Animation Event!");
                }
            }
        }

        protected override void OnDrawGizmosSelected()
        {
            base.OnDrawGizmosSelected();
            Gizmos.color = Color.red;
            Vector3 hitCenter = meleeHitPoint != null ? meleeHitPoint.position : transform.position + transform.forward;
            Gizmos.DrawWireSphere(hitCenter, meleeHitRadius);
        }
    }
}
