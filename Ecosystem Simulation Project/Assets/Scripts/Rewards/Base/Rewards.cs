using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rewards : MonoBehaviour
{
    private Attributes attributes;

    public static float Max = 1.0f;
    public static float Min = 0.01f;

     private float maxReward = Max;
     private float minReward = Min;
    
     private float punishmentMax = Max * -1f;
     private float punishmentMin = (Min * -1f);

     private float maxAttackReward = 1.0f;
     private float minAttackReward = 0.01f;

     private float maxAttackPunishment = 0;
     private float minAttackPunishment = 0;


    protected void OnAwake()
    {
        attributes = GetComponentInParent<Attributes>();
    }

    void Awake()
    {
        OnAwake();
    }

    public float GetAttributeReward(float amount,float currentValue,float maxValue)
    {
        float newValue = currentValue + amount;
        float result = 0f;
        float rewardAmt;
        switch (amount)
        {
            case > 0:
                if (newValue > attributes.maxHunger)
                {
                    newValue = attributes.maxHunger;
                }

                float percentHungerRestored = (newValue - currentValue) / (maxValue - currentValue);
                rewardAmt = percentHungerRestored * maxReward;
                result = GetRewardClamped(rewardAmt);
                return result;
            case < 0:
                if (newValue < 0)
                {
                    newValue = 0;
                }

                float percentHungerLost = Mathf.Abs(amount) / currentValue;
                rewardAmt = percentHungerLost * punishmentMax;
                result = GetRewardClamped(rewardAmt);
                return result;
            default:
                return result;
        }
        
    }

    public float GetRewardClamped(float amount)
    {
        switch (amount)
        {
            case > 0f:
                if (amount > maxReward)
                {
                    return maxReward;
                }
                if(amount < minReward)
                {
                    return minReward;
                }
                return amount;
            case < 0f:
                if (amount < punishmentMax)
                {
                    return punishmentMax;
                }
                if (amount > punishmentMin)
                {
                    return punishmentMin;
                }
                return amount;
            default:
                return 0f;
        }

    }
    public float GetAttackReward()
    {
        return maxAttackReward;
    }
    public float GetAttackPunishment()
    {
        return maxAttackPunishment;
    }
}
