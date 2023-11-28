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

        HarvestRemaining = 20;

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

}