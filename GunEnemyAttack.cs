using UnityEngine;

namespace Doom_Dude.EnemyASI
{
    public class GunEnemyAttack : EnemyAttack
    {
        [Header("Gun Settings")]
        [SerializeField] private Transform gunBarrelPoint;
        [SerializeField] private GameObject projectilePrefab;
        [SerializeField] private float projectileSpeed = 20f;
        [SerializeField] private bool useRaycastShooting = false;
        [SerializeField] private LayerMask raycastHitMask;

        protected override void ExecuteAttack(Transform target)
        {
            Vector3 spawnPos = gunBarrelPoint != null ? gunBarrelPoint.position : transform.position + transform.forward + Vector3.up;
            Vector3 aimDirection = (target.position - spawnPos).normalized;

            if (useRaycastShooting)
            {
                // Raycast Shooting logic
                if (Physics.Raycast(spawnPos, aimDirection, out RaycastHit hit, attackRange, raycastHitMask))
                {
                    IDamageable damageable = hit.collider.GetComponent<IDamageable>();
                    if (damageable != null)
                    {
                        damageable.TakeDamage(attackDamage);
                    }
                    // e.g. Instantiate impact effect at hit.point
                }
                Debug.DrawRay(spawnPos, aimDirection * attackRange, Color.red, 1f);
            }
            else
            {
                // Projectile Shooting logic
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

            Debug.Log("Gun Enemy fired at the target!");
        }
    }
}
