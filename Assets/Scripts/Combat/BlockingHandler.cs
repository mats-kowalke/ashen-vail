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

    public void Start()
    {
        this.blockingAnimator = this.GetComponent<BlockingAnimator>();
        this.isBlocking = false;
        this.canBlock = true;

        this.walkHandler = this.GetComponent<WalkHandler>();
        this.jumpHandler = this.GetComponent<JumpHandler>();
        this.combatHandler = this.GetComponent<CombatHandler>();

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
    }

    private void EnableOther()
    {
        if (this.walkHandler != null) this.walkHandler.EnableWalking();
        if (this.jumpHandler != null) this.jumpHandler.EnableJumping();
        if (this.combatHandler != null) this.combatHandler.EnableAttacking();
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