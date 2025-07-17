using UnityEngine;
using UnityEngine.AI;

namespace BehaviorTree
{
    public class EnemyChase : MonoBehaviour
    {
        private Transform player;
        private EnemyStats stats;
        private NavMeshAgent agent;
        private EnemyCombat combat;
        private PlayerDetection playerDetection;
        private bool isChasing = false;
        
        public void Start()
        {
            agent = GetComponent<NavMeshAgent>();
            player = GameObject.FindGameObjectWithTag("PlayerTag").transform;
            stats = GetComponent<EnemyStats>();
            combat = GetComponent<EnemyCombat>();
            playerDetection = player.GetComponent<PlayerDetection>();
        }

        public void Update()
        {
            if (isChasing && agent.enabled && agent.isOnNavMesh)
            {
                agent.isStopped = false;
                agent.SetDestination(player.position);
            }
        }

        public void StartChase()
        {
            isChasing = true;
        }

        public void StopChase()
        {
            isChasing = false;
            agent.isStopped = true;
        }
    }
}