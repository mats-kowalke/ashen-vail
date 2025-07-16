using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

[RequireComponent(typeof(Health))]
public class Healing : MonoBehaviour
{

    public UIComponent healComponent;
    public Slider slider;
    public float healingCooldown;
    public float healAmount;
    public float portableHealAmount;
    public float initialHeals;
    private float leftOverHeals;
    private Health health;
    private Animator animator;
    private float timeSinceLastHeal;
    private bool healing;
    private bool canHeal;

    public void Start()
    {
        this.health = this.GetComponent<Health>();
        this.animator = this.GetComponentInChildren<Animator>();
        this.timeSinceLastHeal = Time.time;
        this.canHeal = false;
        this.healComponent.Deactivate();
        this.leftOverHeals = this.initialHeals;
        this.slider.minValue = 0;
        this.slider.wholeNumbers = true;
        this.slider.maxValue = this.initialHeals;
        this.slider.value = this.initialHeals;
    }

    public void Update()
    {
        if (canHeal && healing && Time.time - this.timeSinceLastHeal > this.healingCooldown)
        {
            this.timeSinceLastHeal = Time.time;
            this.health.Heal(this.healAmount);
            this.health.SetRespawnPoint();
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Bonfire"))
        {
            if (!this.healing) this.healing = true;
            this.healComponent.Activate();
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Bonfire"))
        {
            if (this.healing) this.healing = false;
            if (this.canHeal) this.canHeal = false;
            this.healComponent.Deactivate();
        }
    }

    public void OnHeal(InputAction.CallbackContext context)
    {
        if (this.healing && context.performed)
        {
            this.canHeal = true;
            this.leftOverHeals = this.initialHeals;
            this.slider.value = this.leftOverHeals;
        }
        if (!this.healing && context.performed)
        {
            if (Time.time - this.timeSinceLastHeal > this.healingCooldown &&
                this.leftOverHeals > 0)
            {
                this.animator.SetTrigger("heal");
                this.leftOverHeals--;
                this.slider.value = this.leftOverHeals;
                this.timeSinceLastHeal = Time.time;
                this.health.Heal(this.portableHealAmount);
            }
        }
    }

}