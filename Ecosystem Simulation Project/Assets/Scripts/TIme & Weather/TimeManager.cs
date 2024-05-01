using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class TimeManager : MonoBehaviour
{
    [Header("Date & Time Settings")]
    [Range(1, 28)]
    public int dateInMonth;
    [Range(1, 4)]
    public int season;
    [Range(1, 1000)]
    public int year;
    [Range(0, 24)]
    public int hour;
    [Range(0, 6)]
    public int minutes;

    public DateTime DateTime;
    public Slider slider; // Assign the Slider reference manually in the prefab
    public Text text; // Assign the Text reference manually in the prefab

    [Header("Tick Settings")]
    public int TickMinutesIncreased = 10;
    public float TimeBetweenTicks = 1;
    private float currentTimeBetweenTicks = 0;

    public static UnityAction<DateTime> OnDateTimeChanged;

    private void Awake()
    {
        DateTime = new DateTime(1, 1, 1);
    }

    private void Start()
    {
        OnDateTimeChanged?.Invoke(DateTime);

        if (slider == null)
        {
            Debug.LogError("Time Scale Slider is not assigned!");
            return;
        }

        slider.onValueChanged.AddListener(UpdateTimeScale);
        Time.timeScale = (float)1.00;
        slider.value = (float)1.00;
    }

    private void Update()
    {
        currentTimeBetweenTicks += Time.deltaTime;

        if (currentTimeBetweenTicks >= TimeBetweenTicks)
        {
            currentTimeBetweenTicks = 0;
            Tick();
        }

        if (slider != null && text != null)
        {
            text.text = slider.value.ToString("F2");
        }
        else
        {
            Debug.Log("Either the Slider or Text gameObjects are not assigned references");
        }

    }

    void UpdateTimeScale(float newValue)
    {
        Time.timeScale = newValue;
    }

    void Tick()
    {
        AdvanceTime();
    }

    void AdvanceTime()
    {
        DateTime.AdvanceMinutes(TickMinutesIncreased);

        OnDateTimeChanged?.Invoke(DateTime);
    }

    public DateTime GetCurrentDateTime()
    {
        return DateTime;
    }

}

[System.Serializable]
public enum Days
{
    NULL = 0,
    Mon = 1,
    Tue = 2,
    Wed = 3,
    Thu = 4,
    Fri = 5,
    Sat = 6,
    Sun = 7
}

[System.Serializable]
public enum Season
{
    Spring = 0,
    Summer = 1,
    Autumn = 2,
    Winter = 3
}

[System.Serializable]
public enum Months
{
    NULL = 0,
    Jan = 1,
    Feb = 2,
    Mar = 3,
    Apr = 4,
    May = 5,
    Jun = 6,
    Jul = 7,
    Aug = 8,
    Sep = 9,
    Oct = 10,
    Nov = 11,
    Dec = 12
}

public struct DateTime
{
    public Days Day;
    public int DayOfMonth;
    public Months Month;
    public int Year;

    public int Hour;
    public int Minutes;

    public int Week;

    public Season Season;

    private int weeksInYear;
    private int weeksInMonth;
    public int WeeksInYear => weeksInYear;
    public int WeeksInMonth => weeksInMonth;

    private int weeksPerSeason => weeksInYear / 4;

    public int CurrentWeek => Week;

    public DateTime(int day, int month, int year)
    {
        Day = (Days)day;
        DayOfMonth = 1;
        Month = (Months)month;
        Year = year;

        Hour = 0;
        Minutes = 0;

        Week = 1;

        Season = (Season)0;

        weeksInYear = 48;
        weeksInMonth = 4;
    }

    public void SetTime(int minutes, int hour, int day, int dayOfMonth, int week, int month, int season, int year)
    {
        Minutes = minutes;
        Hour = hour;
        Day = (Days)day;
        DayOfMonth = dayOfMonth;
        Week = week;
        Month = (Months)month;
        Season = (Season)season;
        Year = year;
    }

    public string DateToString()
    {
        return $"{Day} {Month} {DayOfMonth}, {Year.ToString("D2")}";
    }

    public string TimeToString()
    {
        int adjustedHour = 0;

        if (Hour == 0)
        {
            adjustedHour = 12;
        }
        else if (Hour == 24)
        {
            adjustedHour = 12;
        }
        else if (Hour >= 13)
        {
            adjustedHour = Hour - 12;
        }
        else
        {
            adjustedHour = Hour;
        }

        string AmPm = Hour == 0 || Hour < 12 ? "AM" : "PM";

        return $"{adjustedHour.ToString("D2")}:{Minutes.ToString("D2")} {AmPm}";
    }

    public void AdvanceMinutes(int SecondsToAdvanceBy)
    {
        if (Minutes + SecondsToAdvanceBy > 50)
        {
            int overflowMinutes = Minutes + SecondsToAdvanceBy;
            Minutes = overflowMinutes % 60;
            int numHours = (overflowMinutes) / 60;
            for(;numHours > 0; numHours--) {
                AdvanceHour();
            }
        }
        else
        {
            Minutes += SecondsToAdvanceBy;
        }
    }

    private void AdvanceHour()
    {
        if ((Hour + 1) == 24)
        {
            Hour = 0;
            AdvanceDay();
        }
        else
        {
            Hour++;
        }

    }

    private void AdvanceDay()
    {
        // Increment DayOfMonth along with the day of the week.
        DayOfMonth++;
        if (DayOfMonth > 28) // Assuming a simplified 28-day month for all months
        {
            DayOfMonth = 1; // Reset to the first day of the month
        }

        if (Day + 1 > (Days)7)
        {
            Day = (Days)1;
            AdvanceWeek();
        }
        else
        {
            Day++;
        }

    }

    private void AdvanceWeek()
    {
        Week++;

        if ( (Week - 1) % weeksPerSeason == 0 && Week != 1)
        {
            if (Season == Season.Winter)
                Season = Season.Spring;
            else Season++;
        }

        if ( (Week - 1) % weeksInMonth == 0 && Week != 1)
        {
            AdvanceMonth();
        }
    }

    private void AdvanceMonth()
    {
        if (Month + 1 > (Months)12)
        {
            Month = (Months)1;
            AdvanceYear();
        }
        else
        {
            Month++;
        }
    }

    private void AdvanceYear()
    {
        Year++;
        Week = 1;
    }

}