using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SwordHolder : MonoBehaviour
{

    public Slider speedSlider;
    public Slider attackSlider;
    public Slider armorSlider;
    public UIComponent component;

    public bool holdsSword;
    public GameObject player;
    private CombatAnimator combatAnimator;
    private bool swordEnabled;
    private InventoryManager inventoryManager;

    private RollHandler rollHandler;
    private WalkHandler walkHandler;
    private CombatHandler combatHandler;
    private JumpHandler jumpHandler;

    public void Start()
    {
        this.holdsSword = false;
        this.swordEnabled = true;
        this.combatAnimator = this.player.GetComponentInChildren<CombatAnimator>();
        this.inventoryManager = this.player.GetComponent<InventoryManager>();

        this.rollHandler = this.player.GetComponent<RollHandler>();
        this.walkHandler = this.player.GetComponent<WalkHandler>();
        this.combatHandler = this.player.GetComponent<CombatHandler>();
        this.jumpHandler = this.player.GetComponent<JumpHandler>();
    }

    public void SwitchSword()
    {
        if (this.swordEnabled && this.inventoryManager.HasSword())
        {
            if (!this.holdsSword)
            {
                this.combatAnimator.TriggerDrawSword();
                this.DrawSword();
                if(this.rollHandler != null) this.rollHandler.DisableRolling();
                if(this.walkHandler != null) this.walkHandler.DisableSprinting();
            }
            else
            {
                this.combatAnimator.TriggerSheathSword();
                this.SheathSword();
                if(this.rollHandler != null) this.rollHandler.EnableRolling();
                if(this.walkHandler != null) this.walkHandler.EnableSprinting();
            }
        }
    }

    public void DrawSword()
    {
        this.component.Activate();
        this.DisableOther();
        this.Invoke(nameof(EnableOther), 1.5f);
        this.holdsSword = true;
        GameObject swordObject = Instantiate(this.inventoryManager.GetSword());
        swordObject.transform.SetParent(this.gameObject.transform);
        swordObject.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
        StartCoroutine(SliderRoutine(swordObject));
    }

    private IEnumerator SliderRoutine(GameObject swordObject)
    {
        yield return new WaitForEndOfFrame();
        Debug.Log(swordObject.GetComponent<SwordContainer>().properties);
        this.speedSlider.value = swordObject.GetComponent<SwordContainer>().properties.speed;
        this.attackSlider.value = swordObject.GetComponent<SwordContainer>().properties.weaponValue;
        this.armorSlider.value = swordObject.GetComponent<SwordContainer>().properties.armourValue;
        Accessor.currentSword = swordObject.GetComponent<SwordContainer>();
    }

    public void SheathSword()
    {
        this.DisableOther();
        this.Invoke(nameof(EnableOther), 1.667f);
        this.RemoveSword();
    }

    private void RemoveSword()
    {
        foreach (Transform child in this.gameObject.transform)
        {
            if (child.gameObject.CompareTag("Sword"))
            {
                Destroy(child.gameObject);
                this.holdsSword = false;
            }
        }
        this.component.Deactivate();
        this.speedSlider.value = this.attackSlider.value = this.armorSlider.value = 0;
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

    public void DisableSword()
    {
        this.RemoveSword();
        this.swordEnabled = false;
    }

    public void EnableSword()
    {
        this.swordEnabled = true;
    }

}
