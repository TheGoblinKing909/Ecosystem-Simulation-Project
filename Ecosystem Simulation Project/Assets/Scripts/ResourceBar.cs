using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResourceBar : MonoBehaviour
{
    public Slider healthBar;
    public Slider harvestBar;

    public void UpdateHealthBar(float currentValue, float maxValue)
    {
        healthBar.value = currentValue / maxValue;
    }

    public void UpdateHarvestBar(float currentValue, float maxValue)
    {
        harvestBar.value = currentValue / maxValue;
    }
}
