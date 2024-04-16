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

    public void UpdateAgeNumber(float currentValue)
    {
        int ageInt = (int) currentValue;
        ageNumber.text = ageInt.ToString();
    }
}
