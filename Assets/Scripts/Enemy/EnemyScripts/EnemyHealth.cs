using UnityEngine;

[RequireComponent(typeof(HealthBar))]
public class EnemyHealth : MonoBehaviour
{
    public float initialHealth;
    public float maxHealth;
    public float damageCooldown;
    
    [Header("Loot")]
    public int xpAmount;
    public GameObject collectiblePrefab;
    public GameObject containerPrefab;
    public GameObject swordPrefab;

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
                DropLoot();
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
    
    public void DropLoot()
    {
        XPContainer container = this.containerPrefab.GetComponent<XPContainer>();
        container.XPAmount = this.xpAmount;

        GameObject instance = Instantiate(collectiblePrefab, this.transform.position + new Vector3(0, 3, 0), Quaternion.identity);
        Collectible collectible = instance.GetComponent<Collectible>();
        collectible.SetContent(containerPrefab);

        SwordContainer swordContainer = this.swordPrefab.GetComponent<SwordContainer>();
        WeaponQualityGenerator generator = new WeaponQualityGenerator();
        generator.Start();
        swordContainer.properties = generator.GenerateWeapon(Accessor.xPHandler.currentXP);

        GameObject swordInstance = Instantiate(collectiblePrefab, this.transform.position + new Vector3(0, 3, 0), Quaternion.identity);
        Collectible swordCollectible = swordInstance.GetComponent<Collectible>();
        swordCollectible.SetContent(swordPrefab);

        Accessor.enemySpawner.DecreaseCurrentAmount();
    }


}

