using UnityEngine;

namespace Doom_Dude.EnemyASI
{
    public class AttackStateBehaviour : StateMachineBehaviour
    {
        [Tooltip("At what percentage of the animation should the damage/projectile be triggered? (0.0 to 1.0)")]
        [Range(0f, 1f)]
        public float attackTriggerTime = 0.5f;

        private bool hasTriggered;
        private EnemyAnimationHandler animationHandler;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            hasTriggered = false;

            // Cache the reference for performance so we don't call GetComponent every frame
            if (animationHandler == null)
            {
                animationHandler = animator.GetComponent<EnemyAnimationHandler>();
            }
        }

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (!hasTriggered && animationHandler != null)
            {
                // normalizedTime goes from 0.0 (start) to 1.0 (end) of the animation clip
                // If it loops, it goes 1.0 -> 2.0, etc. We use modulo 1 to handle looping safely.
                float currentNormalizedTime = stateInfo.normalizedTime % 1f;

                if (currentNormalizedTime >= attackTriggerTime)
                {
                    animationHandler.TriggerAttack();
                    hasTriggered = true;
                }
            }
        }
    }
}
