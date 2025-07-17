using UnityEngine;
using UnityEngine.AI;

namespace BehaviorTree
{
    public class TaskWander : Node
    {
        private EnemyWandering wandering;
        private EnemyChase enemyChase;
        private NavMeshAgent navMeshAgent;
        private EnemyHealth  health;
        private Animator animator;
        private EnemyStats stats;
        
        public TaskWander(Transform transform)
        {
            wandering = transform.GetComponent<EnemyWandering>();
            enemyChase = transform.GetComponent<EnemyChase>();
            navMeshAgent =  transform.GetComponent<NavMeshAgent>();
            health = transform.GetComponent<EnemyHealth>();
            animator = transform.GetComponentInChildren<Animator>();
            stats = transform.GetComponent<EnemyStats>();
        }

        public override State Evaluate()
        {
            if (!health.IsAlive())
            {
                return State.FAILIURE;
            }

            stats.isAggro = false;
            
            stats.UpdateDetection(false);
            
            enemyChase.StopChase();

            navMeshAgent.isStopped = false;

            wandering.StartWander();
            
            animator.SetBool("isWandering", true);

            return State.RUNNING;
            
        }
        
    }
}