using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

namespace BehaviorTree
{
    [RequireComponent(typeof(EnemyStats))]
    public class EnemyCombat : MonoBehaviour
    {
        private EnemyStats stats;
        private PlayerDetection playerDetection;
        private Animator animator;
        private NavMeshAgent navMeshAgent;
        
        [HideInInspector]
        public bool isAttacking = false;
        
        [Header("Combat Settings")]
        public float minCombatDistance = 1.5f;
        public float maxCombatDistance = 4f;
        private float optimalCombatDistance;

        public void Start()
        {
            this.stats = this.GetComponent<EnemyStats>();
            this.playerDetection = this.GetComponent<PlayerDetection>();
            this.animator = this.GetComponentInChildren<Animator>();
            this.navMeshAgent = this.GetComponent<NavMeshAgent>();
            
            this.optimalCombatDistance = (this.minCombatDistance + this.maxCombatDistance) / 2;
        }

        public void AttackPlayer()
        {
            if (this.stats.canAttack && this.playerDetection.GetDistanceToPlayer() < this.stats.attackRange)
            {
                StartCoroutine(DoAttack());
            }
        }

        private IEnumerator DoAttack()
        {
            this.navMeshAgent.isStopped = true;
            this.isAttacking = true;
            this.animator.SetTrigger("attack");
            yield return new WaitForSeconds(3);
            this.isAttacking = false;
        }
    }
}