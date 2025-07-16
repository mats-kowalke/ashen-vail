using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(DeathAnimator))]
public class RespawnHandler : MonoBehaviour
{
    public SwordHolder swordHolder;
    public GameObject gameOverScreen;
    public TimerController timerController;

    private DeathAnimator deathAnimator;
    private Health health;

    private WalkHandler walkHandler;
    private RollHandler rollHandler;
    private JumpHandler jumpHandler;
    private CombatHandler combatHandler;
    private RotationHandler rotationHandler;

    private XPHandler xPHandler;

    private Vector3 respawnPosition;

    public void Start()
    {
        this.deathAnimator = this.GetComponent<DeathAnimator>();
        this.health = this.GetComponent<Health>();

        this.walkHandler = this.GetComponent<WalkHandler>();
        this.rollHandler = this.GetComponent<RollHandler>();
        this.jumpHandler = this.GetComponent<JumpHandler>();
        this.combatHandler = this.GetComponent<CombatHandler>();
        this.rotationHandler = this.GetComponent<RotationHandler>();

        this.xPHandler = this.GetComponent<XPHandler>();

        this.respawnPosition = this.transform.position;

        this.gameOverScreen.SetActive(false);
    }

    public void Respawn(Vector3 respawnPosition)
    {
        this.deathAnimator.TriggerDeath();
        this.gameOverScreen.SetActive(true);
        this.ResetPlayer();
        this.respawnPosition = respawnPosition;
        Invoke(nameof(DoRespawn), 5);
    }

    private void DoRespawn()
    {
        this.transform.position = this.respawnPosition;
        this.deathAnimator.TriggerRespawn();
        this.health.ResetHealth();
        this.timerController.DecrementTime(20);
        this.StartCoroutine(RespawnRoutine());
    }

    private IEnumerator RespawnRoutine()
    {
        this.transform.position = this.respawnPosition;
        this.deathAnimator.TriggerRespawn();
        yield return new WaitForSeconds(1);
        this.EnablePlayer();
        this.gameOverScreen.SetActive(false);
    }

    private void ResetPlayer()
    {
        this.walkHandler.DisableWalking();
        this.walkHandler.DisableSprinting();
        this.swordHolder.DisableSword();
        this.combatHandler.DisableAttacking();
        this.rollHandler.DisableRolling();
        this.jumpHandler.DisableJumping();
        this.rotationHandler.DisableRotation();
        this.xPHandler.ResetXP();
    }

    private void EnablePlayer()
    {
        this.walkHandler.EnableWalking();
        this.walkHandler.EnableSprinting();
        this.swordHolder.EnableSword();
        this.combatHandler.EnableAttacking();
        this.rollHandler.EnableRolling();
        this.jumpHandler.EnableJumping();
        this.rotationHandler.EnableRotation();
    }

}