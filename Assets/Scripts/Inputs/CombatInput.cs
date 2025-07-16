using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CombatHandler))]
public class CombatInput : MonoBehaviour
{
    public SwordHolder swordHolder;
    private bool attackFlag;
    private bool switchFlag;
    private CombatHandler combatHandler;

    public void Start()
    {
        this.combatHandler = this.GetComponent<CombatHandler>();
    }

    public void Update()
    {
        if (this.attackFlag) { this.combatHandler.DoAttack(); this.attackFlag = false; }
        if (this.switchFlag) { this.swordHolder.SwitchSword(); this.switchFlag = false; }
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            this.attackFlag = true;
        }
    }

    public void OnSwordSwitch(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            this.switchFlag = true;
        }
    }
}