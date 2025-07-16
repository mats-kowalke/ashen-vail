using UnityEngine;

public class CombatAnimator : MonoBehaviour
{
    private Animator animator;

    public void Start()
    {
        this.animator = this.GetComponentInChildren<Animator>();
    }

    public void TriggerDrawSword()
    {
        this.animator.SetTrigger("drawSword");
    }

    public void TriggerSheathSword()
    {
        this.animator.SetTrigger("sheathSword");
    }

    public void TriggerAttack(bool inward)
    {
        this.animator.SetTrigger("attack");
        this.animator.SetBool("inward", inward);
    }

    public void UpdateMultiplier(float attackSpeed)
    {
        this.animator.SetFloat("attackMultiplier", 1 / attackSpeed);
    }
}