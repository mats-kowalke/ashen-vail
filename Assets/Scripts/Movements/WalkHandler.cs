using UnityEngine;

[RequireComponent(typeof(WalkAnimator))]
public class WalkHandler : MonoBehaviour
{
    public float walkSpeed;
    public float sprintSpeed;
    private float trueSpeed;
    private new Rigidbody rigidbody;
    private WalkAnimator walkAnimator;
    private bool canWalk;
    private bool canSprint;

    public void Start()
    {
        this.trueSpeed = this.walkSpeed;
        this.rigidbody = this.GetComponent<Rigidbody>();
        this.walkAnimator = this.GetComponent<WalkAnimator>();
        this.canWalk = true;
        this.canSprint = true;
    }
    public void UpdateWalk(Vector3 input)
    {
        if (this.canWalk)
        {
            this.rigidbody.MovePosition(
            this.transform.position +
            this.trueSpeed * Time.deltaTime * this.transform.TransformDirection(input));
            this.walkAnimator.UpdateWalkingParameters(input, this.trueSpeed == this.sprintSpeed);
        }
        else
        {
            this.walkAnimator.UpdateWalkingParameters(Vector3.zero, false);
        }
    }

    public void UpdateSprint(bool isSprinting)
    {
        if (this.canWalk && this.canSprint)
        {
            if (isSprinting) this.trueSpeed = this.sprintSpeed;
            else this.trueSpeed = this.walkSpeed;
        }
    }

    public void DisableWalking()
    {
        this.canWalk = false;
        this.trueSpeed = this.walkSpeed;
    }

    public void EnableWalking()
    {
        this.canWalk = true;
    }

    public void DisableSprinting()
    {
        this.canSprint = false;
        this.trueSpeed = this.walkSpeed;
    }

    public void EnableSprinting()
    {
        this.canSprint = true;
    }
}