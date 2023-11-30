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
    
    // Reference to the Prefab. Drag a Prefab into this field in the Inspector.
    public GameObject resourcePrefab;

    // variables for ML agents
    public bool isTrainingMode;
    
    void Start()
    {
        float[,] noiseMap = WorldGenerator.GenerateNoiseMap(MainMenuController.inputWidth, MainMenuController.inputHeight, MainMenuController.inputSeed, MainMenuController.inputScale, MainMenuController.inputOctaves, MainMenuController.inputPersistence, MainMenuController.inputLacunarity, offset);
        WorldGenerator.PlaceTiles(MainMenuController.inputWidth, MainMenuController.inputHeight, noiseMap, grid, tilemaps, tileList);
        ObjectSpawner objSpawner = new ObjectSpawner();
        objSpawner.PlaceResources(grid, resourcePrefab);

    }

    void Update()
    {
        
    }

}
