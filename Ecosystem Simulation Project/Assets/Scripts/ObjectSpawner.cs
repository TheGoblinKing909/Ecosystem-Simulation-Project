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

    public void PlaceResources (int width, int height, Grid grid, List<Tilemap> tilemaps, List<GameObject> resourcePrefabs, bool[,] resourceAllowedTilemaps) {

        bool found;
        int layerNumber;
        TileBase tile;
        int grid_z;
        Vector3Int grid_xyz_pos;
        Vector3 world_xyz_pos;

        for ( int y = (- height / 2) - 1; y < (height / 2) + tilemaps.Count - 3; y++ ) {
            for ( int x = (- width / 2) - 1; x < (width / 2) + tilemaps.Count - 3; x++ ) {

                grid_xyz_pos = new Vector3Int(x, y, 0);

                found = false;
                layerNumber = tilemaps.Count - 1;
                tile = null;
                while ( found == false && layerNumber > -1 ) {
                    tile = tilemaps[layerNumber].GetTile(grid_xyz_pos);
                    if (tile == null)
                        layerNumber--;
                    else
                        found = true;
                }

                if ( layerNumber > -1 ) {

                    for ( int i = 0; i < resourcePrefabs.Count; i++ ) {

                    if ( resourceAllowedTilemaps[i, layerNumber] == true ) {

                        float randomValue = Random.Range(0f, 1f);

                        if (randomValue <= 0.1f) {

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

                        }

                    }

                    }

                }

            }
        }

    }

}