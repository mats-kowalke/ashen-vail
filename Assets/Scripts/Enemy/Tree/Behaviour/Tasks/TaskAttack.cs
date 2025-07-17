using UnityEngine;

namespace BehaviorTree
{
    public class TaskAttack : Node
    {
        private EnemyStats stats;
        private EnemyCombat combat;
        private EnemyChase enemyChase;

        public TaskAttack(Transform transform)
        {
            stats = transform.GetComponent<EnemyStats>();
            combat = transform.GetComponent<EnemyCombat>();
            enemyChase = transform.GetComponent<EnemyChase>();
        }

        public override State Evaluate()
        {

            if (combat.isAttacking)
            {
                return State.RUNNING;
            }
            
            enemyChase.StopChase();
            
            combat.AttackPlayer();

            return State.SUCCESS;
        }
    }
}