using UnityEngine;

namespace BehaviorTree
{
    public class TaskChase : Node
    {
        private EnemyChase enemyChase;
        private EnemyWandering enemyWandering;
        private EnemyHealth health;
        private Animator animator;
        private EnemyStats stats;

        public TaskChase(Transform transform)
        {
            enemyChase = transform.GetComponent<EnemyChase>();
            enemyWandering = transform.GetComponent<EnemyWandering>();
            animator = transform.GetComponentInChildren<Animator>();
            stats = transform.GetComponent<EnemyStats>();
            health = transform.GetComponent<EnemyHealth>();
        }

        public override State Evaluate()
        {
            if (!health.IsAlive())
            {
                return State.FAILIURE;
            }

            stats.isAggro = true;
        
            stats.UpdateDetection(true);
        
            enemyWandering.StopWander();
            
            animator.SetBool("isChasing", true);

            enemyChase.StartChase();
            return State.RUNNING;
        }
    }
}