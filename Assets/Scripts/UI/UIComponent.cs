using UnityEngine;
using UnityEngine.UI;

public class UIComponent : MonoBehaviour
{
    public Image background;
    public Color activeColor;
    public Color inactiveColor;

    public void Activate()
    {
        this.background.color = this.activeColor;
    }

    public void Deactivate()
    {
        this.background.color = this.inactiveColor;
    }
}