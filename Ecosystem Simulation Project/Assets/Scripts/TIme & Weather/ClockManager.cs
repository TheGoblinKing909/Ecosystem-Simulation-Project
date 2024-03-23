using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ClockManager : MonoBehaviour
{
    [Header("Lighting Settings")]
    public Gradient lightingGradient;
    public RectTransform ClockFace;
    public TextMeshProUGUI Date, Time, Season, Week;
    private float startingRotation;
    // public Light sunlight;
    public float nightIntensity;
    public float dayIntensity;
    public AnimationCurve dayNightCurve;

    private void Awake()
    {
        startingRotation = ClockFace.localEulerAngles.z;
    }

    private void OnEnable()
    {
        TimeManager.OnDateTimeChanged += UpdateDateTime;
    }

    private void OnDisable()
    {
        TimeManager.OnDateTimeChanged -= UpdateDateTime;
    }

    private void UpdateDateTime(DateTime dateTime)
    {

        if (Date != null)
            Date.text = dateTime.DateToString();

        if (Time != null)
            Time.text = dateTime.TimeToString();

        if (Season != null)
            Season.text = dateTime.Season.ToString();

        if (Week != null)
            Week.text = $"WK: {dateTime.CurrentWeek}";

        float t = (float)dateTime.Hour / 24f;

        float newRotation = Mathf.Lerp(0, 360, t);
        ClockFace.localEulerAngles = new Vector3(0, 0, newRotation + startingRotation);

        float dayNightT = dayNightCurve.Evaluate(t);

        // sunlight.intensity = Mathf.Lerp(dayIntensity, nightIntensity, dayNightT);
    }
}
