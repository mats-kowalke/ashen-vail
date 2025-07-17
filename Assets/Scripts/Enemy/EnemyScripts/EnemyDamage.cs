using System;
using UnityEngine;

namespace BehaviorTree
{
    [RequireComponent(typeof(EnemyHealth))]
    public class EnemyDamage : MonoBehaviour
    {
        private EnemyHealth health;

        public void Start()
        {
            health = GetComponent<EnemyHealth>();
        }
        
        public void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Sword") && Accessor.combatHandler.isAttacking)
            {
                Debug.Log("Taken damage: " + Mathf.Sqrt(Accessor.currentSword.properties.weaponValue) * 5);
                health.TakeDamage(Mathf.Sqrt(Accessor.currentSword.properties.weaponValue) * 5);
            }
        }
    }
}