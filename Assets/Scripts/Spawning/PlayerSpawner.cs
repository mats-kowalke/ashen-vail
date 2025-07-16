using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    public GameObject player;
    public Vector2Int spawnPosition;

    public void Spawn(Terrain terrain)
    {
        float terrainHeight = terrain.terrainData.GetHeight(spawnPosition.x, spawnPosition.y) + 1;
        this.player.transform.position = new Vector3(spawnPosition.x, terrainHeight, spawnPosition.y);
        this.player.GetComponent<Health>().SetRespawnPoint();
    }

}
