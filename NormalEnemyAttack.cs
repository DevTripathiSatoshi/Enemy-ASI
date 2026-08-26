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
            // Here you would typically trigger an animation event, 
            // but for a simple implementation we do the overlap sphere immediately.
            
            Vector3 hitCenter = meleeHitPoint != null ? meleeHitPoint.position : transform.position + transform.forward;
            
            Collider[] hitTargets = Physics.OverlapSphere(hitCenter, meleeHitRadius, targetMask);
            
            foreach (var hit in hitTargets)
            {
                IDamageable damageable = hit.GetComponent<IDamageable>();
                if (damageable != null)
                {
                    damageable.TakeDamage(attackDamage);
                    Debug.Log("Normal Enemy hit the target!");
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
