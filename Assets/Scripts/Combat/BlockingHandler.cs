using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(BlockingAnimator))]
public class BlockingHandler : MonoBehaviour
{

    public SwordHolder swordHolder;

    public bool isBlocking;
    private bool canBlock;
    private BlockingAnimator blockingAnimator;

    private WalkHandler walkHandler;
    private JumpHandler jumpHandler;
    private CombatHandler combatHandler;
    private Health health;

    public void Start()
    {
        this.blockingAnimator = this.GetComponent<BlockingAnimator>();
        this.isBlocking = false;
        this.canBlock = true;

        this.walkHandler = this.GetComponent<WalkHandler>();
        this.jumpHandler = this.GetComponent<JumpHandler>();
        this.combatHandler = this.GetComponent<CombatHandler>();
        this.health = this.GetComponent<Health>();

        Accessor.blockingHandler = this;
    }

    public void Update()
    {
        this.blockingAnimator.SetBlocking(this.isBlocking && this.canBlock);
    }

    public void OnBlocking(InputAction.CallbackContext context)
    {
        if (context.started && this.canBlock && this.swordHolder.holdsSword)
        {
            this.isBlocking = true;
            this.DisableOther();
        }
        if (context.canceled && this.canBlock)
        {
            this.isBlocking = false;
            this.EnableOther();
        }
    }

    private void DisableOther()
    {
        if (this.walkHandler != null) this.walkHandler.DisableWalking();
        if (this.jumpHandler != null) this.jumpHandler.DisableJumping();
        if (this.combatHandler != null) this.combatHandler.DisableAttacking();
        if (this.health != null) this.health.DisableDamage();
    }

    private void EnableOther()
    {
        if (this.walkHandler != null) this.walkHandler.EnableWalking();
        if (this.jumpHandler != null) this.jumpHandler.EnableJumping();
        if (this.combatHandler != null) this.combatHandler.EnableAttacking();
        if (this.health != null) this.health.EnableDamage();
    }

    public void EnableBlocking()
    {
        this.canBlock = true;
    }

    public void DisableBlocking()
    {
        this.canBlock = false;
    }
}