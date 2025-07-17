using UnityEngine;

[RequireComponent(typeof(HealthBar))]
public class EnemyHealth : MonoBehaviour
{
    public float initialHealth;
    public float maxHealth;
    public float damageCooldown;

    [HideInInspector]
    public float currentHealth;
    private float timeAtLastDamage;
    private HealthBar healthBar;

    private bool canTakeDamage;

    public void Start()
    {
        this.canTakeDamage = true;
        this.currentHealth = this.initialHealth;
        this.timeAtLastDamage = 0.0f;
        this.healthBar = this.GetComponent<HealthBar>();

        this.healthBar.SetMinHealth(0.0f);
        this.healthBar.SetMaxHealth(this.maxHealth);
        this.healthBar.UpdateHealthBar(this.initialHealth);
    }

    public void TakeDamage(float amount)
    {
        if (this.canTakeDamage && Time.time - this.timeAtLastDamage > this.damageCooldown)
        {
            this.timeAtLastDamage = Time.time;
            this.currentHealth -= amount;
            this.healthBar.UpdateHealthBar(this.currentHealth);
            if (this.currentHealth <= 0)
            {
                Destroy(this.gameObject);
            }
        }
    }


    public void DisableDamage()
    {
        this.canTakeDamage = false;
    }

    public void EnableDamage()
    {
        this.canTakeDamage = true;
    }

    public void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Water"))
        {
            this.TakeDamage(10000);
        }
    }

    public bool IsAlive()
    {
        return this.currentHealth > 0;
    }


}

