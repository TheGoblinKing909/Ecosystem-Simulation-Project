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
    
    // variables for ML agents
    public bool isTrainingMode;

    void Start()
    {
        float[,] noiseMap = WorldGenerator.GenerateNoiseMap(width, height, seed, scale, octaves, persistence, lacunarity, offset);
        WorldGenerator.PlaceTiles(width, height, noiseMap, grid, tilemaps, tileList);
    }

    void Update()
    {
        
    }

}
