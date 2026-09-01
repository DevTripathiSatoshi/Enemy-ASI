using UnityEngine;

namespace Doom_Dude.EnemyASI
{
    public abstract class EnemyAttack : MonoBehaviour
    {
        [Header("Base Attack Settings")]
        [SerializeField] protected float attackDamage = 20f;
        [SerializeField] protected float attackRange = 2f;
        [SerializeField] protected float attackCooldown = 1.5f;

        protected float lastAttackTime;

        public float AttackRange => attackRange;

        public bool CanAttack()
        {
            return Time.time >= lastAttackTime + attackCooldown;
        }

        public void PerformAttack(Transform target)
        {
            if (CanAttack())
            {
                lastAttackTime = Time.time;
                ExecuteAttack(target);
            }
        }

        protected abstract void ExecuteAttack(Transform target);

        // Called by animation events to apply damage or fire a projectile
        public virtual void TriggerAttackEvent()
        {
        }

        // Utility to visually show attack range in Editor
        protected virtual void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(transform.position, attackRange);
        }
    }
}
