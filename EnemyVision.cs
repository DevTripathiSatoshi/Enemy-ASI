using UnityEngine;

namespace Doom_Dude.EnemyASI
{
    public class EnemyVision : MonoBehaviour
    {
        [Header("Vision Settings")]
        [SerializeField] private float viewRadius = 15f;
        [SerializeField] [Range(0, 360)] private float viewAngle = 90f;

        [Header("Layer Masks")]
        [SerializeField] private LayerMask targetMask; // The player layer
        [SerializeField] private LayerMask obstacleMask; // Layers that block vision

        private Transform detectedPlayer;
        public bool IsPlayerDetected => detectedPlayer != null;
        public Transform DetectedPlayer => detectedPlayer;

        private void Update()
        {
            FindVisibleTargets();
        }

        private void FindVisibleTargets()
        {
            detectedPlayer = null;

            Collider[] targetsInViewRadius = Physics.OverlapSphere(transform.position, viewRadius, targetMask);

            for (int i = 0; i < targetsInViewRadius.Length; i++)
            {
                Transform target = targetsInViewRadius[i].transform;
                Vector3 dirToTarget = (target.position - transform.position).normalized;

                if (Vector3.Angle(transform.forward, dirToTarget) < viewAngle / 2)
                {
                    float dstToTarget = Vector3.Distance(transform.position, target.position);

                    // If raycast doesn't hit an obstacle, we can see the target
                    if (!Physics.Raycast(transform.position, dirToTarget, dstToTarget, obstacleMask))
                    {
                        detectedPlayer = target;
                        return; // Found the player, we can stop checking
                    }
                }
            }
        }

        // For debugging in editor
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.white;
            Gizmos.DrawWireSphere(transform.position, viewRadius);

            Vector3 viewAngleA = DirFromAngle(-viewAngle / 2, false);
            Vector3 viewAngleB = DirFromAngle(viewAngle / 2, false);

            Gizmos.DrawLine(transform.position, transform.position + viewAngleA * viewRadius);
            Gizmos.DrawLine(transform.position, transform.position + viewAngleB * viewRadius);

            if (detectedPlayer != null)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawLine(transform.position, detectedPlayer.position);
            }
        }

        private Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal)
        {
            if (!angleIsGlobal)
            {
                angleInDegrees += transform.eulerAngles.y;
            }
            return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
        }
    }
}
