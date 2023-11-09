using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ObjectSpawner : MonoBehaviour {
 
    public bool[,] InitializeResourceAllowedTilemaps() {
        /*
            Order:
                (1) Grass
                (2) Wheat
        */
        bool[,] resourceAllowedTilemaps = new bool[,] {
            { false, false, false, true, true, true, true, false, false, false, false },
            { false, false, false, true, true, true, true, false, false, false, false }
        };

        return resourceAllowedTilemaps;

    }

    public void PlaceResources (int width, int height, Grid grid, List<Tilemap> tilemaps, List<GameObject> resourcePrefabs, bool[,] resourceAllowedTilemaps, float density) {

        int layerNumber;
        TileBase tile;
        int[] resourceQueue = new int[resourcePrefabs.Count];
        bool guarenteed = false;
        bool alreadyPlaced = false;
        float randomValue;
        float effectiveDensity = density / resourcePrefabs.Count;
        int grid_z;
        Vector3Int grid_xyz_pos;
        Vector3Int Adj_grid_xyz_pos;
        Vector3 world_xyz_pos;

        int count1 = 0;
        int count2 = 0;

        for ( int y = (- height / 2) - 1; y < (height / 2) + tilemaps.Count - 3; y++ ) {
            for ( int x = (- width / 2) - 1; x < (width / 2) + tilemaps.Count - 3; x++ ) {

                grid_xyz_pos = new Vector3Int(x, y, 0);

                layerNumber = tilemaps.Count - 1;
                tile = null;

                while ( layerNumber > -1 ) {

                    tile = tilemaps[layerNumber].GetTile(grid_xyz_pos);
                    if (tile == null) {
                        layerNumber--;
                    }
                    else {

                        for ( int i = 0; i < resourcePrefabs.Count; i++ ) {

                            if ( resourceAllowedTilemaps[i, layerNumber] == true ) {

                                randomValue = Random.Range(0f, 1f);

                                if ( resourceQueue[i] > 0 || randomValue <= effectiveDensity ) {

                                    if ( alreadyPlaced == true && randomValue <= effectiveDensity ) {
                                        resourceQueue[i]++;
                                    }
                                    if ( alreadyPlaced == false ) {

                                        if ( layerNumber < 3 )
                                            grid_z = 0;
                                        else
                                            grid_z = 2 * (layerNumber - 2);

                                        grid_xyz_pos.z += grid_z + 1;
                                        if ( layerNumber >= 3 ) {
                                            grid_xyz_pos.y -= layerNumber - 2;
                                            grid_xyz_pos.x -= layerNumber - 2;
                                        }
                                        world_xyz_pos = grid.CellToWorld(grid_xyz_pos);
                                        Instantiate(resourcePrefabs[i], world_xyz_pos, Quaternion.identity, transform);

                                        if ( i == 0 )
                                            count1++;
                                        else
                                            count2++;

                                        if ( resourceQueue[i] > 0 && randomValue > effectiveDensity )
                                            resourceQueue[i]--;

                                        alreadyPlaced = true;

                                    }

                                }

                            }

                        }

                        alreadyPlaced = false;
                        layerNumber--;
                        grid_xyz_pos = new Vector3Int(x, y, 0);

                    }

                }

            }
    
        }

        Debug.Log("Grass = " + count1);
        Debug.Log("Wheat = " + count2);
    
    }

    public void PlaceResource (  ) {

    }

}