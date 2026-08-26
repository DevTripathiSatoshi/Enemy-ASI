using UnityEngine;

namespace Doom_Dude.EnemyASI
{
    public class EnemyPatrol : MonoBehaviour
    {
        [Header("Patrol Settings")]
        [SerializeField] private Transform[] patrolPoints;
        [SerializeField] private float waitTimeAtPoint = 2f;

        private int currentPatrolIndex;
        private float currentWaitTimer;

        public bool HasPatrolPoints => patrolPoints != null && patrolPoints.Length > 0;
        public float WaitTimeAtPoint => waitTimeAtPoint;
        public float CurrentWaitTimer => currentWaitTimer;

        public void ResetTimer()
        {
            currentWaitTimer = waitTimeAtPoint;
        }

        public void DecreaseTimer(float deltaTime)
        {
            currentWaitTimer -= deltaTime;
        }

        public Vector3 GetCurrentPatrolPoint()
        {
            if (patrolPoints.Length == 0) return transform.position;
            return patrolPoints[currentPatrolIndex].position;
        }

        public void IncrementPatrolIndex()
        {
            if (patrolPoints.Length == 0) return;
            currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
        }

        public void OnDrawGizmos()
        {
            if (patrolPoints == null || patrolPoints.Length == 0) return;

            Gizmos.color = Color.green;
            for (int i = 0; i < patrolPoints.Length; i++)
            {
                if (patrolPoints[i] != null)
                {
                    Gizmos.DrawSphere(patrolPoints[i].position, 0.5f);
                    if (i < patrolPoints.Length - 1 && patrolPoints[i + 1] != null)
                    {
                        Gizmos.DrawLine(patrolPoints[i].position, patrolPoints[i + 1].position);
                    }
                    else if (patrolPoints[0] != null) // loop back to first
                    {
                        Gizmos.DrawLine(patrolPoints[i].position, patrolPoints[0].position);
                    }
                }
            }
        }
    }
}
