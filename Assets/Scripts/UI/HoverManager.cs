using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class HoverManager : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{

    public TMP_Text toolTipUI;
    public String toolTip;

    public void OnPointerEnter(PointerEventData eventData)
    {
        this.toolTipUI.text = this.toolTip;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        this.toolTipUI.text = "";
    }
}