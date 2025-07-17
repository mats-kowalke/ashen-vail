using UnityEngine;

[RequireComponent(typeof(RespawnHandler))]
[RequireComponent(typeof(HealthBar))]
public class Health : MonoBehaviour
{

    public float initialHealth;
    public float maxHealth;
    public float damageCooldown;

    public float currentHealth;
    private float timeAtLastDamage;
    private Vector3 respawnPosition;

    private RespawnHandler respawnHandler;
    private HealthBar healthBar;

    private bool canTakeDamage;

    public void Start()
    {
        this.canTakeDamage = true;
        this.currentHealth = this.initialHealth;
        this.timeAtLastDamage = 0.0f;
        this.respawnPosition = this.transform.position;

        this.respawnHandler = this.GetComponent<RespawnHandler>();
        this.healthBar = this.GetComponent<HealthBar>();

        this.healthBar.SetMinHealth(0.0f);
        this.healthBar.SetMaxHealth(this.maxHealth);
        this.healthBar.UpdateHealthBar(this.initialHealth);
    }

    public void TakeDamage(float amount)
    {
        if (Accessor.blockingHandler.isBlocking)
        {
            amount *= 1 / Mathf.Sqrt(Accessor.currentSword.properties.armourValue);
        }
        if (this.canTakeDamage && Time.time - this.timeAtLastDamage > this.damageCooldown)
        {
            this.timeAtLastDamage = Time.time;
            this.currentHealth -= amount;
            this.healthBar.UpdateHealthBar(this.currentHealth);
            if (this.currentHealth <= 0)
            {
                this.respawnHandler.Respawn(respawnPosition);
            }
        }
    }

    public void Heal(float amount)
    {
        this.currentHealth = (this.currentHealth + amount <= this.maxHealth) ? this.currentHealth + amount : maxHealth;
        this.healthBar.UpdateHealthBar(this.currentHealth);
    }

    public void SetRespawnPoint()
    {
        this.respawnPosition = this.transform.position;
    }

    public void ResetHealth()
    {
        this.currentHealth = initialHealth;
        this.timeAtLastDamage = Time.time;
        this.healthBar.UpdateHealthBar(this.currentHealth);
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


}
