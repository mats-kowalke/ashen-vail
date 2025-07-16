using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{

    public Slider slider;
    public Gradient gradient;
    public Image fill;

    public void UpdateHealthBar(float health)
    {
        this.slider.value = health;
        this.fill.color = gradient.Evaluate(this.slider.normalizedValue);
    }

    public void SetMaxHealth(float maxHealth)
    {
        this.slider.maxValue = maxHealth;
    }

    public void SetMinHealth(float minValue)
    {
        this.slider.minValue = minValue;
        this.fill.color = gradient.Evaluate(this.slider.normalizedValue);
    }
}
