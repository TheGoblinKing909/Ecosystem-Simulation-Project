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

    // world generation parameters
    public int inputWidth, inputHeight, inputSeed, inputOctaves;
    public float inputScale, inputPersistence, inputLacunarity, inputResDensity, inputEntDensity;

    void Start()
    {
        if (Application.isEditor)
        {
            inputWidth = MainMenuController.inputWidth;
            inputHeight = MainMenuController.inputHeight;
            inputSeed = MainMenuController.inputSeed;
            inputOctaves = MainMenuController.inputOctaves;
            inputScale = MainMenuController.inputScale;
            inputPersistence = MainMenuController.inputPersistence;
            inputLacunarity = MainMenuController.inputLacunarity;
            inputResDensity = MainMenuController.inputResDensity;
            inputEntDensity = MainMenuController.inputEntDensity;
        }
        else
        {
            string[] args = System.Environment.GetCommandLineArgs();
            int argLen = args.Length;
            inputWidth = int.Parse(args[argLen-9]);
            inputHeight = int.Parse(args[argLen-8]);
            inputSeed = int.Parse(args[argLen-7]);
            inputOctaves = int.Parse(args[argLen-6]);
            inputScale = float.Parse(args[argLen-5]);
            inputPersistence = float.Parse(args[argLen-4]);
            inputLacunarity = float.Parse(args[argLen-3]);
            inputResDensity = float.Parse(args[argLen-2]);
            inputEntDensity = float.Parse(args[argLen-1]);
        }
        float[,] noiseMap = WorldGenerator.GenerateNoiseMap(inputWidth, inputHeight, inputSeed, inputScale, inputOctaves, inputPersistence, inputLacunarity, offset);
        WorldGenerator.PlaceTiles(inputWidth, inputHeight, noiseMap, grid, tilemaps, tileList);
        resourceSpawner.OnInstantiate();
        resourceMax = resourceSpawner.PlaceResources();
        entitySpawner.OnInstantiate();
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