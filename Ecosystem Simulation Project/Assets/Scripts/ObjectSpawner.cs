using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ObjectSpawner : MonoBehaviour {

    public void PlaceResources (int width, int height, Grid grid, List<Tilemap> tilemaps, GameObject resourcePrefab, List<Tile> tileList) {

        int grid_z;
        Vector3Int grid_xyz_pos;
        Vector3 world_xyz_pos;

        for( int step = 0; step < tilemaps.Count; step++) {

            if (step < 3)
                grid_z = 0;
            else
                grid_z = 2 * (step - 2);

            for ( int y = (- height / 2) - 1; y < (height / 2) + tilemaps.Count - 3; y++) {
                for ( int x = (- width / 2) - 1; x < (width / 2) + tilemaps.Count - 3; x++) {

                    grid_xyz_pos = new Vector3Int(x, y, 0);
                    if ( tilemaps[step].HasTile(grid_xyz_pos) ) {
                        grid_xyz_pos.z += grid_z + 1;
                        if ( step >= 3 ) {
                            grid_xyz_pos.y -= step - 2;
                            grid_xyz_pos.x -= step - 2;
                        }
                        world_xyz_pos = grid.CellToWorld(grid_xyz_pos);
                        Instantiate(resourcePrefab, world_xyz_pos, Quaternion.identity, transform);
                    }

                }

            }

        }

    }
}