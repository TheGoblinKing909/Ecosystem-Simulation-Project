using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShelterBar : MonoBehaviour
{
    public Slider capacityBar;

    public void UpdateCapacityBar(float currentValue, float maxValue)
    {
        capacityBar.value = 1 - (currentValue / maxValue);
    }
}
