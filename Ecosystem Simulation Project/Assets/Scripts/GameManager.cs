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

    // sequential order MUST match 'resourceAllowedTilemaps' in objSpawner
    public List<GameObject> resourcePrefabs;

    // suggested default value 0.01 - 0.025
    public float resourceDensity;

    // reference to ObjectSpawner script
    public ObjectSpawner entitySpawner;

    // sequential order MUST match 'resourceAllowedTilemaps' in objSpawner
    public List<GameObject> entityPrefabs;

    // suggested default value 0.01 - 0.025
    public float entityDensity;

    // variables for ML agents
    public bool isTrainingMode;
    
    void Start()
    {
        float[,] noiseMap = WorldGenerator.GenerateNoiseMap(width, height, seed, scale, octaves, persistence, lacunarity, offset);
        WorldGenerator.PlaceTiles(width, height, noiseMap, grid, tilemaps, tileList);
        resourceSpawner.PlaceResources();
        resourceSpawner.PlaceEntities();
        //entitySpawner.PlaceEntities();
    }

    void Update()
    {
        
    }

}