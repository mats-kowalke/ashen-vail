using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

namespace BehaviorTree
{
    public class EnemyStats : MonoBehaviour
    {
        [Header("Combat")] 
        public float attackDamage = 20f;
        public float attackRange = 2f;
        public float attackCooldown = 2f;
        public float evadeChance = 0.3f;
        
        [Header("Movement")]
        public float movementSpeed = 3.5f;

        [Header("Aggro Settings")]
        public float aggroTimeout = 0.5f;
        private float lastDetection;
        
        [Header("Rage Mode")]
        public float damageMultiplier = 1.2f;
        public float speedMultiplier = 1.15f;
        public float coolDownMultiplier = 0.8f;
        private bool inRage = false;

        private NavMeshAgent agent;
        private EnemyHealth health;
        private Animator animator;

        [HideInInspector]
        public bool canAttack = true;
        public bool isAggro = false;

        private float actualDamage;
        private float actualCooldown;
        private float actualSpeed;

        public void Start()
        {
            this.agent = this.GetComponent<NavMeshAgent>();
            this.health = this.GetComponent<EnemyHealth>();
            this.animator = this.GetComponent<Animator>();
            
            this.agent.speed = this.movementSpeed;

            this.actualSpeed = this.movementSpeed;
            this.actualDamage = this.attackDamage;
            this.actualCooldown = this.attackCooldown;
        }

        public bool ShouldRage()
        {
            return this.health.currentHealth <= this.health.maxHealth / 2 && !this.inRage;
        }

        public bool ShouldLoseAggro()
        {
            return this.isAggro && Time.time - this.lastDetection > this.aggroTimeout;
        }

        public void EnterRage()
        {
            this.inRage = true;
            this.actualDamage = this.attackDamage * this.damageMultiplier;
            this.actualSpeed = this.movementSpeed * this.speedMultiplier;
            this.actualCooldown = this.attackCooldown * this.coolDownMultiplier;
            this.agent.speed = this.movementSpeed * this.speedMultiplier;
        }

        public void StartCooldown()
        {
            this.EndAttack();
            this.StartCoroutine(CoolDownRoutine());

        }

        public void EndAttack()
        {
            this.animator.SetBool("isAttacking", false);
        }

        public void UpdateDetection(bool playerDetected)
        {
            if(playerDetected) this.lastDetection = Time.time;
        }
        
        private IEnumerator CoolDownRoutine()
        {
            this.canAttack = false;
            yield return new WaitForSeconds(this.actualCooldown);
            this.canAttack = true;
        }
        
    }
}