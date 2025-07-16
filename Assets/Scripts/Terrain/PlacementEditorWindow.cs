using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PlacementEditorWindow : MonoBehaviour
{
    [HideInInspector]
    public Texture2D noiseMapTexture;
    [HideInInspector]
    public float density = 0.5f;
    [HideInInspector]
    public GameObject[] prefabs;

    private int placementResolution = 64; // Lower resolution for performance
    public float minHeight = 0.1f; // Minimum terrain height to place objects
    public float maxHeight = 0.2f; // Maximum terrain height to place objects

    [Header("Spacing Settings")] private float minSpacing = 2f;
    private float maxSpacing = 8f;
    private bool useRandomSpacing = true;

    [Header("Steepness Settings")] private float maxSteepness = 30f;
    private bool checkSteepness = true;

    public void PlaceObjects(Terrain terrain, Texture2D texture, float density, GameObject[] prefabs)
    {
        // Clear existing objects first
        ClearPlacedObjects();
        
        // Create parent object
        Transform parent = new GameObject("PlacedObjects").transform;

        TerrainData terrainData = terrain.terrainData;
        Vector3 terrainSize = terrainData.size;
        
        // Get the actual heightmap resolution
        int heightmapWidth = terrainData.heightmapResolution;
        int heightmapHeight = terrainData.heightmapResolution;
        
        // List to track placed object positions for spacing checks
        List<Vector3> placedPositions = new List<Vector3>();

        if (useRandomSpacing)
        {
            // Use random placement with spacing control
            PlaceObjectsWithRandomSpacing(terrain, texture, density, prefabs, parent, placedPositions);
        }
        else
        {
            // Use grid-based placement
            PlaceObjectsGridBased(terrain, texture, density, prefabs, parent, heightmapWidth, heightmapHeight,
                terrainSize);
        }
        
        Debug.Log($"Placed {placedPositions.Count} objects under parent: {parent.name}");
    }

    private void PlaceObjectsWithRandomSpacing(Terrain terrain, Texture2D texture, float density, GameObject[] prefabs, Transform parent, List<Vector3> placedPositions)
    {
        TerrainData terrainData = terrain.terrainData;
        Vector3 terrainSize = terrainData.size;
        
        // Calculate number of attempts based on density and terrain size
        int maxAttempts = Mathf.RoundToInt(density * terrainSize.x * terrainSize.z / (minSpacing * minSpacing));

        for (int attempt = 0; attempt < maxAttempts; attempt++)
        {
            // Generate random position within terrain bounds
            float randomX = Random.Range(0f, 1f);
            float randomZ = Random.Range(0f, 1f);
            
            // Convert to wrold position
            Vector3 worldPos = new Vector3(
                randomX * terrainSize.x,
                0f,
                randomZ * terrainSize.z
            );
            worldPos += terrain.transform.position;
            
            // Get terrain height at this position
            float terrainHeight = terrainData.GetInterpolatedHeight(randomX, randomZ);
            float normalizedHeight = terrainHeight / terrainSize.y;
            worldPos.y = terrainHeight + terrain.transform.position.y;
            
            // Check height constraints
            if (normalizedHeight < minHeight || normalizedHeight > maxHeight)
                continue;
            
            // Check steepness if enabled
            if (checkSteepness && !IsPositionSuitableForPlacement(terrainData, randomX, randomZ, terrainSize)) 
                continue;
            
            // Check spacing with existing objects
            if (!IsValidSpacing(worldPos, placedPositions)) 
                continue;
            
            // Sample noise map
            int noiseX = Mathf.FloorToInt(randomX * (texture.width - 1));
            int noiseZ = Mathf.FloorToInt(randomZ * (texture.height - 1));
            float noiseValue = texture.GetPixel(noiseX, noiseZ).g;
            
            // Check if we should place an object here based on noise
            if (noiseValue > (1f - density))
            {
                GameObject selectedPrefab = prefabs[Random.Range(0, prefabs.Length)];
                PlaceObject(selectedPrefab, worldPos, parent);
                placedPositions.Add(worldPos);
            }
        }
    }

    private void PlaceObjectsGridBased(Terrain terrain, Texture2D texture, float density, GameObject[] prefabs,
        Transform parent, int heightmapWidth, int heightmapHeight, Vector3 terrainSize)
    {
        TerrainData terrainData = terrain.terrainData;
        
        int sampleStep = Mathf.Max(1, heightmapWidth / placementResolution);

        for (int x = 0; x < heightmapWidth; x += sampleStep)
        {
            for (int z = 0; z < heightmapHeight; z += sampleStep)
            {
                float normalizedX = (float)x / (heightmapWidth - 1);
                float normalizedZ = (float)z / (heightmapHeight - 1);

                float terrainHeight = terrainData.GetInterpolatedHeight(normalizedX, normalizedZ);
                float normalizedHeight = terrainHeight / terrainSize.y;

                if (normalizedHeight < minHeight || normalizedHeight > maxHeight)
                    continue;

                if (checkSteepness && !IsPositionSuitableForPlacement(terrainData, normalizedX, normalizedZ, terrainSize)) 
                    continue;
                
                int noiseX = Mathf.FloorToInt(normalizedX * (texture.width - 1));
                int noiseZ = Mathf.FloorToInt(normalizedZ * (texture.height - 1));
                float noiseValue = texture.GetPixel(noiseX, noiseZ).g;

                if (noiseValue > (1f - density))
                {
                    Vector3 worldPos = new Vector3(
                        normalizedX * terrainSize.x + Random.Range(-0.5f, 0.5f),
                        terrainHeight,
                        normalizedZ * terrainSize.z + Random.Range(-0.5f, 0.5f)
                    );

                    worldPos += terrain.transform.position;
                    
                    GameObject selectedPrefab = prefabs[Random.Range(0, prefabs.Length)];
                    PlaceObject(selectedPrefab, worldPos, parent);
                }
            }
        }
    }

    private bool IsValidSpacing(Vector3 newPosition, List<Vector3> existingPositions)
    {
        float spacingDistance = Random.Range(minSpacing, maxSpacing);

        foreach (Vector3 existingPos in existingPositions)
        {
            float distance = Vector3.Distance(newPosition, existingPos);
            if (distance < spacingDistance)
                return false;
        }

        return true;
    }

    private bool IsPositionSuitableForPlacement(TerrainData terrainData, float normalizedX, float normalizedZ,
        Vector3 terrainSize)
    {
        float sampleDistance = 1f / terrainData.heightmapResolution;
        
        float rightHeight = terrainData.GetInterpolatedHeight(Mathf.Clamp01(normalizedX + sampleDistance), normalizedZ);
        float leftHeight = terrainData.GetInterpolatedHeight(Mathf.Clamp01(normalizedX - sampleDistance), normalizedZ);
        float upHeight = terrainData.GetInterpolatedHeight(normalizedX, Mathf.Clamp01(normalizedZ + sampleDistance));
        float downHeight = terrainData.GetInterpolatedHeight(normalizedX, Mathf.Clamp01(normalizedZ - sampleDistance));
        
        float worldSampleDistance = sampleDistance * terrainSize.x; // Assuming square terrain

        float gradientX = (rightHeight - leftHeight) / (2f * worldSampleDistance);
        float gradientZ = (upHeight - downHeight) / (2f * worldSampleDistance);

        float slopeAngle = Mathf.Atan(Mathf.Sqrt(gradientX * gradientX + gradientZ * gradientZ)) * Mathf.Rad2Deg;

        return slopeAngle <= maxSteepness;
    }

    private void PlaceObject(GameObject prefab, Vector3 position, Transform parent)
    {
        // Create random rotation
        Quaternion rotation = Quaternion.Euler(0, Random.Range(0f, 360f), 0);
        GameObject go = Instantiate(prefab);
        go.transform.position = position;
        go.transform.rotation = rotation;
        go.transform.SetParent(parent);
        
        // Add some random scale variation
        float scaleVariation = Random.Range(0.8f, 1.2f);
        go.transform.localScale *= scaleVariation;
    }
    
    private void ClearPlacedObjects()
    {
        GameObject existingParent = GameObject.Find("PlacedObjects");
        if (existingParent != null)
        {
            DestroyImmediate(existingParent);
        }
    }
}
