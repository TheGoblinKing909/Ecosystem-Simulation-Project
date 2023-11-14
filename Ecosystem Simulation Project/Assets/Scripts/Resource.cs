using System.Collections;
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

    private void Start() {
        HarvestRemaining = HarvestMax;
        HealthRemaining = HealthMax;
    }

    private void FixedUpdate() {
        if (HealthRemaining <= 0) {
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

    public float Harvest() {
        float amountHarvested = Mathf.Clamp(HarvestRemaining, 0f, AmountPerHarvest);
        HarvestRemaining -= amountHarvested;
        return amountHarvested;
    }

}