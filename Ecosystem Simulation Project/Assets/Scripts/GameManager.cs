using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GameManager : MonoBehaviour
{
    public Vector2 offset;

    // variables for tile placement
    public Grid grid = null;
    public List<Tilemap> tilemaps = new List<Tilemap>();
    public List<Tile> tileList = new List<Tile>();

    // reference to ObjectSpawner script
    public ObjectSpawner resourceSpawner;

    // sequential order MUST match 'resourceAllowedTilemaps' in ObjectSpawner script
    public List<GameObject> resourcePrefabs;

    // suggested default value 0.01 - 0.001
    public float resourceDensity;

    // both used for continual respawning
    public float resourceMax;
    public int totalResourceCount;

    // reference to ObjectSpawner script
    public ObjectSpawner entitySpawner;

    // sequential order MUST match 'entityAllowedTilemaps' in ObjectSpawner script
    public List<GameObject> entityPrefabs;

    // suggested default value 0.01 - 0.001
    public float entityDensity;

    // both used for continual respawning
    public float entityMax;
    public int totalEntityCount;

    public TimeManager timeManager;

    public WeatherManager weatherManager;

    public ClockManager clockManager;

    // variables for ML agents
    public bool isTrainingMode;

    public int frameCount = 0;

    void Start()
    {
        float[,] noiseMap = WorldGenerator.GenerateNoiseMap(MainMenuController.inputWidth, MainMenuController.inputHeight, MainMenuController.inputSeed, MainMenuController.inputScale, MainMenuController.inputOctaves, MainMenuController.inputPersistence, MainMenuController.inputLacunarity, offset);
        WorldGenerator.PlaceTiles(MainMenuController.inputWidth, MainMenuController.inputHeight, noiseMap, grid, tilemaps, tileList);
        resourceMax = resourceSpawner.PlaceResources();
        entityMax = entitySpawner.PlaceEntities();
    }

    void Update()
    {
        if (++frameCount > 20) {
            frameCount = 0;

            totalResourceCount = resourceSpawner.GetChildCount();
            totalEntityCount = entitySpawner.GetChildCount();

            int resourceSpawnAmount = CalculateSpawnAmount(resourceMax, totalResourceCount);
            int entitySpawnAmount = CalculateSpawnAmount(entityMax, totalEntityCount);

            if (resourceSpawnAmount > 0) {
                resourceSpawner.SpawnResources(resourceSpawnAmount);
            }
            if (entitySpawnAmount > 0) {
                entitySpawner.SpawnEntities(entitySpawnAmount);
            }
        }
    }

    int CalculateSpawnAmount(float max, int current)
    {
        float deficit = max - current;
        float proportion = 0.1f; // Adjust this value as needed

        // Ensure at least 1 is spawned if there's any deficit at all,
        // but cap it with a calculated proportion of the deficit to avoid overpopulation
        int spawnAmount = Mathf.Clamp(Mathf.CeilToInt(deficit * proportion), 0, Mathf.Max(1, Mathf.FloorToInt(deficit)));

        return spawnAmount;
    }

}