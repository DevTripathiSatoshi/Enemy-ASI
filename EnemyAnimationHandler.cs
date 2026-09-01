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
        [SerializeField] private EnemyAttack enemyAttack;
        [SerializeField] private NavMeshAgent agent;

        [Header("Animation Settings")]
        [SerializeField] private bool useAimingState = true;
        [SerializeField] private bool useHitReactions = true;

        // Animator Hashes for Kevin Iglesias pack mapping
        private readonly int hashIsAiming = Animator.StringToHash("IsAiming");
        private readonly int hashVelocityX = Animator.StringToHash("VelocityX");
        private readonly int hashVelocityZ = Animator.StringToHash("VelocityZ");
        private readonly int hashHit = Animator.StringToHash("Hit");

        private void Awake()
        {
            if (animator == null) animator = GetComponent<Animator>();
            if (enemyAI == null) enemyAI = GetComponentInParent<EnemyAI>();
            if (enemyHealth == null) enemyHealth = GetComponentInParent<EnemyHealth>();
            if (enemyVision == null) enemyVision = GetComponentInParent<EnemyVision>();
            if (enemyAttack == null) enemyAttack = GetComponentInParent<EnemyAttack>();
            if (agent == null) agent = GetComponentInParent<NavMeshAgent>();
        }

        private void Start()
        {
            // Apply Weapon Animation Override if provided
            UniversalEnemyAttack universalAttack = enemyAttack as UniversalEnemyAttack;
            if (animator != null && universalAttack != null && universalAttack.weaponAnimatorOverride != null)
            {
                animator.runtimeAnimatorController = universalAttack.weaponAnimatorOverride;
            }
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
                // Convert world velocity to local velocity for strafing support (2D Blend Tree)
                Vector3 localVelocity = transform.InverseTransformDirection(agent.velocity);
                
                // localVelocity.x is left/right (- for left, + for right)
                // localVelocity.z is forward/backward (- for back, + for forward)
                
                animator.SetFloat(hashVelocityX, localVelocity.x, 0.1f, Time.deltaTime);
                animator.SetFloat(hashVelocityZ, localVelocity.z, 0.1f, Time.deltaTime);
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
            TriggerAttack();
        }

        public void AnimEvent_MeleeHit()
        {
            TriggerAttack();
        }

        // Called by StateMachineBehaviour as an alternative to Animation Events
        public void TriggerAttack()
        {
            if (enemyAttack != null)
            {
                enemyAttack.TriggerAttackEvent();
            }
        }
    }
}
