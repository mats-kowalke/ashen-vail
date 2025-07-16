using System;
using UnityEngine;

[RequireComponent(typeof(CombatAnimator))]
public class CombatHandler : MonoBehaviour
{
    public SwordHolder swordHolder;
    public bool isAttacking;
    private bool lastInward;
    private bool canAttack;
    private CombatAnimator combatAnimator;

    private WalkHandler walkHandler;
    private RollHandler rollHandler;
    private JumpHandler jumpHandler;
    private BlockingHandler blockingHandler;


    public void Start()
    {
        this.isAttacking = false;
        this.canAttack = true;
        this.lastInward = UnityEngine.Random.value < 0.5f;
        this.combatAnimator = this.GetComponent<CombatAnimator>();

        this.walkHandler = this.GetComponent<WalkHandler>();
        this.rollHandler = this.GetComponent<RollHandler>();
        this.jumpHandler = this.GetComponent<JumpHandler>();
        this.blockingHandler = this.GetComponent<BlockingHandler>();

        Accessor.combatHandler = this;
    }

    public void DoAttack()
    {
        if (this.canAttack)
        {
            if (!isAttacking && this.swordHolder.holdsSword)
            {
                this.DisableOther();
                this.Invoke(nameof(EnableOther), GetSpeedDelay());
                this.isAttacking = true;
                this.lastInward = !lastInward;
                this.Invoke(nameof(ResetAttacking), GetSpeedDelay());
                this.combatAnimator.UpdateMultiplier(GetSpeedDelay() / 2);
                this.combatAnimator.TriggerAttack(lastInward);
            }
        }
    }

    private float GetSpeedDelay()
    {
        foreach (Transform child in this.swordHolder.transform)
        {
            if (child.gameObject.CompareTag("Sword"))
            {
                float speedRaw = child.GetComponent<SwordContainer>().properties.speed;
                return (1 / Mathf.Sqrt(speedRaw)) * 3; 
            }
        }
        return 1;
    }

    private void DisableOther()
    {
        if (this.walkHandler != null) this.walkHandler.DisableWalking();
        if (this.jumpHandler != null) this.jumpHandler.DisableJumping();
        if (this.blockingHandler != null) this.blockingHandler.DisableBlocking(); 
    }

    private void EnableOther()
    {
        if (this.walkHandler != null) this.walkHandler.EnableWalking();
        if (this.jumpHandler != null) this.jumpHandler.EnableJumping();
        if (this.blockingHandler != null) this.blockingHandler.EnableBlocking();
    }

    private void ResetAttacking()
    {
        this.isAttacking = false;
    }

    public void EnableAttacking()
    {
        this.canAttack = true;
    }

    public void DisableAttacking()
    {
        this.isAttacking = false;
        this.canAttack = false;
    }

}
