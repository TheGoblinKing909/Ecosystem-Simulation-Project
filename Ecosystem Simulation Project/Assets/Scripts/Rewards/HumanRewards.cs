using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Max reward is 1.0

public class HumanRewards : MonoBehaviour
{
    [SerializeField] private Attributes attributes;
    void Start()
    {
        attributes = GetComponentInParent<Attributes>();
    }
    public float GetHungerGainedReward(float hungerGained)
    {
        float maxReward = 0.5f;
        float currentHunger = attributes.currentHunger;

        float newHunger = currentHunger + hungerGained;
        if(newHunger > attributes.maxHunger)
        {
            newHunger = attributes.maxHunger;   
        }

        float percentHungerRestored = 1 - (newHunger / attributes.maxHunger);
        Debug.Log("Reward");
        return percentHungerRestored * maxReward;
    }
    
    public float GetHealthGainedReward(float healthGained)
    {
        float maxReward = 0.8f;
        float currentHealth = attributes.currentHealth;

        float newHealth = currentHealth + healthGained;
        if(newHealth > attributes.maxHealth)
        {
            newHealth = attributes.maxHealth;   
        }

        float percentHealthRestored = 1 - (newHealth / attributes.maxHealth);
        return percentHealthRestored * maxReward;
    }


    public float GetThirstGainedReward(float thirstGained)
    {
        float maxReward = 0.5f;
        float currentThirst = attributes.currentThirst;

        float newThirst = currentThirst + thirstGained;
        if(newThirst > attributes.maxThirst)
        {
            newThirst = attributes.maxThirst;   
        }

        float percentThirstRestored = 1 - (newThirst / attributes.maxThirst);
        return percentThirstRestored * maxReward;
    }

    public float GetHealthLossReward(float damageTaken)
    {
        float maxReward = -0.8f;
        float currentHealth = attributes.currentHealth;
        
        //Death
        if(damageTaken > currentHealth)
        {
            return (maxReward);
        }

        float percentDamage = (damageTaken / currentHealth);
        return (percentDamage * maxReward);
    }
    
    public float GetHungerLossReward(float hungerLost)
    {
        float maxReward = -0.5f;
        float currentHunger = attributes.currentHunger;
        
        //no hunger
        if(hungerLost > currentHunger)
        {
            return (maxReward);
        }

        float percentDamage = (hungerLost / currentHunger);
        Debug.Log(percentDamage * maxReward + "hunger punisment");
        return (percentDamage * maxReward);
    }
    
    public float GetThirstLossReward(float thirstLost)
    {
        float maxReward = -0.5f;
        float currentThirst = attributes.currentThirst;
        
        //no hunger
        if(thirstLost > currentThirst)
        {
            return (maxReward);
        }

        float percentDamage = (thirstLost / currentThirst);
        Debug.Log(percentDamage * maxReward + "thirst punisment");
        return (percentDamage * maxReward);
    }
}
