using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ObjectSpawner : MonoBehaviour {

    public void PlaceResources (Grid grid, GameObject resourcePrefab) {

        for (int y = 0; y < 5; ++y) {
            for (int x = 0; x < 10; ++x) {
                Vector3Int xyz_pos = new Vector3Int(x, y, 25);
                Vector3 grid_xyz_pos = grid.CellToWorld(xyz_pos);
                Instantiate(resourcePrefab, grid_xyz_pos, Quaternion.identity, transform);
            }
        }

    }

}