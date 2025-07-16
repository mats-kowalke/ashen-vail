using UnityEngine;

public class GearSpawner : MonoBehaviour
{
    public GameObject collectiblePrefab;
    public GameObject swordPrefab;

    public void Spawn(Terrain terrain)
    {
        WeaponQualityGenerator generator = new WeaponQualityGenerator();
        generator.Start();
        WeaponProperties weaponProperties = generator.GenerateWeapon(0);
        Debug.Log("Generated: " + weaponProperties);
        SwordContainer swordContainer = swordPrefab.GetComponent<SwordContainer>();
        swordContainer.UpdateQuality(weaponProperties);

        GameObject collectibleInstance = Instantiate(collectiblePrefab);
        Collectible collectible = collectibleInstance.GetComponent<Collectible>();
        collectible.SetContent(swordPrefab);

        Vector2Int playerPosition = this.GetComponent<PlayerSpawner>().spawnPosition + new Vector2Int(5, 0);
        float terrainHeight = terrain.terrainData.GetHeight(playerPosition.x, playerPosition.y) + 10;
        collectibleInstance.transform.position = new Vector3(playerPosition.x, terrainHeight, playerPosition.y);

    }
}
