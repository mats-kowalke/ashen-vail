using System;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{

    public GameObject enemyPrefab;
    public float minRadius;
    public float maxRadius;
    public float maxAmount;

    private int currentAmount;
    private Terrain terrain;

    public void Start()
    {
        this.currentAmount = 0;
        Accessor.enemySpawner = this;
    }

    public void Update()
    {
        if (this.terrain != null && this.currentAmount < this.maxAmount)
        {
            SpawnAtRandomRadius();
        }
    }

    private void SpawnAtRandomRadius()
    {
        float radius = UnityEngine.Random.Range(this.minRadius, this.maxRadius);
        float angle = UnityEngine.Random.Range(0, 2 * Mathf.PI);

        int xPos = Mathf.FloorToInt(Mathf.Cos(angle) * radius + this.transform.position.x);
        int zPos = Mathf.FloorToInt(Mathf.Sin(angle) * radius + this.transform.position.z);

        float height = this.terrain.terrainData.GetHeight(xPos, zPos) + 2;
        if (height > 0.5f)
        {
            Instantiate(enemyPrefab, new Vector3(xPos, height, zPos), Quaternion.identity);
            Debug.Log("Instantiated at: " + new Vector3(xPos, height, zPos));

            this.currentAmount++;
        }

    }

    public void SetTerrain(Terrain terrain)
    {
        this.terrain = terrain;
        Debug.Log("Terrain set");
    }

    public void DecreaseCurrentAmount()
    {
        this.currentAmount--;
    }


}
