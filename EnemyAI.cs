using UnityEngine;
using UnityEngine.AI;

namespace Doom_Dude.EnemyASI
{
    public enum EnemyState
    {
        Idle,
        Walk, // Patrol
        Run,  // Chase
        Attack,
        Die
    }

    [RequireComponent(typeof(NavMeshAgent))]
    public class EnemyAI : MonoBehaviour
    {
        [Header("State Settings")]
        [SerializeField] private EnemyState currentState = EnemyState.Idle;

        [Header("Movement Speeds")]
        [SerializeField] private float walkSpeed = 3.5f;
        [SerializeField] private float runSpeed = 6f;

        [Header("Components")]
        [SerializeField] private Animator animator; // Optional, assigned in inspector
        [SerializeField] private EnemyVision vision;
        [SerializeField] private EnemyHealth health;
        [SerializeField] private EnemyPatrol patrol;
        [SerializeField] private EnemyAttack attack;

        private NavMeshAgent agent;

        // Animator Hashes for performance
        private readonly int hashWalk = Animator.StringToHash("Walk");
        private readonly int hashRun = Animator.StringToHash("Run");
        private readonly int hashAttack = Animator.StringToHash("Attack");
        private readonly int hashDie = Animator.StringToHash("Die");

        private void Awake()
        {
            agent = GetComponent<NavMeshAgent>();
            if (vision == null) vision = GetComponent<EnemyVision>();
            if (health == null) health = GetComponent<EnemyHealth>();
            if (patrol == null) patrol = GetComponent<EnemyPatrol>();
            if (attack == null) attack = GetComponent<EnemyAttack>();
            if (animator == null) animator = GetComponentInChildren<Animator>();

            if (health != null)
            {
                health.OnDeath.AddListener(HandleDeath);
            }
        }

        private void Start()
        {
            SwitchState(EnemyState.Idle);
        }

        private void Update()
        {
            if (currentState == EnemyState.Die) return;

            switch (currentState)
            {
                case EnemyState.Idle:
                    UpdateIdleState();
                    break;
                case EnemyState.Walk:
                    UpdateWalkState();
                    break;
                case EnemyState.Run:
                    UpdateRunState();
                    break;
                case EnemyState.Attack:
                    UpdateAttackState();
                    break;
            }
        }

        private void SwitchState(EnemyState newState)
        {
            if (currentState == EnemyState.Die) return; // Can't escape death

            currentState = newState;

            // Reset Animation Triggers/Bools
            if (animator != null)
            {
                animator.SetBool(hashWalk, false);
                animator.SetBool(hashRun, false);
            }

            switch (currentState)
            {
                case EnemyState.Idle:
                    agent.isStopped = true;
                    if (patrol != null) patrol.ResetTimer();
                    break;

                case EnemyState.Walk:
                    agent.isStopped = false;
                    agent.speed = walkSpeed;
                    if (animator != null) animator.SetBool(hashWalk, true);
                    if (patrol != null && patrol.HasPatrolPoints)
                    {
                        agent.SetDestination(patrol.GetCurrentPatrolPoint());
                    }
                    break;

                case EnemyState.Run:
                    agent.isStopped = false;
                    agent.speed = runSpeed;
                    if (animator != null) animator.SetBool(hashRun, true);
                    break;

                case EnemyState.Attack:
                    agent.isStopped = true;
                    break;

                case EnemyState.Die:
                    agent.isStopped = true;
                    agent.enabled = false;
                    if (animator != null) animator.SetTrigger(hashDie);
                    break;
            }
        }

        private void UpdateIdleState()
        {
            CheckForPlayer();
            if (currentState != EnemyState.Idle) return;

            if (patrol != null && patrol.HasPatrolPoints)
            {
                patrol.DecreaseTimer(Time.deltaTime);
                if (patrol.CurrentWaitTimer <= 0)
                {
                    SwitchState(EnemyState.Walk);
                }
            }
        }

        private void UpdateWalkState()
        {
            CheckForPlayer();
            if (currentState != EnemyState.Walk) return;

            if (patrol != null && patrol.HasPatrolPoints)
            {
                // Check if reached destination
                if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
                {
                    patrol.IncrementPatrolIndex();
                    SwitchState(EnemyState.Idle);
                }
            }
        }

        private void UpdateRunState()
        {
            if (vision != null && vision.IsPlayerDetected)
            {
                Transform target = vision.DetectedPlayer;
                float distance = Vector3.Distance(transform.position, target.position);

                if (attack != null && distance <= attack.AttackRange)
                {
                    SwitchState(EnemyState.Attack);
                    return;
                }

                // Keep chasing
                agent.SetDestination(target.position);
            }
            else
            {
                // Lost player, go back to patrol
                SwitchState(EnemyState.Walk);
            }
        }

        private void UpdateAttackState()
        {
            if (vision == null || attack == null)
            {
                SwitchState(EnemyState.Idle);
                return;
            }

            if (vision.IsPlayerDetected)
            {
                Transform target = vision.DetectedPlayer;
                float distance = Vector3.Distance(transform.position, target.position);

                // Face the player
                Vector3 direction = (target.position - transform.position).normalized;
                direction.y = 0; // Keep rotation strictly horizontal
                if (direction != Vector3.zero)
                {
                    transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), Time.deltaTime * 5f);
                }

                if (distance <= attack.AttackRange)
                {
                    if (attack.CanAttack())
                    {
                        if (animator != null)
                        {
                            UniversalEnemyAttack universalAttack = attack as UniversalEnemyAttack;
                            if (universalAttack != null && universalAttack.numberOfAttackVariations > 0)
                            {
                                int randomAttack = Random.Range(0, universalAttack.numberOfAttackVariations);
                                animator.SetInteger("AttackIndex", randomAttack);
                            }
                            animator.SetTrigger(hashAttack);
                        }
                        attack.PerformAttack(target);
                    }
                }
                else
                {
                    // Player moved out of range, chase again
                    SwitchState(EnemyState.Run);
                }
            }
            else
            {
                // Lost player
                SwitchState(EnemyState.Walk);
            }
        }

        private void CheckForPlayer()
        {
            if (vision != null && vision.IsPlayerDetected)
            {
                SwitchState(EnemyState.Run);
            }
        }

        private void HandleDeath()
        {
            SwitchState(EnemyState.Die);
        }
    }
}
