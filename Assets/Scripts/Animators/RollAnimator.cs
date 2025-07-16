using UnityEngine;

public class RollAnimator : MonoBehaviour
{
    private Animator animator;

    public void Start()
    {
        this.animator = this.GetComponentInChildren<Animator>();
    }

    public void TriggerRoll()
    {
        this.animator.SetTrigger("roll");
    }
}
