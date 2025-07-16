using UnityEngine;

public class BlockingAnimator : MonoBehaviour
{

    private Animator animator;

    public void Start()
    {
        this.animator = this.GetComponentInChildren<Animator>();
    }

    public void SetBlocking(bool isBlocking)
    {
        this.animator.SetBool("blocking", isBlocking);
    }

}