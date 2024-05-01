using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.SceneManagement;

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
    private string[] args;
    private int argLen, mlagentsId;
    public List<GameObject> initialModels;

    void Start()
    {
        if (Application.isEditor)
        {
            loadWorld = MainMenuController.loadWorld;
            worldName = MainMenuController.worldName;
        }
        else
        {
            args = System.Environment.GetCommandLineArgs();
            argLen = args.Length;
            loadWorld = bool.Parse(args[argLen-3]);
            worldName = args[argLen-2];
            mlagentsId = int.Parse(args[argLen-1]);
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
            }
            else
            {
                inputWidth = int.Parse(args[argLen-12]);
                inputHeight = int.Parse(args[argLen-11]);
                inputSeed = int.Parse(args[argLen-10]);
                inputOctaves = int.Parse(args[argLen-9]);
                inputScale = float.Parse(args[argLen-8]);
                inputPersistence = float.Parse(args[argLen-7]);
                inputLacunarity = float.Parse(args[argLen-6]);
                inputResDensity = float.Parse(args[argLen-5]);
                inputEntDensity = float.Parse(args[argLen-4]);

                // DisplayTensorboard();
            }
            float[,] noiseMap = WorldGenerator.GenerateNoiseMap(inputWidth, inputHeight, inputSeed, inputScale, inputOctaves, inputPersistence, inputLacunarity, offset);
            WorldGenerator.PlaceTiles(inputWidth, inputHeight, noiseMap, grid, tilemaps, tileList);
            resourceSpawner.OnInstantiate();
            resourceMax = resourceSpawner.PlaceResources();
            entitySpawner.OnInstantiate();
            entityMax = entitySpawner.PlaceEntities();
            initialModels = entitySpawner.InitializeModels();
        }
        else
        {
            Load();
        }
    }

    void Update()
    {
        if (++frameCount > 10) {
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
            if (initialModels.Count > 0)
            {
                entitySpawner.RemoveModels(initialModels);
            }
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SaveAndExit();
        }
        if (Input.GetKeyDown(KeyCode.T))
        {
            DisplayTensorboard();
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
        tensorboardProcess.StandardInput.WriteLine("tensorboard --logdir=results/" + worldName);

        Application.OpenURL("http://localhost:6006/");
    }

    internal const int CTRL_C_EVENT = 0;
    [DllImport("kernel32.dll")]
    internal static extern bool GenerateConsoleCtrlEvent(uint dwCtrlEvent, uint dwProcessGroupId);
    [DllImport("kernel32.dll", SetLastError = true)]
    internal static extern bool AttachConsole(uint dwProcessId);
    [DllImport("kernel32.dll")]
    static extern bool SetConsoleCtrlHandler(ConsoleCtrlDelegate HandlerRoutine, bool Add);
    delegate bool ConsoleCtrlDelegate(uint CtrlType);

    void SaveAndExit()
    {
        Save();
        if (!Application.isEditor)
        {
            if (AttachConsole((uint) mlagentsId))
            {
                SetConsoleCtrlHandler(null, true);
                GenerateConsoleCtrlEvent(CTRL_C_EVENT, 0);
            }
        }
        else
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
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
            Attributes ent = child.GetComponent<Attributes>();
            entData.health = ent.currentHealth;
            entData.stamina = ent.currentStamina;
            entData.hunger = ent.currentHunger;
            entData.thirst = ent.currentThirst;
            entData.age = ent.currentAge;
            entData.ageTime = ent.ageTime;
            entData.index = ent.prefabIndex;
            saveData.entities.Add(entData);
        }

        saveData.minutes = timeManager.DateTime.Minutes;
        saveData.hour = timeManager.DateTime.Hour;
        saveData.day = (int) timeManager.DateTime.Day;
        saveData.dayOfMonth = timeManager.DateTime.DayOfMonth;
        saveData.week = timeManager.DateTime.Week;
        saveData.month = (int) timeManager.DateTime.Month;
        saveData.season = (int) timeManager.DateTime.Season;
        saveData.year = timeManager.DateTime.Year;

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
            Attributes ent = instantiatedEntity.GetComponent<Attributes>();
            ent.currentHealth = entData.health;
            ent.currentStamina = entData.stamina;
            ent.currentHunger = entData.hunger;
            ent.currentThirst = entData.thirst;
            ent.currentAge = entData.age;
            ent.ageTime = entData.ageTime;
            ent.prefabIndex = entData.index;
            ent.isLoaded = true;
        }

        timeManager.DateTime.SetTime(saveData.minutes, saveData.hour, saveData.day, saveData.dayOfMonth, saveData.week, saveData.month, saveData.season, saveData.year);
        TimeManager.OnDateTimeChanged?.Invoke(timeManager.DateTime);

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
    public int minutes;
    public int hour;
    public int day;
    public int dayOfMonth;
    public int week;
    public int month;
    public int season;
    public int year;
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