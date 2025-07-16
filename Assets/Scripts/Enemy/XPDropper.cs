using UnityEngine;
public class XPDropper : MonoBehaviour
{
    public int XPAmount;
    public GameObject collectiblePrefab;
    public GameObject containerPrefab;
    public GameObject swordPrefab;

    public void OnDisable()
    {

        XPContainer container = this.containerPrefab.GetComponent<XPContainer>();
        container.XPAmount = this.XPAmount;

        GameObject instance = Instantiate(collectiblePrefab, this.transform.position + new Vector3(0, 3, 0), Quaternion.identity);
        Collectible collectible = instance.GetComponent<Collectible>();
        collectible.SetContent(containerPrefab);

        SwordContainer swordContainer = this.swordPrefab.GetComponent<SwordContainer>();
        WeaponQualityGenerator generator = new WeaponQualityGenerator();
        generator.Start();
        swordContainer.properties = generator.GenerateWeapon(Accessor.xPHandler.currentXP);

        GameObject swordInstance = Instantiate(collectiblePrefab, this.transform.position + new Vector3(0, 3, 0), Quaternion.identity);
        Collectible swordCollectible = swordInstance.GetComponent<Collectible>();
        swordCollectible.SetContent(swordPrefab);

        Accessor.enemySpawner.DecreaseCurrentAmount();

    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Sword") && Accessor.combatHandler.isAttacking)
        {
            Debug.Log("Taken damage");
            Debug.Log(Mathf.Sqrt(Accessor.currentSword.properties.weaponValue) * 5);
            Destroy(this.gameObject);
        }
    }

}