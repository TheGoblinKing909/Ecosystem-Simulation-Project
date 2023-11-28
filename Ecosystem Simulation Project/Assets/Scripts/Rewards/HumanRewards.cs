using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Max reward is 1.0

public class HumanRewards : MonoBehaviour
{
    [SerializeField] private Attributes attributes;

    [SerializeField] private float maxHealthGainReward = 1.0f;
    [SerializeField] private float maxHealthLossPunisment = -1.0f;
    [SerializeField] private float maxHungerGainReward = 1.0f;
    [SerializeField] private float maxHungerLossPunishment = -1.0f;
    [SerializeField] private float maxThirstGainReward = 1.0f;
    [SerializeField] private float maxThirstLossPunishment = -1.0f;
    [SerializeField] private float maxAttackReward = 1.0f;
    [SerializeField] private float maxAttackPunishment = -1.0f;


    void Start()
    {
        attributes = GetComponentInParent<Attributes>();
    }
    
    public float GetHealthGainedReward(float healthGained)
    {
        float currentHealth = attributes.currentHealth;

        float newHealth = currentHealth + healthGained;
        if(newHealth > attributes.maxHealth)
        {
            newHealth = attributes.maxHealth;   
        }

        float percentHealthRestored = 1 - (newHealth / attributes.maxHealth);
        return percentHealthRestored * maxHealthGainReward;
    }
    public float GetHealthLossReward(float damageTaken)
    {
        float currentHealth = attributes.currentHealth;
        
        //Death
        if(damageTaken > currentHealth)
        {
            return (maxHealthLossPunisment);
        }

        float percentDamage = (damageTaken / currentHealth);
        return (percentDamage * maxHealthLossPunisment);
    }
    
    public float GetHungerGainedReward(float hungerGained)
    {
        float currentHunger = attributes.currentHunger;

        float newHunger = currentHunger + hungerGained;
        if(newHunger > attributes.maxHunger)
        {
            newHunger = attributes.maxHunger;   
        }

        float percentHungerRestored = 1 - (newHunger / attributes.maxHunger);
        return percentHungerRestored * maxHungerGainReward;
    }
    public float GetHungerLossReward(float hungerLost)
    {
        float currentHunger = attributes.currentHunger;
        
        //no hunger
        if(hungerLost > currentHunger)
        {
            return (maxHungerLossPunishment);
        }

        float percentDamage = (hungerLost / currentHunger);
        return (percentDamage * maxHungerLossPunishment);
    }
    

    public float GetThirstGainedReward(float thirstGained)
    {
        float currentThirst = attributes.currentThirst;

        float newThirst = currentThirst + thirstGained;
        if(newThirst > attributes.maxThirst)
        {
            newThirst = attributes.maxThirst;   
        }

        float percentThirstRestored = 1 - (newThirst / attributes.maxThirst);
        return percentThirstRestored * maxThirstGainReward;
    }
    public float GetThirstLossReward(float thirstLost)
    {
        float currentThirst = attributes.currentThirst;
        
        //no hunger
        if(thirstLost > currentThirst)
        {
            return (maxThirstLossPunishment);
        }

        float percentDamage = (thirstLost / currentThirst);
        return (percentDamage * maxThirstLossPunishment);
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
