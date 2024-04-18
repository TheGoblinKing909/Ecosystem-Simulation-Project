using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ObjectSpawner : MonoBehaviour {

    public GameManager gameManager; // Reference to the GameManager
    public Grid grid;
    public int width;
    public int height;
    public List<Tilemap> tilemaps;

    // sequential order MUST match 'resourceAllowedTilemaps' from below
    public List<GameObject> resourcePrefabs;

    /*
    Order:
        (1) Grass
        (2) Wheat
        (3) Cave
    */
    /*
    public bool[,] resourceAllowedTilemaps = new bool[,] {
        { false, false, false, true, true, true, true, false, false, false, false },
        { false, false, false, true, true, true, true, false, false, false, false },
        { false, false, false, false, false, false, false, true, true, true, true }
    };
    */

    public float resourceDensity;

    // sequential order MUST match 'entityAllowedTilemaps' from below
    public List<GameObject> entityPrefabs;

    /*
    Order:
        (1) Human
        (2) Bear
    */
    /*
    public bool[,] entityAllowedTilemaps = new bool[,] {
        { false, false, false, true, true, false, false, false, false, false, false },
        { false, false, false, false, false, true, true, false, false, false, false }
    };
    */

    public float entityDensity;

    public void OnInstantiate() {

        gameManager = transform.parent.GetComponent<GameManager>();

        if ( gameManager != null ) {
            width = gameManager.inputWidth;
            height = gameManager.inputHeight;
            grid = gameManager.grid;
            tilemaps = gameManager.tilemaps;
            resourcePrefabs = gameManager.resourcePrefabs;
            // resourceDensity = gameManager.resourceDensity;
            resourceDensity = gameManager.inputResDensity;
            entityPrefabs = gameManager.entityPrefabs;
            //entityDensity = gameManager.entityDensity;
            entityDensity = gameManager.inputEntDensity;
        }
        else {
            Debug.LogError("GameManager not found!");
        }

    }

    public int GetChildCount () {
        return transform.childCount;
    }

    public int PlaceResources () {

        int layerNumber;
        TileBase tile;
        int[] resourceQueue = new int[resourcePrefabs.Count];
        bool alreadyPlaced = false;
        float randomValue;
        float effectiveDensity = resourceDensity / resourcePrefabs.Count;
        int grid_z;
        Vector3Int grid_xyz_pos;
        Vector3 world_xyz_pos;

        int totalResourceCount = 0;

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

                            // if ( resourceAllowedTilemaps[i, layerNumber] == true ) {

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
                                        GameObject instantiatedResource = Instantiate(resourcePrefabs[i], world_xyz_pos, Quaternion.identity, transform);
                                        Vector3 resourcePosition = instantiatedResource.transform.position;
                                        resourcePosition.z = 25; // Set the z-coordinate to 0
                                        instantiatedResource.transform.position = resourcePosition; // Update the position
                                        Resource res = instantiatedResource.GetComponent<Resource>();
                                        res.MaxRemaining();
                                        res.PrefabIndex = i;

                                        totalResourceCount++;

                                        if ( resourceQueue[i] > 0 && randomValue > effectiveDensity )
                                            resourceQueue[i]--;

                                        alreadyPlaced = true;

                                    }

                                }

                            // }

                        }

                        alreadyPlaced = false;
                        layerNumber--;
                        grid_xyz_pos = new Vector3Int(x, y, 0);

                    }

                }

            }
    
        }

        //Debug.Log("Grass = " + count1);
        //Debug.Log("Wheat = " + count2);
        return totalResourceCount;

    }

    public void SpawnResources ( int spawnAmount ) {

        int layerNumber;
        TileBase tile;
        int grid_z;
        Vector3Int grid_xyz_pos;
        Vector3 world_xyz_pos;

        int y, x;
        while ( spawnAmount > 0 ) {

            y = Random.Range( (- height / 2) - 1 , (height / 2) + tilemaps.Count - 3 + 1 );
            x = Random.Range( (- width / 2) - 1 , (width / 2) + tilemaps.Count - 3 + 1 );

            grid_xyz_pos = new Vector3Int(x, y, 0);

            layerNumber = tilemaps.Count - 1;
            tile = null;

            while ( layerNumber > -1 && spawnAmount > 0 ) {

                tile = tilemaps[layerNumber].GetTile(grid_xyz_pos);

                if (tile == null) {
                    layerNumber--;
                }

                else {

                    int i = Random.Range( 0 , resourcePrefabs.Count );
                    bool spawned = false;
                    int attempts = 0;

                    while ( !spawned && attempts < 10 ) {

                        // if ( resourceAllowedTilemaps[i, layerNumber] == true ) {

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
                            GameObject instantiatedResource = Instantiate(resourcePrefabs[i], world_xyz_pos, Quaternion.identity, transform);
                            Vector3 resourcePosition = instantiatedResource.transform.position;
                            resourcePosition.z = 25; // Set the z-coordinate to 0
                            instantiatedResource.transform.position = resourcePosition; // Update the position
                            Resource res = instantiatedResource.GetComponent<Resource>();
                            res.MaxRemaining();
                            res.PrefabIndex = i;

                            spawned = true;
                            spawnAmount--;

                        // }

                        attempts++;

                    }

                layerNumber--;
                grid_xyz_pos = new Vector3Int(x, y, 0);

                }

            }

        }

    }

    public int PlaceEntities () {

        int layerNumber;
        TileBase tile;
        int[] entityQueue = new int[entityPrefabs.Count];
        bool alreadyPlaced = false;
        float randomValue;
        float effectiveDensity = entityDensity / entityPrefabs.Count;
        int grid_z;
        Vector3Int grid_xyz_pos;
        Vector3 world_xyz_pos;

        int totalEntityCount = 0;

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

                        for ( int i = 0; i < entityPrefabs.Count; i++ ) {

                            // if ( entityAllowedTilemaps[i, layerNumber] == true ) {

                                randomValue = Random.Range(0f, 1f);

                                if ( entityQueue[i] > 0 || randomValue <= effectiveDensity ) {

                                    if ( alreadyPlaced == true && randomValue <= effectiveDensity ) {
                                        entityQueue[i]++;
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
                                        GameObject instantiatedEntity = Instantiate(entityPrefabs[i], world_xyz_pos, Quaternion.identity, transform);
                                        Vector3 entityPosition = instantiatedEntity.transform.position;
                                        entityPosition.z = 25; // Set the z-coordinate to 0
                                        instantiatedEntity.transform.position = entityPosition; // Update the position

                                        instantiatedEntity.layer = layerNumber + 6;
                                        Movement movementScript = instantiatedEntity.GetComponent<Movement>();
                                        movementScript.OnInstantiate();

                                        if (instantiatedEntity.GetComponent<Carnivore>() != null)
                                        {
                                            Carnivore ent = instantiatedEntity.GetComponent<Carnivore>();
                                            ent.prefabIndex = i;
                                        }
                                        else if (instantiatedEntity.GetComponent<Omnivore>() != null)
                                        {
                                            Omnivore ent = instantiatedEntity.GetComponent<Omnivore>();
                                            ent.prefabIndex = i;
                                        }
                                        else
                                        {
                                            Herbivore ent = instantiatedEntity.GetComponent<Herbivore>();
                                            ent.prefabIndex = i;
                                        }
                                    
                                        totalEntityCount++;

                                        if ( entityQueue[i] > 0 && randomValue > effectiveDensity )
                                            entityQueue[i]--;

                                        alreadyPlaced = true;

                                    }

                                }

                            // }

                        }

                        alreadyPlaced = false;
                        layerNumber--;
                        grid_xyz_pos = new Vector3Int(x, y, 0);

                    }

                }

            }

        }

        return totalEntityCount;

    }

    public void SpawnEntities ( int spawnAmount ) {

        int layerNumber;
        TileBase tile;
        int grid_z;
        Vector3Int grid_xyz_pos;
        Vector3 world_xyz_pos;

        int y, x;
        while ( spawnAmount > 0 ) {

            y = Random.Range( (- height / 2) - 1 , (height / 2) + tilemaps.Count - 3 + 1 );
            x = Random.Range( (- width / 2) - 1 , (width / 2) + tilemaps.Count - 3 + 1 );

            grid_xyz_pos = new Vector3Int(x, y, 0);

            layerNumber = tilemaps.Count - 1;
            tile = null;

            while ( layerNumber > -1 && spawnAmount > 0 ) {

                tile = tilemaps[layerNumber].GetTile(grid_xyz_pos);

                if (tile == null) {
                    layerNumber--;
                }

                else {

                    int i = Random.Range( 0 , entityPrefabs.Count );
                    bool spawned = false;
                    int attempts = 0;

                    while ( !spawned && attempts < 10 ) {

                        // if ( entityAllowedTilemaps[i, layerNumber] == true ) {

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
                            GameObject instantiatedEntity = Instantiate(entityPrefabs[i], world_xyz_pos, Quaternion.identity, transform);
                            Vector3 entityPosition = instantiatedEntity.transform.position;
                            entityPosition.z = 25; // Set the z-coordinate to 0
                            instantiatedEntity.transform.position = entityPosition; // Update the position

                            instantiatedEntity.layer = layerNumber + 6;
                            Movement movementScript = instantiatedEntity.GetComponent<Movement>();
                            movementScript.OnInstantiate();

                            if (instantiatedEntity.GetComponent<Carnivore>() != null)
                            {
                                Carnivore ent = instantiatedEntity.GetComponent<Carnivore>();
                                ent.prefabIndex = i;
                            }
                            else if (instantiatedEntity.GetComponent<Omnivore>() != null)
                            {
                                Omnivore ent = instantiatedEntity.GetComponent<Omnivore>();
                                ent.prefabIndex = i;
                            }
                            else
                            {
                                Herbivore ent = instantiatedEntity.GetComponent<Herbivore>();
                                ent.prefabIndex = i;
                            }

                            spawned = true;
                            spawnAmount--;

                        // }

                        attempts++;

                    }

                    layerNumber--;
                    grid_xyz_pos = new Vector3Int(x, y, 0);

                }

            }

        }

    }

}