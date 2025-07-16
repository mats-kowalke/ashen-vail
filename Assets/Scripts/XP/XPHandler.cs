using TMPro;
using UnityEngine;

public class XPHandler : MonoBehaviour
{
    public int currentXP;
    public TMP_Text text;

    public static float currentXPLvl;

    public void Start()
    {
        this.currentXP = 0;
        currentXPLvl = 0;

        Accessor.xPHandler = this;
    }

    public void ResetXP()
    {
        this.currentXP = 0;
        currentXPLvl = 0;
        this.text.text = currentXP.ToString();
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Collectible"))
        {
            if (other.TryGetComponent<Collectible>(out var collectible) && collectible.content.CompareTag("XP"))
            {
                this.currentXP += collectible.content.GetComponent<XPContainer>().XPAmount;
                currentXPLvl = this.currentXP;
                this.text.text = currentXP.ToString();
                Destroy(other.gameObject);
            }
        }
    }
}