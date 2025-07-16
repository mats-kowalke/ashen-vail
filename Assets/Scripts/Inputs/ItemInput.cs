using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(InventoryManager))]
public class ItemInput : MonoBehaviour
{

    private InventoryManager inventoryManager;
    private bool dropFlag;
    private bool collectFlag;

    public void Start()
    {
        this.inventoryManager = this.GetComponent<InventoryManager>();
    }

    public void Update()
    {
        if (this.dropFlag)
        {
            this.dropFlag = false;
            this.inventoryManager.DropSword();
        }
        if (this.collectFlag)
        {
            this.collectFlag = false;
            this.inventoryManager.Collect();
        }
    }

    public void OnDrop(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            this.dropFlag = true;
        }
    }

    public void OnCollect(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            this.collectFlag = true;
        }
    }

}