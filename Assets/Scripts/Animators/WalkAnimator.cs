using UnityEngine;

public class WalkAnimator : MonoBehaviour
{

    private Animator animator;

    public void Start()
    {
        this.animator = this.GetComponentInChildren<Animator>();
    }

    public void UpdateWalkingParameters(Vector3 movement, bool isSprinting)
    {
        float clampedX, clampedY;
        if (movement.x > 0) clampedX = 1;
        else if (movement.x < 0) clampedX = -1;
        else clampedX = 0;
        if (movement.z > 0) clampedY = 1;
        else if (movement.z < 0) clampedY = -1;
        else clampedY = 0;

        this.animator.SetFloat("moveX", clampedX);
        this.animator.SetFloat("moveY", clampedY);
        this.animator.SetBool("isSprinting", isSprinting);
    }
}