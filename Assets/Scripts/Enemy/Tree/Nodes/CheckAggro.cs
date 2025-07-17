using UnityEngine;

namespace BehaviorTree
{
    public class CheckAggro : Node
    {
        private EnemyHealth health;
        private Animator animator;
        private PlayerDetection playerDetection;
        
        private float resumeChaseDistance = 10f;

        public CheckAggro(Transform transform)
        {
            this.animator = transform.GetComponentInChildren<Animator>();
            this.health = transform.GetComponent<EnemyHealth>();
            this.playerDetection = transform.GetComponent<PlayerDetection>();
        }

        public override State Evaluate()
        {
            if (this.health.IsAlive() && playerDetection.GetDistanceToPlayer() < resumeChaseDistance)
            {
                return State.SUCCESS;
            }
            animator.SetBool("isChasing", false);
            return State.FAILIURE;
        }
    }
}