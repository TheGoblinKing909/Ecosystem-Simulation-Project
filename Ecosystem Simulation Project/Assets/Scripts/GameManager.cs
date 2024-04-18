using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GameManager : MonoBehaviour
{
    public Vector2 offset;

    // variables for tile placement
    public Grid grid = null;
    public List<Tilemap> tilemaps = new List<Tilemap>();
    public List<Tile> tileList = new List<Tile>();

    // reference to ObjectSpawner script
    public ObjectSpawner resourceSpawner;

    // sequential order MUST match 'resourceAllowedTilemaps' in ObjectSpawner script
    public List<GameObject> resourcePrefabs;

    // suggested default value 0.01 - 0.001
    public float resourceDensity;

    // both used for continual respawning
    public float resourceMax;
    public int totalResourceCount;

    // reference to ObjectSpawner script
    public ObjectSpawner entitySpawner;

    // sequential order MUST match 'entityAllowedTilemaps' in ObjectSpawner script
    public List<GameObject> entityPrefabs;

    // suggested default value 0.01 - 0.001
    public float entityDensity;

    // both used for continual respawning
    public float entityMax;
    public int totalEntityCount;

    public TimeManager timeManager;

    public WeatherManager weatherManager;

    public ClockManager clockManager;

    // variables for ML agents
    public bool isTrainingMode;

    public int frameCount = 0;

    // world generation parameters
    public int inputWidth, inputHeight, inputSeed, inputOctaves;
    public float inputScale, inputPersistence, inputLacunarity, inputResDensity, inputEntDensity;
    public bool loadWorld;
    public string worldName;

    void Start()
    {
        if (Application.isEditor)
        {
            // loadWorld = MainMenuController.loadWorld;
            // worldName = MainMenuController.WorldName;
            loadWorld = false;
            worldName = "TestWorld";
        }
        else
        {
            // string[] args = System.Environment.GetCommandLineArgs();
            // loadWorld = bool.Parse(args[0]);
            // worldName = args[1];
            loadWorld = false;
            worldName = "TestWorld";
        }

        if (!loadWorld)
        {
            if (Application.isEditor)
            {
                inputWidth = MainMenuController.inputWidth;
                inputHeight = MainMenuController.inputHeight;
                inputSeed = MainMenuController.inputSeed;
                inputOctaves = MainMenuController.inputOctaves;
                inputScale = MainMenuController.inputScale;
                inputPersistence = MainMenuController.inputPersistence;
                inputLacunarity = MainMenuController.inputLacunarity;
                inputResDensity = MainMenuController.inputResDensity;
                inputEntDensity = MainMenuController.inputEntDensity;
                // inputWidth = 100;
                // inputHeight = 100;
                // inputSeed = 123;
                // inputOctaves = 3;
                // inputScale = 75f;
                // inputPersistence = .3f;
                // inputLacunarity = 3f;
                // inputResDensity = .005f;
                // inputEntDensity = .003f;
            }
            else
            {
                string[] args = System.Environment.GetCommandLineArgs();
                int argLen = args.Length;
                inputWidth = int.Parse(args[argLen-9]);
                inputHeight = int.Parse(args[argLen-8]);
                inputSeed = int.Parse(args[argLen-7]);
                inputOctaves = int.Parse(args[argLen-6]);
                inputScale = float.Parse(args[argLen-5]);
                inputPersistence = float.Parse(args[argLen-4]);
                inputLacunarity = float.Parse(args[argLen-3]);
                inputResDensity = float.Parse(args[argLen-2]);
                inputEntDensity = float.Parse(args[argLen-1]);

                DisplayTensorboard();
            }
            float[,] noiseMap = WorldGenerator.GenerateNoiseMap(inputWidth, inputHeight, inputSeed, inputScale, inputOctaves, inputPersistence, inputLacunarity, offset);
            WorldGenerator.PlaceTiles(inputWidth, inputHeight, noiseMap, grid, tilemaps, tileList);
            resourceSpawner.OnInstantiate();
            resourceMax = resourceSpawner.PlaceResources();
            entitySpawner.OnInstantiate();
            entityMax = entitySpawner.PlaceEntities();
        }
        else
        {
            Load();
        }
    }

    void Update()
    {
        if (++frameCount > 20) {
            frameCount = 0;

            totalResourceCount = resourceSpawner.GetChildCount();
            totalEntityCount = entitySpawner.GetChildCount();

            int resourceSpawnAmount = CalculateSpawnAmount(resourceMax, totalResourceCount);
            int entitySpawnAmount = CalculateSpawnAmount(entityMax, totalEntityCount);

            if (resourceSpawnAmount > 0) {
                resourceSpawner.SpawnResources(resourceSpawnAmount);
            }
            if (entitySpawnAmount > 0) {
                entitySpawner.SpawnEntities(entitySpawnAmount);
            }
            // Save();
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SaveAndExit();
        }
    }

    int CalculateSpawnAmount(float max, int current)
    {
        float deficit = max - current;
        float proportion = 0.1f; // Adjust this value as needed

        // Ensure at least 1 is spawned if there's any deficit at all,
        // but cap it with a calculated proportion of the deficit to avoid overpopulation
        int spawnAmount = Mathf.Clamp(Mathf.CeilToInt(deficit * proportion), 0, Mathf.Max(1, Mathf.FloorToInt(deficit)));

        return spawnAmount;
    }

    void DisplayTensorboard()
    {
        Process tensorboardProcess = new Process();
        tensorboardProcess.StartInfo.UseShellExecute = false;
        tensorboardProcess.StartInfo.RedirectStandardInput = true;
        tensorboardProcess.StartInfo.FileName = "cmd.exe";
        tensorboardProcess.StartInfo.Arguments = @"/K ..\anaconda3\Scripts\activate.bat ..\anaconda3";
        tensorboardProcess.Start();

        tensorboardProcess.StandardInput.WriteLine("conda activate build-env");
        tensorboardProcess.StandardInput.WriteLine("tensorboard --logdir=results");

        Application.OpenURL("http://localhost:6006/");
    }

    void SaveAndExit()
    {
        Save();
        if (!Application.isEditor)
        {
            Process[] processes = Process.GetProcessesByName("mlagents-learn");
            foreach (Process process in processes)
            {
                process.Kill();
                process.WaitForExit();
                process.Dispose();
            }
            processes = Process.GetProcessesByName("tensorboard");
            foreach (Process process in processes)
            {
                process.Kill();
                process.WaitForExit();
                process.Dispose();
            }
            processes = Process.GetProcessesByName("cmd");
            foreach (Process process in processes)
            {
                process.Kill();
                process.WaitForExit();
                process.Dispose();
            }
            Application.Quit();
        }
    }

    void Save()
    {
        string savePath = Application.persistentDataPath + "/Saves/";
        if (!System.IO.Directory.Exists(savePath))
        {
            System.IO.Directory.CreateDirectory(savePath);
        }
        savePath += worldName + ".json";

        SaveData saveData = new SaveData();
        saveData.width = inputWidth;
        saveData.height = inputHeight;
        saveData.seed = inputSeed;
        saveData.octaves = inputOctaves;
        saveData.scale = inputScale;
        saveData.persistence = inputPersistence;
        saveData.lacunarity = inputLacunarity;
        saveData.resDensity = inputResDensity;
        saveData.entDensity = inputEntDensity;

        foreach (Transform child in resourceSpawner.transform)
        {
            ResourceData resData = new ResourceData();
            resData.position = child.position;
            Resource res = child.GetComponent<Resource>();
            resData.harvest = res.HarvestRemaining;
            resData.health = res.HealthRemaining;
            resData.index = res.PrefabIndex;
            saveData.resources.Add(resData);
        }

        foreach (Transform child in entitySpawner.transform)
        {
            EntityData entData = new EntityData();
            entData.position = child.position;
            entData.layer = child.gameObject.layer;
            if (child.GetComponent<Carnivore>() != null)
            {
                Carnivore ent = child.GetComponent<Carnivore>();
                entData.health = ent.currentHealth;
                entData.stamina = ent.currentStamina;
                entData.hunger = ent.currentHunger;
                entData.thirst = ent.currentThirst;
                entData.age = ent.currentAge;
                entData.ageTime = ent.ageTime;
                entData.index = ent.prefabIndex;
                saveData.entities.Add(entData);
            }
            else if (child.GetComponent<Omnivore>() != null)
            {
                Omnivore ent = child.GetComponent<Omnivore>();
                entData.health = ent.currentHealth;
                entData.stamina = ent.currentStamina;
                entData.hunger = ent.currentHunger;
                entData.thirst = ent.currentThirst;
                entData.age = ent.currentAge;
                entData.ageTime = ent.ageTime;
                entData.index = ent.prefabIndex;
                saveData.entities.Add(entData);
            }
            else
            {
                Herbivore ent = child.GetComponent<Herbivore>();
                entData.health = ent.currentHealth;
                entData.stamina = ent.currentStamina;
                entData.hunger = ent.currentHunger;
                entData.thirst = ent.currentThirst;
                entData.age = ent.currentAge;
                entData.ageTime = ent.ageTime;
                entData.index = ent.prefabIndex;
                saveData.entities.Add(entData);
            }
        }

        string saveDataString = JsonUtility.ToJson(saveData);
        System.IO.File.WriteAllText(savePath, saveDataString);
        UnityEngine.Debug.Log("Saved to " + savePath);
    }

    void Load()
    {
        string savePath = Application.persistentDataPath + "/Saves/";
        if (!System.IO.Directory.Exists(savePath))
        {
            System.IO.Directory.CreateDirectory(savePath);
        }
        savePath += worldName + ".json";

        string saveDataString = System.IO.File.ReadAllText(savePath);
        SaveData saveData = new SaveData();
        saveData = JsonUtility.FromJson<SaveData>(saveDataString);

        inputWidth = saveData.width;
        inputHeight = saveData.height;
        inputSeed = saveData.seed;
        inputOctaves = saveData.octaves;
        inputScale = saveData.scale;
        inputPersistence = saveData.persistence;
        inputLacunarity = saveData.lacunarity;
        inputResDensity = saveData.resDensity;
        inputEntDensity = saveData.entDensity;

        float[,] noiseMap = WorldGenerator.GenerateNoiseMap(inputWidth, inputHeight, inputSeed, inputScale, inputOctaves, inputPersistence, inputLacunarity, offset);
        WorldGenerator.PlaceTiles(inputWidth, inputHeight, noiseMap, grid, tilemaps, tileList);

        resourceSpawner.OnInstantiate();
        foreach (ResourceData resData in saveData.resources)
        {
            GameObject instantiatedResource = Instantiate(resourcePrefabs[resData.index], resData.position, Quaternion.identity, resourceSpawner.transform);
            Resource res = instantiatedResource.GetComponent<Resource>();
            res.HarvestRemaining = resData.harvest;
            res.HealthRemaining = resData.health;
            res.PrefabIndex = resData.index;
        }

        entitySpawner.OnInstantiate();
        foreach (EntityData entData in saveData.entities)
        {
            GameObject instantiatedEntity = Instantiate(entityPrefabs[entData.index], entData.position, Quaternion.identity, entitySpawner.transform);
            instantiatedEntity.layer = entData.layer;
            Movement movementScript = instantiatedEntity.GetComponent<Movement>();
            movementScript.OnInstantiate();

            if (instantiatedEntity.GetComponent<Carnivore>() != null)
            {
                Carnivore ent = instantiatedEntity.GetComponent<Carnivore>();
                ent.currentHealth = entData.health;
                ent.currentStamina = entData.stamina;
                ent.currentHunger = entData.hunger;
                ent.currentThirst = entData.thirst;
                ent.currentAge = entData.age;
                ent.ageTime = entData.ageTime;
                ent.prefabIndex = entData.index;
                ent.isLoaded = true;
            }
            else if (instantiatedEntity.GetComponent<Omnivore>() != null)
            {
                Omnivore ent = instantiatedEntity.GetComponent<Omnivore>();
                ent.currentHealth = entData.health;
                ent.currentStamina = entData.stamina;
                ent.currentHunger = entData.hunger;
                ent.currentThirst = entData.thirst;
                ent.currentAge = entData.age;
                ent.ageTime = entData.ageTime;
                ent.prefabIndex = entData.index;
                ent.isLoaded = true;
            }
            else
            {
                Herbivore ent = instantiatedEntity.GetComponent<Herbivore>();
                ent.currentHealth = entData.health;
                ent.currentStamina = entData.stamina;
                ent.currentHunger = entData.hunger;
                ent.currentThirst = entData.thirst;
                ent.currentAge = entData.age;
                ent.ageTime = entData.ageTime;
                ent.prefabIndex = entData.index;
                ent.isLoaded = true;
            }
        }

        UnityEngine.Debug.Log("Loaded from " + savePath);
    }

}

[System.Serializable]
public class SaveData
{
    public int width;
    public int height;
    public int seed;
    public int octaves;
    public float scale;
    public float persistence;
    public float lacunarity;
    public float resDensity;
    public float entDensity;
    public List<ResourceData> resources = new List<ResourceData>();
    public List<EntityData> entities = new List<EntityData>();
}

[System.Serializable]
public class ResourceData
{
    public Vector3 position;
    public float harvest;
    public float health;
    public int index;
}

[System.Serializable]
public class EntityData
{
    public Vector3 position;
    public int layer;
    public float health;
    public float stamina;
    public float hunger;
    public float thirst;
    public float age;
    public float ageTime;
    public int index;
}