using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Resource : MonoBehaviour {

    private int AmountPerHarvest = 10; 
    public int HarvestRemaing = 30;
    public int Health = 100;

    public SpriteRenderer spriteRenderer;

    public Color defaultColor = Color.green;
    private Color deadColor = Color.black;

    void Start()
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if(spriteRenderer == null)
        {
            Debug.Log("Sprite Render not attached to component");
        }

        spriteRenderer.color = defaultColor;

        HarvestRemaing = 20;

    }

    public int Harvest() 
    {
        if(HarvestRemaing <= 0)
        {
            return 0;
        }

        int amountHarvested = Mathf.Clamp(HarvestRemaing, 0, AmountPerHarvest);
        HarvestRemaing -= amountHarvested;

        if(HarvestRemaing <= 0 )
        {
            spriteRenderer.color = deadColor;
        }

        return amountHarvested;
    }

}