using UnityEngine;

[RequireComponent(typeof(JumpAnimator))]
public class JumpHandler : MonoBehaviour
{
    public float jumpForce;
    private bool isJumping;
    private bool isFalling;
    private bool isGrounded;
    private bool canJump;
    private new Rigidbody rigidbody;
    private JumpAnimator jumpAnimator;

    private RollHandler rollHandler;
    private CombatHandler combatHandler;
    private BlockingHandler blockingHandler;

    public void Start()
    {
        this.canJump = true;
        this.rigidbody = this.GetComponent<Rigidbody>();
        this.jumpAnimator = this.GetComponent<JumpAnimator>();

        this.rollHandler = this.GetComponent<RollHandler>();
        this.combatHandler = this.GetComponent<CombatHandler>();
        this.blockingHandler = this.GetComponent<BlockingHandler>();
    }

    public void Update()
    {
        if (this.canJump)
        {
            UpdateJumpState();
            this.jumpAnimator.UpdateJumpingParameters(isJumping, isFalling, isGrounded);
        }
    }

    public void DoJump()
    {
        if (this.canJump && Physics.Raycast(this.transform.position, Vector3.down, 0.1f))
        {
            this.isJumping = true;
            this.rigidbody.AddForce(Vector3.up * this.jumpForce);
        }
    }

    private void UpdateJumpState()
    {
        if (Physics.Raycast(this.transform.position, Vector3.down, 0.1f))
        {
            this.isGrounded = true;
            this.isFalling = false;
            this.isJumping = false;
            this.EnableOther();
        }
        else
        {
            this.isGrounded = false;
            this.isFalling = true;
            this.DisableOther();
        }
    }

    private void DisableOther()
    {
        if (this.rollHandler != null) this.rollHandler.DisableRolling();
        if (this.combatHandler != null) this.combatHandler.DisableAttacking();
        if (this.blockingHandler != null) this.blockingHandler.DisableBlocking();
    }

    private void EnableOther()
    {
        if (this.rollHandler != null) this.rollHandler.EnableRolling();
        if (this.combatHandler != null) this.combatHandler.EnableAttacking();
        if (this.blockingHandler != null) this.blockingHandler.EnableBlocking();
    }

    public void DisableJumping()
    {
        this.isJumping = false;
        this.isFalling = false;
        this.isGrounded = true;
        this.canJump = false;
    }

    public void EnableJumping()
    {
        this.canJump = true;
    }

}