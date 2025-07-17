using UnityEngine;

[RequireComponent(typeof(Health))]
public class Damage : MonoBehaviour
{
    private Health health;

    public void Start()
    {
        this.health = this.GetComponent<Health>();
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("EnemySword") && this.health.currentHealth > 0)
        {
            this.health.TakeDamage(10);
        }
    }

}
