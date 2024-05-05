using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AttributeBar : MonoBehaviour
{
    public Slider healthBar;
    public Slider staminaBar;
    public Slider hungerBar;
    public Slider thirstBar;
    public TMPro.TextMeshProUGUI ageNumber;
    public int ageDay;
    public int ageMonth;
    public int ageYear;

    public void UpdateHealthBar(float currentValue, float maxValue)
    {
        healthBar.value = currentValue / maxValue;
    }

    public void UpdateStaminaBar(float currentValue, float maxValue)
    {
        staminaBar.value = currentValue / maxValue;
    }

    public void UpdateHungerBar(float currentValue, float maxValue)
    {
        hungerBar.value = currentValue / maxValue;
    }

    public void UpdateThirstBar(float currentValue, float maxValue)
    {
        thirstBar.value = currentValue / maxValue;
    }

    public void UpdateAgeNumber()
    {
        ageDay++;
        if (ageDay >= 28)
        {
            ageDay = 0;
            ageMonth++;
            if (ageMonth >= 12)
            {
                ageMonth = 0;
                ageYear++;
            }
        }
        ageNumber.text = "D:" + ageDay + " M:" + ageMonth + " Y:" + ageYear;
    }

    public void SetAgeNumber(int day, int month, int year)
    {
        ageDay = day;
        ageMonth = month;
        ageYear = year;
        ageNumber.text = "D:" + ageDay + " M:" + ageMonth + " Y:" + ageYear;
    }
}
