using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Resource : MonoBehaviour {

    private int AmountPerHarvest = 10; 
    public int HarvestRemaing = 100;
    public int Health = 100;

    public int Harvest() {
        int amountHarvested = Mathf.Clamp(HarvestRemaing, 0, AmountPerHarvest);
        HarvestRemaing -= amountHarvested;
        return amountHarvested;
    }

}