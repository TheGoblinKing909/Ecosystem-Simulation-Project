using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using weather.effects;

public enum ResourceType
{
    None = 0,
    BlueBerry = 1,
    BlueFlower,
    BushBerry,
    Grass,
    OrangeFlower,
    PinkBud,
    PinkFlower,
    RedBerry,
    DeadEntity
}

public class Resource : MonoBehaviour {
    public ResourceType resourceType = ResourceType.None;
    public float AmountPerHarvest = 10;

    public float HealthMax = 100f;
    public float HarvestMax = 100f;

    public float HealthRemaining;
    public float HarvestRemaining;

    public float HealthRecovery = 0.5f;
    public float HarvestRecovery = 0.5f;

    public float Thermo_min = 0.45f;
    public float Thermo_max = 0.75f;
    private WeatherManager weatherManager;
    private ResourceBar resourceBar;

    public SpriteRenderer spriteRenderer;

    public Color defaultColor = Color.green;
    private Color deadColor = Color.black;

    public int PrefabIndex;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if(spriteRenderer == null) {
            Debug.LogError("SpriteRenderer not attached to component");
        } else {
            spriteRenderer.color = defaultColor;
        }

        weatherManager = FindObjectOfType<WeatherManager>();

        resourceBar = GetComponentInChildren<ResourceBar>();
        if (resourceBar == null) { throw new System.Exception("Resource Bar not set in attributes"); }
    }

    private void FixedUpdate()
    {
        float temperatureEffect = CalculateTemperatureEffect();

        if (HealthRemaining <= 0) {
            DestroyResource();
        }

        HealthRemaining += (HealthRecovery * temperatureEffect) * Time.deltaTime;
        HealthRemaining = Mathf.Min(HealthRemaining, HealthMax);

        HarvestRemaining += (HarvestRecovery * temperatureEffect) * Time.deltaTime;
        HarvestRemaining = Mathf.Min(HarvestRemaining, HarvestMax);

        resourceBar.UpdateHealthBar(HealthRemaining, HealthMax);
        resourceBar.UpdateHarvestBar(HarvestRemaining, HarvestMax);
    }

    public void MaxRemaining()
    {
        HarvestRemaining = HarvestMax;
        HealthRemaining = HealthMax;
    }

    public float Harvest()
    {
        if(HarvestRemaining <= 0) {
            return 0;
        }

        float amountHarvested = Mathf.Clamp(HarvestRemaining, 0, AmountPerHarvest);
        HarvestRemaining -= amountHarvested;

        return amountHarvested;
    }

    private void DestroyResource()
    {
        if (TryGetComponent<Shelter>(out Shelter shelter)) {
            foreach (var entity in shelter.shelteredEntities) {
                shelter.ExitShelter(entity);
            }
        }
        Destroy(gameObject);
    }

    private float CalculateTemperatureEffect()
    {
        if (weatherManager != null)
        {
            float currentTemperature = weatherManager.GetCurrentTemperature();
            currentTemperature = currentTemperature / 100.0f;

            if (currentTemperature < Thermo_min || currentTemperature > Thermo_max)
            {
                float difference = Mathf.Max(currentTemperature - Thermo_max, Thermo_min - currentTemperature);
                difference *= 100;
                float effect = Mathf.Pow(difference, 1.75f) / 300.0f;
                return Mathf.Max(0, 1 - effect);  // Ensure the result is never negative
            }

            return 1f;
        }
        return 1f;
    }

    public void ApplyWeatherEffects(AttributeEffects effects)
    {
        HealthRemaining += effects.HealthEffect;
        HealthRemaining = Mathf.Clamp(HealthRemaining, 0, HealthMax);

        HarvestRemaining += effects.AmountEffect;
        HarvestRemaining = Mathf.Clamp(HarvestRemaining, 0, HarvestMax);

        Debug.Log($"Weather effects applied to Resource: {effects.HealthEffect} to Health, {effects.AmountEffect} to Harvest Amount.");
    }

}



















/* using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Resource : MonoBehaviour {

    private float AmountPerHarvest = 10;

    public float HarvestMax = 100f;
    public float HealthMax = 100f;

    public float HarvestRemaining;
    public float HealthRemaining;

    public float HarvestRecovery = 2f;
    public float HealthRecovery = 2f;

    private void FixedUpdate() {
        if (HealthRemaining <= 0) {
            if (TryGetComponent<Shelter>(out Shelter shelter)) {
                for (int i = 0; i < shelter.shelteredEntities.Count; i++) {
                    shelter.ExitShelter(shelter.shelteredEntities[i]);
                }
            }
            Destroy(gameObject);
        }
        if (HarvestRemaining < HarvestMax) {
            HarvestRemaining += HarvestRecovery * Time.deltaTime;
            if (HarvestRemaining > HarvestMax) {
                HarvestRemaining = HarvestMax;
            }
        }
        if (HealthRemaining < HealthMax) {
            HealthRemaining += HealthRecovery * Time.deltaTime;
            if (HealthRemaining > HealthMax) {
                HealthRemaining = HealthMax;
            }
        }
    }

    public SpriteRenderer spriteRenderer;

    public Color defaultColor = Color.green;
    private Color deadColor = Color.black;

    void Start()
    {
        HarvestRemaining = HarvestMax;
        HealthRemaining = HealthMax;
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if(spriteRenderer == null)
        {
            Debug.Log("Sprite Render not attached to component");
        }

        spriteRenderer.color = defaultColor;

        // HarvestRemaining = 20;

    }

    public float Harvest() 
    {
        if(HarvestRemaining <= 0)
        {
            return 0;
        }

        float amountHarvested = Mathf.Clamp(HarvestRemaining, 0, AmountPerHarvest);
        HarvestRemaining -= amountHarvested;

        // if(HarvestRemaining <= 0 )
        // {
        //     spriteRenderer.color = deadColor;
        // }

        return amountHarvested;
    }

} */