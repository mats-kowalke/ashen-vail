using UnityEngine;

public class RotationHandler : MonoBehaviour
{

    private bool canRotate;

    public void Start()
    {
        this.canRotate = true;
    }

    public void UpdateRotation(float input)
    {
        if (this.canRotate)
        {
            this.transform.Rotate(0, input, 0);
        }
    }

    public void DisableRotation()
    {
        this.canRotate = false;
    }

    public void EnableRotation()
    {
        this.canRotate = true;
    }
}