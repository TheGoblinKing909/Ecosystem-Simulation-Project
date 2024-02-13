using UnityEngine;

public class WeatherManager : MonoBehaviour
{
    public static Weather currentWeather = Weather.Sunny;

    private void OnEnable()
    {
        TimeManager.OnDateTimeChanged += GetRandomWeather;
    }

    private void OnDisable()
    {
        TimeManager.OnDateTimeChanged -= GetRandomWeather;
    }

    private void GetRandomWeather(DateTime dateTime)
    {
        if (dateTime.Hour == 0 && dateTime.Minutes == 0)
        {
            currentWeather = (Weather)Random.Range(0, (int)Weather.MAX_WEATHER_AMOUNT + 1);
        }
    }
}

public enum Weather
{
    Sunny = 0,
    Raining = 1,
    MAX_WEATHER_AMOUNT = Raining
}