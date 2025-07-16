using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.Animations;

public class TerrainGenerator : MonoBehaviour
{
    [Header("Terrain Settings")]
    public int depth = 20;
    public int width = 256;
    public int height = 256;
    public float scale = 20f;
    
    [Header("Island Shape")]
    [Range(0.3f, 0.8f)]
    public float islandRadius = 0.4f;
    [Range(0.01f, 0.3f)]
    public float coastSoftness = 0.2f;

    [Header("Height Settings")]
    [Range(0.2f, 1.0f)]
    public float maxHeight = 0.8f;
    [Range(0.05f, 1.2f)]
    public float coastHeight = 0.1f;

    [Header("Mountain Settings")]
    [Range(1f, 8f)]
    public float mountainScale = 3f;
    [Range(0.3f, 2f)]
    public float mountainHeight = 1.2f;
    
    [Header("Animation")]
    public bool animateTerrain = true;
    public float animationSpeed = 2.5f;

    [Header("Trees")]
    public GameObject[] trees;
    public float density = 0.5f;

    public float offsetX;
    public float offsetY;

    private PlayerSpawner playerSpawner;
    private BonfireSpawner bonfireSpawner;
    private GearSpawner gearSpawner;
    public EnemySpawner enemySpawner;

    void Start()
    {
        offsetX = Random.Range(0f, 9999f);
        offsetY = Random.Range(0f, 9999f);

        Terrain terrain = GetComponent<Terrain>();
        if (terrain != null)
        {
            terrain.terrainData = GenerateTerrain(terrain.terrainData);
        }
        PlacementEditorWindow placementEditorWindow = new PlacementEditorWindow();
        placementEditorWindow.minHeight = 0.1f;
        placementEditorWindow.maxHeight = 0.8f;
        int width = (int)Terrain.activeTerrain.terrainData.size.x;
        int height = (int)Terrain.activeTerrain.terrainData.size.y;
        float scale = 5;
        placementEditorWindow.PlaceObjects(
            terrain,
            Noise.GetNoiseMap(width, height, scale),
            this.density,
            this.trees
            );
        this.playerSpawner = this.GetComponent<PlayerSpawner>();
        this.playerSpawner.Spawn(terrain);
        this.bonfireSpawner = this.GetComponent<BonfireSpawner>();
        this.bonfireSpawner.Spawn(terrain);
        this.gearSpawner = this.GetComponent<GearSpawner>();
        this.gearSpawner.Spawn(terrain);
        this.enemySpawner.SetTerrain(terrain);
        NavMeshSurface surface = this.GetComponent<NavMeshSurface>();
        surface.BuildNavMesh();
    }

    void Update()
    {
        if (!animateTerrain) return;
        
        Terrain terrain = GetComponent<Terrain>();
        if (terrain != null)
        {
            terrain.terrainData = GenerateTerrain(terrain.terrainData);
            
            offsetX += Time.deltaTime * animationSpeed;
            offsetY += Time.deltaTime * animationSpeed;
        }
    }

    float GetIslandMask(float normalizedX, float normalizedY)
    {
        // Convert to centred coordinates (-0.5 to 0.5)
        float x = normalizedX - 0.5f;
        float y = normalizedY - 0.5f;
        
        float distanceFromCentre = Mathf.Sqrt(x * x + y * y);

        if (distanceFromCentre > islandRadius)
        {
            return 0f;
        }
        
        float transitionStart = islandRadius - coastSoftness;
        
        if (distanceFromCentre <= transitionStart)
        {
            return 1f;
        }
        
        float t = (distanceFromCentre - transitionStart) / coastSoftness;
        return 1f - Mathf.SmoothStep(0f, 1f, t);
    }

    TerrainData GenerateTerrain(TerrainData terrainData)
    {
        terrainData.heightmapResolution = width + 1;
        terrainData.size = new Vector3(width, depth, height);
        terrainData.SetHeights(0, 0, GenerateHeights());
        return terrainData;
    }

    float[,] GenerateHeights()
    {
        float[,] heights = new float[width, height];
        
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                heights[x, y] = CalculateHeight(x, y);
            }
        }

        return heights;
    }
    float CalculateHeight(int x, int y)
    {
        // Get island mask for this position
        float normalizedX = (float)x / width;
        float normalizedY = (float)y / height;
        
        float islandMask = GetIslandMask(normalizedX, normalizedY);
        if (islandMask < 0.02f) 
            return 0f;
        
        float xCoord = (float)x / width * scale + offsetX;
        float yCoord = (float)y / height * scale + offsetY;

        float mountainNoise = 0f;
        mountainNoise += Mathf.PerlinNoise(xCoord / mountainScale, yCoord / mountainScale) * 1.0f;
        mountainNoise += Mathf.PerlinNoise(xCoord / (mountainScale * 0.5f), yCoord / (mountainScale * 0.5f)) * 0.5f;
        mountainNoise += Mathf.PerlinNoise(xCoord / (mountainScale * 0.25f), yCoord / (mountainScale * 0.25f)) * 0.25f;
        mountainNoise /= 1.75f;
        mountainNoise = Mathf.Clamp01(mountainNoise);

        float distanceFromCentre = Vector2.Distance(new Vector2(normalizedX, normalizedY), new Vector2(0.5f, 0.5f));
        float normalizedDistance = Mathf.Clamp01(distanceFromCentre / islandRadius);

        float ringHeightProfile = Mathf.Sin(normalizedDistance * Mathf.PI);
        ringHeightProfile = Mathf.Pow(ringHeightProfile, 1.5f);

        float finalHeight = (coastHeight + mountainNoise * mountainHeight * ringHeightProfile) * maxHeight;

        return Mathf.Clamp01(finalHeight);
    }

    [ContextMenu("Generate New Island Shape")]
    public void RegenerateIslandShape()
    {
        offsetX = Random.Range(0f, 9999f);
        offsetY = Random.Range(0f, 9999f);

        Terrain terrain = GetComponent<Terrain>();
        if (terrain != null)
        {
            terrain.terrainData = GenerateTerrain(terrain.terrainData);
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;

        Vector3 terrainCentre = transform.position + new Vector3(width / 2f, 0, height / 2f);

        float worldRadius = islandRadius * Mathf.Min(width, height);

        DrawCircleGizmo(terrainCentre, worldRadius, 100);
    }

    void DrawCircleGizmo(Vector3 centre, float radius, int segments)
    {
        Vector3 prevPoint = centre + new Vector3(Mathf.Cos(0), 0, Mathf.Sin(0)) * radius;
        for (int i = 1; i < segments; i++)
        {
            float angle = i * 2 * Mathf.PI / segments;
            Vector3 newPoint = centre + new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * radius;
            Gizmos.DrawLine(prevPoint, newPoint);
            prevPoint = newPoint;
        }
    }
}
