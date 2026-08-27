using UnityEngine;
using UnityEngine.AI;

namespace Doom_Dude.EnemyASI
{
    [RequireComponent(typeof(Animator))]
    public class EnemyAnimationHandler : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] private Animator animator;
        [SerializeField] private EnemyAI enemyAI;
        [SerializeField] private EnemyHealth enemyHealth;
        [SerializeField] private EnemyVision enemyVision;
        [SerializeField] private NavMeshAgent agent;

        [Header("Animation Settings")]
        [SerializeField] private bool useAimingState = true;
        [SerializeField] private bool useHitReactions = true;

        // Animator Hashes for Kevin Iglesias pack mapping
        private readonly int hashIsAiming = Animator.StringToHash("IsAiming");
        private readonly int hashSpeed = Animator.StringToHash("Speed"); // Blends Walk/Run
        private readonly int hashHit = Animator.StringToHash("Hit");

        private void Awake()
        {
            if (animator == null) animator = GetComponent<Animator>();
            if (enemyAI == null) enemyAI = GetComponentInParent<EnemyAI>();
            if (enemyHealth == null) enemyHealth = GetComponentInParent<EnemyHealth>();
            if (enemyVision == null) enemyVision = GetComponentInParent<EnemyVision>();
            if (agent == null) agent = GetComponentInParent<NavMeshAgent>();
        }

        private void OnEnable()
        {
            if (enemyHealth != null && useHitReactions)
            {
                enemyHealth.OnTakeDamage.AddListener(PlayHitReaction);
            }
        }

        private void OnDisable()
        {
            if (enemyHealth != null && useHitReactions)
            {
                enemyHealth.OnTakeDamage.RemoveListener(PlayHitReaction);
            }
        }

        private void Update()
        {
            if (enemyHealth != null && enemyHealth.IsDead) return;

            UpdateMovementAnimation();
            UpdateAimingState();
        }

        private void UpdateMovementAnimation()
        {
            if (agent != null && animator != null)
            {
                // Smoothly pass the agent's current speed to the animator
                // 0 = Idle, ~3.5 = Walk, ~6 = Run (based on EnemyAI settings)
                animator.SetFloat(hashSpeed, agent.velocity.magnitude, 0.1f, Time.deltaTime);
            }
        }

        private void UpdateAimingState()
        {
            if (!useAimingState || animator == null || enemyVision == null) return;

            // If the enemy sees the player, switch to "Aiming" posture
            bool shouldAim = enemyVision.IsPlayerDetected;
            animator.SetBool(hashIsAiming, shouldAim);
        }

        private void PlayHitReaction()
        {
            if (animator != null)
            {
                animator.SetTrigger(hashHit);
            }
        }

        // --- Animation Events ---
        // These can be called by Animation Events on the Kevin Iglesias animations
        
        public void AnimEvent_Footstep()
        {
            // Play footstep sound
        }

        public void AnimEvent_Shoot()
        {
            // Optional: You can move the shooting logic from GunEnemyAttack.cs 
            // into here if you want the bullet to fire EXACTLY on a specific animation frame.
        }

        public void AnimEvent_MeleeHit()
        {
            // Optional: Trigger melee damage exactly when the gun butt swings.
        }
    }
}
