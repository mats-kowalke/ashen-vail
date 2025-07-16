using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(RotationHandler))]
[RequireComponent(typeof(JumpHandler))]
[RequireComponent(typeof(WalkHandler))]
[RequireComponent(typeof(RollHandler))]
public class MovementInput : MonoBehaviour
{
    public float mouseXSensitivity;
    private Vector3 movement;
    private float rotationX;
    private bool sprinting;
    private bool jumpFlag;
    private bool rollFlag;
    private RotationHandler rotationHandler;
    private JumpHandler jumpHandler;
    private WalkHandler walkHandler;
    private RollHandler rollHandler;

    public void Start()
    {
        this.rotationHandler = this.GetComponent<RotationHandler>();
        this.jumpHandler = this.GetComponent<JumpHandler>();
        this.walkHandler = this.GetComponent<WalkHandler>();
        this.rollHandler = this.GetComponent<RollHandler>();
    }

    public void Update()
    {
        this.rotationHandler.UpdateRotation(this.rotationX);

        if (this.jumpFlag) { this.jumpHandler.DoJump(); this.jumpFlag = false; }

        this.walkHandler.UpdateWalk(this.movement);
        this.walkHandler.UpdateSprint(this.sprinting);

        if (this.rollFlag) { this.rollHandler.DoRoll(); this.rollFlag = false; }

    }

    public void OnMovement(InputAction.CallbackContext context)
    {
        Vector2 extractedVector = context.ReadValue<Vector2>();
        this.movement = new Vector3(extractedVector.x, 0, extractedVector.y);
    }

    public void OnMouseX(InputAction.CallbackContext context)
    {
        float extractedMovement = context.ReadValue<float>();
        this.rotationX = extractedMovement * this.mouseXSensitivity;
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            this.jumpFlag = true;
        }
    }

    public void OnSprint(InputAction.CallbackContext context)
    {
        if (context.performed && this.movement.z > 0)
        {
            this.sprinting = true;
        }
        if (context.canceled)
        {
            this.sprinting = false;
        }
    }

    public void OnRoll(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            this.rollFlag = true;
        }
    }

}