using UnityEngine;

public class WorldLight : MonoBehaviour
{
    public TimeManager timeManager; // Reference to the TimeManager script

    public Gradient lightingGradient; // Gradient for light color changes

    private void Start()
    {
        // Ensure the necessary components are assigned
        if (timeManager == null)
        {
            Debug.LogError("TimeManager reference not set in WorldLight script.");
            return;
        }

        // Subscribe to the OnDateTimeChanged event using the type name
        TimeManager.OnDateTimeChanged += UpdateLightColor;

        // Initial light color update
        UpdateLightColor(timeManager.GetCurrentDateTime());
    }

    private void OnDestroy()
    {
        // Unsubscribe from the OnDateTimeChanged event when the script is destroyed
        TimeManager.OnDateTimeChanged -= UpdateLightColor;
    }

    private void UpdateLightColor(DateTime dateTime)
    {
        // Calculate a normalized time value (between 0 and 1)
        float normalizedTime = (float)dateTime.Hour / 24f;

        // Sample the gradient using normalized time
        Color lightColor = lightingGradient.Evaluate(normalizedTime);

    }

}