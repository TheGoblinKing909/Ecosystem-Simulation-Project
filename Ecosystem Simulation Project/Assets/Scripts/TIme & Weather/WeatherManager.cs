using UnityEngine;
using System;
using TMPro;

public class WeatherManager : MonoBehaviour
{
    public TimeManager timeManager;
    public TextMeshProUGUI Temperature;
    private int lastHour = -1;
    private int temperature;

    private void Start()
    {
        if (timeManager == null)
        {
            Debug.LogError("TimeManager reference not assigned!");
            return;
        }

        TimeManager.OnDateTimeChanged += HandleTimeChanged;
    }

    public int GetCurrentTemperature()
    {
        return temperature;
    }

    private void HandleTimeChanged(DateTime currentDateTime)
    {
        // Check if the hour has changed since the last update
        if (currentDateTime.Hour != lastHour)
        {
            lastHour = currentDateTime.Hour; // Update the lastHour to the current hour
            float currentTemperature = CalculateCurrentTemperature(currentDateTime);
            // Debug.Log($"Current Temperature: {currentTemperature}°F");

            if (Temperature != null)
            {
                // Round the temperature for display
                int roundedTemperature = Mathf.RoundToInt(currentTemperature);
                Temperature.text = roundedTemperature.ToString() + "°F"; // Append "°F" for clarity
            }
        }
    }

    float CalculateCurrentTemperature(DateTime dateTime)
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

        // Apply the seasonal offset
        temperature += GetSeasonalOffset(timeManager.GetCurrentDateTime().Season);

        return temperature;
    }

    // Helper method to determine the seasonal offset
    float GetSeasonalOffset(Season season)
    {
        switch (season)
        {
            case Season.Spring:
                return 5f; // Spring serves as a baseline with +5 offset
            case Season.Summer:
                return 20f; // Summer with +20 offset
            case Season.Autumn:
                return 10f; // Autumn with +10 offset
            case Season.Winter:
                return -20f; // Winter with -20 offset
            default:
                return 0f; // No offset if the season is somehow unspecified
        }
    }

    private void OnDestroy()
    {
        TimeManager.OnDateTimeChanged -= HandleTimeChanged;
    }

}