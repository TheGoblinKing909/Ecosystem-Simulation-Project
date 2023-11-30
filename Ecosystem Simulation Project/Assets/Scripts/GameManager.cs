using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GameManager : MonoBehaviour
{

    // variables for noise map generation
    public int width;
    public int height;
    public int seed;
    public float scale;
    public int octaves;
    public float persistence;
    public float lacunarity;
    public Vector2 offset;

    // variables for tile placement
    public Grid grid = null;
    public List<Tilemap> tilemaps = new List<Tilemap>();
    public List<Tile> tileList = new List<Tile>();

    // reference to ObjectSpawner script
    public ObjectSpawner resourceSpawner;

    // sequential order MUST match 'resourceAllowedTilemaps' in ObjectSpawner script
    public List<GameObject> resourcePrefabs;

    // suggested default value 0.01 - 0.03
    public float resourceDensity;

    // both used for continual respawning
    public float resourceMax;
    public int totalResourceCount;

    // reference to ObjectSpawner script
    public ObjectSpawner entitySpawner;

    // sequential order MUST match 'entityAllowedTilemaps' in ObjectSpawner script
    public List<GameObject> entityPrefabs;

    // suggested default value 0.01 - 0.02
    public float entityDensity;

    // both used for continual respawning
    public float entityMax;
    public int totalEntityCount;

    // variables for ML agents
    public bool isTrainingMode;

    public int frameCount = 0;

    void Start()
    {
        float[,] noiseMap = WorldGenerator.GenerateNoiseMap(width, height, seed, scale, octaves, persistence, lacunarity, offset);
        WorldGenerator.PlaceTiles(width, height, noiseMap, grid, tilemaps, tileList);
        resourceMax = resourceSpawner.PlaceResources();
        entityMax = entitySpawner.PlaceEntities();
    }

    void Update()
    {
        if ( ++frameCount > 2 ) {
            frameCount = 0;
            int spawnAmount;
            totalResourceCount = resourceSpawner.GetChildCount();
            totalEntityCount = entitySpawner.GetChildCount();
            if ( totalResourceCount < resourceMax ) {
                spawnAmount = 1;
                resourceSpawner.SpawnResources(spawnAmount);
            }
            if ( totalEntityCount < entityMax ) {
                spawnAmount = 1;
                entitySpawner.SpawnEntities(spawnAmount);
            }
        }
    }

}