using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ObjectSpawner : MonoBehaviour {

    /* 
    public void InitializePrefabs( List<List<GameObject>> resourcePrefabs, int maxLayers, int maxResources ) {
        resourcePrefabs.Clear();

        for (int i = 0; i < maxLayers; i++) {
            List<GameObject> layer_i = new List<GameObject>();
            for (int j = 0; j < maxResources; j++) {
                
                // Add your logic here to determine which prefab should go in each cell.
                // For example, you can use random selection or some other logic.

                

                GameObject prefabToAdd = SelectRandomPrefab();
                layer_i.Add(prefabToAdd);
            }
            resourcePrefabs.Add(layer_i);
        }
    }
    */

    public void PlaceResources (int width, int height, Grid grid, List<Tilemap> tilemaps, GameObject resourcePrefab) {

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
                    Instantiate(resourcePrefab, world_xyz_pos, Quaternion.identity, transform);

                }

            }
        }

    }

}