using UnityEngine;

public class BonfireSpawner : MonoBehaviour
{
    public GameObject bonfirePrefab;
    public Vector2Int[] positions;

    public void Spawn(Terrain terrain)
    {
        foreach (Vector2Int position in positions)
        {
            GameObject instance = Instantiate(bonfirePrefab);
            float terrainHeight = terrain.terrainData.GetHeight(position.x, position.y) + 2;
            instance.transform.position = new Vector3(position.x, terrainHeight, position.y);
        }
    }
}
