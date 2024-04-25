using UnityEngine;
using System;
using TMPro;
using weather.effects;

public class WeatherManager : MonoBehaviour
{
    public GameManager gameManager;
    public TimeManager timeManager;
    public TextMeshProUGUI Temperature;
    private int lastHour = -1;
    private int temperature;
    public WeatherEvent currentWeatherEvent;

    private WeatherEvent[] springEvents;
    private WeatherEvent[] summerEvents;
    private WeatherEvent[] autumnEvents;
    private WeatherEvent[] winterEvents;

    [System.Serializable]
    public class WeatherEvent
    {
        public string Name;
        public float TemperatureOffset;
        public AttributeEffects EntityEffects;
        public AttributeEffects ResourceEffects;

        public WeatherEvent(string name, float temperatureOffset, AttributeEffects entityEffects, AttributeEffects resourceEffects)
        {
            Name = name;
            TemperatureOffset = temperatureOffset;
            EntityEffects = entityEffects;
            ResourceEffects = resourceEffects;
        }
    }

    private void Start()
    {

        if (gameManager == null)
        {
            gameManager = FindObjectOfType<GameManager>();
        }

        if (timeManager == null)
        {
            Debug.LogError("TimeManager reference not assigned!");
            return;
        }

        TimeManager.OnDateTimeChanged += HandleTimeChanged;
        InitializeWeatherEvents();
    }

    void InitializeWeatherEvents()
    {
        springEvents = new WeatherEvent[]
        {
            new WeatherEvent("Sunny", 5f, new AttributeEffects(1, 1, -0.1f, -0.1f, 0.5f, 0, -0.05f, -0.05f), new AttributeEffects(0, 0, 0, 0, 0, 10, 0, 0)),
            new WeatherEvent("Heatwave", 10f, new AttributeEffects(-1, -1, 0.2f, 0.2f, -0.5f, 0, 0.1f, 0.1f), new AttributeEffects(0, 0, 0, 0, 0, -5, 0, 0))
            
        };
        summerEvents = new WeatherEvent[]
        {
            new WeatherEvent("Sunny", 5f, new AttributeEffects(1, 1, -0.1f, -0.1f, 0.5f, 0, -0.05f, -0.05f), new AttributeEffects(0, 0, 0, 0, 0, 10, 0, 0)),
            new WeatherEvent("Heatwave", 10f, new AttributeEffects(-1, -1, 0.2f, 0.2f, -0.5f, 0, 0.1f, 0.1f), new AttributeEffects(0, 0, 0, 0, 0, -5, 0, 0))
            
        };
        autumnEvents = new WeatherEvent[]
        {
            new WeatherEvent("Snow", -10f, new AttributeEffects(-1, -1, 0.1f, 0.1f, -0.5f, 0, 0.05f, 0.05f), new AttributeEffects(0, 0, 0, 0, 0, -10, 0, 0)),
            new WeatherEvent("Blizzard", -15f, new AttributeEffects(-2, -2, 0.2f, 0.2f, -1f, 0, 0.1f, 0.1f), new AttributeEffects(0, 0, 0, 0, 0, -15, 0, 0))
            
        };
        winterEvents = new WeatherEvent[]
        {
            new WeatherEvent("Snow", -10f, new AttributeEffects(-1, -1, 0.1f, 0.1f, -0.5f, 0, 0.05f, 0.05f), new AttributeEffects(0, 0, 0, 0, 0, -10, 0, 0)),
            new WeatherEvent("Blizzard", -15f, new AttributeEffects(-2, -2, 0.2f, 0.2f, -1f, 0, 0.1f, 0.1f), new AttributeEffects(0, 0, 0, 0, 0, -15, 0, 0))
            
        };
        currentWeatherEvent = springEvents[0];
        //currentWeatherEvent = springEvents[1];
        //currentWeatherEvent = summerEvents[0];
        //currentWeatherEvent = summerEvents[1];
        //currentWeatherEvent = autumnEvents[0];
        //currentWeatherEvent = autumnEvents[1];
        //currentWeatherEvent = winterEvents[0];
        //currentWeatherEvent = winterEvents[1];
    }

    public int GetCurrentTemperature()
    {
        return temperature;
    }

    private void HandleTimeChanged(DateTime currentDateTime)
    {

        if (currentDateTime.Hour != lastHour)
        {
            lastHour = currentDateTime.Hour;
            float currentTemperature = CalculateCurrentTemperature(currentDateTime, currentWeatherEvent.TemperatureOffset);

            if (Temperature != null)
            {
                int roundedTemperature = Mathf.RoundToInt(currentTemperature);
                Temperature.text = roundedTemperature.ToString() + "°F";
            }

            if (UnityEngine.Random.value <= 0.10)
            {
                ChangeWeatherEvent(currentDateTime.Season);
            }
            Debug.Log($"Current Weather Event: {currentWeatherEvent.Name}.");

        }

        ApplyWeatherEffectsToEntities(currentWeatherEvent.EntityEffects);
        ApplyWeatherEffectsToResources(currentWeatherEvent.ResourceEffects);

    }

    float CalculateCurrentTemperature(DateTime dateTime, float weatherOffset)
    {
        float temperature = 0f;
        switch (dateTime.Hour)
        {
            case 0:
                temperature = UnityEngine.Random.Range(50f, 54f);
                break;
            case 1:
                temperature = UnityEngine.Random.Range(45f, 51f);
                break;
            case 2:
                temperature = UnityEngine.Random.Range(41f, 46f);
                break;
            case 3:
                temperature = UnityEngine.Random.Range(38f, 42f);
                break;
            case 4:
                temperature = UnityEngine.Random.Range(35f, 39f);
                break;
            case 5:
                temperature = UnityEngine.Random.Range(30f, 36f);
                break;
            case 6:
                temperature = UnityEngine.Random.Range(35f, 39f);
                break;
            case 7:
                temperature = UnityEngine.Random.Range(38f, 43f);
                break;
            case 8:
                temperature = UnityEngine.Random.Range(42f, 46f);
                break;
            case 9:
                temperature = UnityEngine.Random.Range(45f, 49f);
                break;
            case 10:
                temperature = UnityEngine.Random.Range(48f, 53f);
                break;
            case 11:
                temperature = UnityEngine.Random.Range(52f, 57f);
                break;
            case 12:
                temperature = UnityEngine.Random.Range(56f, 61f);
                break;
            case 13:
                temperature = UnityEngine.Random.Range(60f, 65f);
                break;
            case 14:
                temperature = UnityEngine.Random.Range(64f, 69f);
                break;
            case 15:
                temperature = UnityEngine.Random.Range(68f, 74f);
                break;
            case 16:
                temperature = UnityEngine.Random.Range(73f, 77f);
                break;
            case 17:
                temperature = UnityEngine.Random.Range(76f, 80f);
                break;
            case 18:
                temperature = UnityEngine.Random.Range(71f, 76f);
                break;
            case 19:
                temperature = UnityEngine.Random.Range(68f, 72f);
                break;
            case 20:
                temperature = UnityEngine.Random.Range(65f, 69f);
                break;
            case 21:
                temperature = UnityEngine.Random.Range(61f, 66f);
                break;
            case 22:
                temperature = UnityEngine.Random.Range(57f, 62f);
                break;
            case 23:
                temperature = UnityEngine.Random.Range(53f, 58f);
                break;
        }

        temperature += GetSeasonalOffset(timeManager.GetCurrentDateTime().Season) + weatherOffset;

        return temperature;
    }

    float GetSeasonalOffset(Season season)
    {
        switch (season)
        {
            case Season.Spring:
                return 5f;
            case Season.Summer:
                return 20f;
            case Season.Autumn:
                return 10f;
            case Season.Winter:
                return -20f;
            default:
                return 0f;
        }
    }

    void ChangeWeatherEvent(Season season)
    {
        switch (season)
        {
            case Season.Spring:
                currentWeatherEvent = springEvents[UnityEngine.Random.Range(0, springEvents.Length)];
                break;
            case Season.Summer:
                currentWeatherEvent = summerEvents[UnityEngine.Random.Range(0, summerEvents.Length)];
                break;
            case Season.Autumn:
                currentWeatherEvent = autumnEvents[UnityEngine.Random.Range(0, autumnEvents.Length)];
                break;
            case Season.Winter:
                currentWeatherEvent = winterEvents[UnityEngine.Random.Range(0, winterEvents.Length)];
                break;
        }
    }

    private void ApplyWeatherEffectsToEntities(AttributeEffects effects)
    {

        foreach (Transform entityTransform in gameManager.entitySpawner.transform)
        {
            Entity entity = entityTransform.GetComponent<Entity>();
            if (entity != null)
            {
                entity.ApplyWeatherEffects(currentWeatherEvent.EntityEffects);
            }
        }

        foreach (Transform resourceTransform in gameManager.resourceSpawner.transform)
        {
            Resource resource = resourceTransform.GetComponent<Resource>();
            if (resource != null)
            {
                resource.ApplyWeatherEffects(currentWeatherEvent.ResourceEffects);
            }
        }

    }

    private void ApplyWeatherEffectsToResources(AttributeEffects effects)
    {
        // Assuming all resources derive from a base class or implement an interface called IResource
        foreach (var resource in FindObjectsOfType<Resource>())
        {
            resource.ApplyWeatherEffects(effects);
        }
    }

    private void OnDestroy()
    {
        TimeManager.OnDateTimeChanged -= HandleTimeChanged;
    }

}