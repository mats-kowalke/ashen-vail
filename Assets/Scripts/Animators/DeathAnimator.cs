using UnityEngine;

public class DeathAnimator : MonoBehaviour
{

    private Animator animator;

    public Animator respawnAnimator;

    public void Start()
    {
        this.animator = this.GetComponentInChildren<Animator>();
    }

    public void TriggerDeath()
    {
        this.animator.SetTrigger("death");
    }

    public void TriggerRespawn()
    {
        this.respawnAnimator.SetTrigger("Respawn");
    }

}