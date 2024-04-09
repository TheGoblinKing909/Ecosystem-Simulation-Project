using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
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

            Process tensorboardProcess = new Process();
            tensorboardProcess.StartInfo.UseShellExecute = false;
            tensorboardProcess.StartInfo.RedirectStandardInput = true;
            tensorboardProcess.StartInfo.FileName = "cmd.exe";
            tensorboardProcess.StartInfo.Arguments = @"/K ..\anaconda3\Scripts\activate.bat ..\anaconda3";
            tensorboardProcess.Start();

            tensorboardProcess.StandardInput.WriteLine("conda activate build-env");
            tensorboardProcess.StandardInput.WriteLine("tensorboard --logdir=results");

            Application.OpenURL("http://localhost:6006/");
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