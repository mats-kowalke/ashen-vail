using UnityEngine;

public class JumpAnimator : MonoBehaviour
{
    private Animator animator;

    public void Start()
    {
        this.animator = this.GetComponentInChildren<Animator>();
    }

    public void UpdateJumpingParameters(bool isJumping, bool isFalling, bool isGrounded)
    {
        this.animator.SetBool("isJumping", isJumping);
        this.animator.SetBool("isFalling", isFalling);
        this.animator.SetBool("isGrounded", isGrounded);
    }
}