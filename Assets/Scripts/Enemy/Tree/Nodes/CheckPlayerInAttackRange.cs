using UnityEngine;

namespace BehaviorTree
{
    public class CheckPlayerInAttackRange : Node
    {
        private PlayerDetection playerDetection;
        private EnemyStats stats;

        public CheckPlayerInAttackRange(Transform transform)
        {
            this.playerDetection = transform.GetComponent<PlayerDetection>();
            this.stats = transform.GetComponent<EnemyStats>();
        }

        public override State Evaluate()
        {
            bool inAttackRange = false;
            if (this.playerDetection.PlayerInRange())
            {
                float distanceToPlayer = this.playerDetection.GetDistanceToPlayer();
                inAttackRange = distanceToPlayer <= this.stats.attackRange;
            }
            this.stats.UpdateDetection(inAttackRange);
            if (!inAttackRange && this.stats.isAggro)
            {
                this.stats.isAggro = false;
            }

            return inAttackRange ? State.SUCCESS : State.FAILIURE;
        }
    }
}