using System.Collections;
using UnityEngine;

[RequireComponent(typeof(RollAnimator))]
public class RollHandler : MonoBehaviour
{
    public float rollSpeed;
    public AnimationCurve rollingCurve;
    private new Rigidbody rigidbody;
    private RollAnimator rollAnimator;
    private float rollTime;
    private bool canRoll;

    private WalkHandler walkHandler;
    private JumpHandler jumpHandler;
    private SwordHolder swordHolder;
    private CombatHandler combatHandler;
    private BlockingHandler blockingHandler;
    private Health health;

    public void Start()
    {
        this.canRoll = true;
        this.rigidbody = this.GetComponent<Rigidbody>();
        this.rollAnimator = this.GetComponent<RollAnimator>();
        this.rollTime = this.rollingCurve[this.rollingCurve.length - 1].time;

        this.walkHandler = this.GetComponent<WalkHandler>();
        this.jumpHandler = this.GetComponent<JumpHandler>();
        this.swordHolder = this.GetComponent<SwordHolder>();
        this.combatHandler = this.GetComponent<CombatHandler>();
        this.blockingHandler = this.GetComponent<BlockingHandler>();
        this.health = this.GetComponent<Health>();
    }

    public void DoRoll()
    {
        if (this.canRoll)
        {
            this.DisableOther();
            this.Invoke(nameof(EnableOther), this.rollTime);
            this.StartCoroutine(Roll());
            this.rollAnimator.TriggerRoll();
        }
    }

    private IEnumerator Roll()
    {
        float timer = 0;
        while (timer < this.rollTime)
        {
            float speed = this.rollingCurve.Evaluate(timer) * this.rollSpeed;
            this.rigidbody.MovePosition(this.transform.position + speed * Time.deltaTime * this.transform.forward);
            timer += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        yield return null;
    }

    private void DisableOther()
    {
        if (this.walkHandler != null) this.walkHandler.DisableWalking();
        if (this.walkHandler != null) this.walkHandler.DisableSprinting();
        if (this.jumpHandler != null) this.jumpHandler.DisableJumping();
        if (this.swordHolder != null) this.swordHolder.DisableSword();
        if (this.combatHandler != null) this.combatHandler.DisableAttacking();
        if (this.health != null) this.health.DisableDamage();
        if (this.blockingHandler != null) this.blockingHandler.DisableBlocking();
    }

    private void EnableOther()
    {
        if (this.walkHandler != null) this.walkHandler.EnableWalking();
        if (this.walkHandler != null) this.walkHandler.EnableSprinting();
        if (this.jumpHandler != null) this.jumpHandler.EnableJumping();
        if (this.swordHolder != null) this.swordHolder.EnableSword();
        if (this.combatHandler != null) this.combatHandler.EnableAttacking();
        if (this.health != null) this.health.EnableDamage();
        if (this.blockingHandler != null) this.blockingHandler.EnableBlocking();
    }

    public void DisableRolling()
    {
        this.StopCoroutine(Roll());
        this.canRoll = false;
    }

    public void EnableRolling()
    {
        this.canRoll = true;
    }

}