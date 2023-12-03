using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public static class WorldGenerator
{
    public static float[,] GenerateNoiseMap(int width, int height, int seed, float scale, int octaves, float persistence, float lacunarity, Vector2 offset)
    {
        float[,] noiseMap = new float[width, height];
        System.Random rand = new System.Random(seed);
        Vector2[] octavesOffset = new Vector2[octaves];

        for (int i = 0; i < octaves; i++)
        {
            float xOffset = rand.Next(-100000, 100000) + offset.x;
            float yOffset = rand.Next(-100000, 100000) + offset.y;
            octavesOffset[i] = new Vector2(xOffset / width, yOffset / height);
        }

        if (scale < 0)
        {
            scale = 0.0001f;
        }

        float halfWidth = width / 2f;
        float halfHeight = height / 2f;

        for(int y = 0; y < height; y++) 
        {
            for(int x = 0; x < width; x++) 
            {
                float amplitude = 1;
                float frequency = 1;
                float noiseHeight = 0;
                float superpositionCompensation = 0;

                for (int i = 0; i < octaves; i++) 
                {
                    float xResult = (x - halfWidth) / scale * frequency + octavesOffset[i].x * frequency;
                    float yResult = (y - halfHeight) / scale * frequency + octavesOffset[i].y * frequency;
                    float generatedValue = Mathf.PerlinNoise(xResult, yResult);
                    noiseHeight += generatedValue * amplitude;
                    noiseHeight -= superpositionCompensation;
                    amplitude *= persistence;
                    frequency *= lacunarity;
                    superpositionCompensation = amplitude / 2;
                }

                noiseMap[x, y] = Mathf.Clamp01(noiseHeight);
            }
        }

        return noiseMap;
    }

    public static void PlaceTiles(int width, int height, float[,] noiseMap, Grid grid, List<Tilemap> tilemaps, List<Tile> tileList)
    {
        for(int y = 0; y < height; y++) 
        {
            for(int x = 0; x < width; x++) 
            {
                float noiseHeight = noiseMap[x, y];
                float colorHeight = noiseHeight * tileList.Count;
                int colorIndex = Mathf.FloorToInt(colorHeight);
                int tileHeight = colorIndex;
                if (colorIndex == tileList.Count)
                {
                    colorIndex = tileList.Count - 1;
                }
                if (colorIndex == -1) 
                {
                    colorIndex = 0;
                }
                tileHeight -= 2;
                if (tileHeight < 0 )
                {
                    tileHeight = 0;
                }
                Tile tile = tileList[colorIndex];
                Vector3Int p = new Vector3Int(x - width / 2 + tileHeight, y - height / 2 + tileHeight, 0);
                tilemaps[colorIndex].SetTile(p, tile);
            }
        }
    }
}
