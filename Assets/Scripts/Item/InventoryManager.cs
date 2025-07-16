using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public GameObject collectablePrefab;
    public SwordHolder swordHolder;
    public UIComponent pickUpComponent;

    private GameObject swordInstance;
    private GameObject possibleCollectible;

    public void Start()
    {
        this.pickUpComponent.Deactivate();
    }

    public void Collect()
    {
        if (this.possibleCollectible != null)
        {
            if (possibleCollectible.TryGetComponent<Collectible>(out var collectible))
            {
                Destroy(this.possibleCollectible);
                this.possibleCollectible = null;
                this.pickUpComponent.Deactivate();
                if (this.swordInstance == null)
                {
                    this.swordInstance = collectible.content;
                    this.swordHolder.EnableSword();
                }
                else
                {
                    DropSword();
                    this.swordInstance = collectible.content;
                    this.swordHolder.EnableSword();
                }
            }
        }
    }

    public void DropSword()
    {
        if (this.swordInstance != null)
        {
            GameObject collectible = Instantiate(collectablePrefab);
            Collectible container = collectible.GetComponent<Collectible>();
            container.SetContent(this.swordInstance);
            container.Drop(this.transform.position + this.transform.forward, this.transform.forward);
            this.swordInstance = null;
            this.swordHolder.DisableSword();
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Collectible"))
        {
            if (other.TryGetComponent<Collectible>(out var collectible) && collectible.content.CompareTag("Sword"))
            {
                this.pickUpComponent.Activate();
                this.possibleCollectible = other.gameObject;
            }
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Collectible"))
        {
            if (other.TryGetComponent<Collectible>(out var collectible) && collectible.content.CompareTag("Sword"))
            {
                this.pickUpComponent.Deactivate();
                this.possibleCollectible = null;
            }
        }
    }

    public bool HasSword()
    {
        return this.swordInstance != null;
    }

    public GameObject GetSword()
    {
        return this.swordInstance;
    }

}