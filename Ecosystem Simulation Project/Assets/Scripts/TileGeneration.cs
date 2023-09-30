using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileGeneration : MonoBehaviour
{
    public float scale;
    public int size;
    public Tilemap tilemap = null;
    public List<Tile> tileList = new List<Tile>();

    // Start is called before the first frame update
    void Start()
    {
        float[,] noiseMap = new float[size, size];
        (float xOffset, float yOffset) = (Random.Range(-10000f, 10000f), Random.Range(-10000f, 10000f));
        for(int y = 0; y < size; y++) {
            for(int x = 0; x < size; x++) {
                float noiseValue = Mathf.PerlinNoise(x * scale + xOffset, y * scale + yOffset);
                noiseMap[x, y] = noiseValue;
            }
        }

        for(int y = 0; y < size; y++) {
            for(int x = 0; x < size; x++) {
                float noiseHeight = noiseMap[x, y];
                float colorHeight = noiseHeight * tileList.Count;
                int colorIndex = Mathf.FloorToInt(colorHeight);
                if (colorIndex == tileList.Count)
                {
                    colorIndex = tileList.Count-1;
                }
                if (colorIndex == -1) {
                    colorIndex = 0;
                }
                float tileHeight = noiseHeight * tileList.Count;
                int tileHeightIndex = Mathf.FloorToInt(tileHeight) * 2;
                tileHeightIndex -= 4;
                if (tileHeightIndex < 0)
                {
                    tileHeightIndex = 0;
                }
                Tile tile = tileList[colorIndex];
                Vector3Int p = new Vector3Int(x - size / 2 + tileHeightIndex / 2, y - size / 2  + tileHeightIndex / 2, tileHeightIndex);
                tilemap.SetTile(p, tile);
            }
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
