using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Max reward is 1.0

public class BirdRewards : MonoBehaviour
{
    [SerializeField] private BirdAttributes attributes;

    [SerializeField] private float maxHealthGainReward = 1.0f;
    [SerializeField] private float minHealthGainReward = 0.01f;

    [SerializeField] private float maxHealthLossPunishment = -1.0f;
    [SerializeField] private float minHealthLossPunishment = -0.01f;

    [SerializeField] private float maxHungerGainReward = 2.0f;
    [SerializeField] private float minHungerGainReward = 0.01f;

    [SerializeField] private float maxHungerLossPunishment = -1.0f;
    [SerializeField] private float minHungerLossPunishment = -0.01f;

    [SerializeField] private float maxThirstGainReward = 0.7f;
    [SerializeField] private float minThirstGainReward = 0.01f;

    [SerializeField] private float maxThirstLossPunishment = -1.0f;
    [SerializeField] private float minThirstLossPunishment = -0.01f;

    [SerializeField] private float maxAttackReward = 0.01f;
    [SerializeField] private float minAttackReward = 0.01f;

    [SerializeField] private float maxAttackPunishment = -0.01f;
    [SerializeField] private float minAttackPunishment = -0.01f;



    void Start()
    {
        attributes = GetComponentInParent<BirdAttributes>();
    }

    public float GetHealthGainedReward(float healthGained)
    {
        float currentHealth = attributes.currentHealth;

        float newHealth = currentHealth + healthGained;
        if (newHealth > attributes.maxHealth)
        {
            newHealth = attributes.maxHealth;
        }

        float percentHealthRestored = 1 - (newHealth / attributes.maxHealth);
        return Mathf.Clamp((percentHealthRestored * maxHealthGainReward), minHealthGainReward, maxHealthGainReward);
    }
    public float GetHealthLossReward(float damageTaken)
    {
        float currentHealth = attributes.currentHealth;

        //Death
        if (damageTaken > currentHealth)
        {
            return (maxHealthLossPunishment);
        }

        float percentDamage = (damageTaken / currentHealth);
        return Mathf.Clamp((percentDamage * maxHealthLossPunishment), minHealthLossPunishment, maxHealthLossPunishment);
    }

    public float GetHungerGainedReward(float hungerGained)
    {
        float currentHunger = attributes.currentHunger;

        float newHunger = currentHunger + hungerGained;
        if (newHunger > attributes.maxHunger)
        {
            newHunger = attributes.maxHunger;
        }

        float percentHungerRestored = 1 - (newHunger / attributes.maxHunger);
        return Mathf.Clamp((percentHungerRestored * maxHungerGainReward), minHungerGainReward, maxHungerGainReward);
    }
    public float GetHungerLossReward(float hungerLost)
    {
        float currentHunger = attributes.currentHunger;

        //no hunger
        if (hungerLost > currentHunger)
        {
            return (maxHungerLossPunishment);
        }

        float percentDamage = (hungerLost / currentHunger);
        return Mathf.Clamp((percentDamage * maxHungerLossPunishment), minHungerLossPunishment, maxHungerLossPunishment);
    }


    public float GetThirstGainedReward(float thirstGained)
    {
        float currentThirst = attributes.currentThirst;

        float newThirst = currentThirst + thirstGained;
        if (newThirst > attributes.maxThirst)
        {
            newThirst = attributes.maxThirst;
        }

        float percentThirstRestored = 1 - (newThirst / attributes.maxThirst);
        return Mathf.Clamp((percentThirstRestored * maxThirstGainReward), minThirstGainReward, maxThirstGainReward);
    }
    public float GetThirstLossReward(float thirstLost)
    {
        float currentThirst = attributes.currentThirst;

        //no hunger
        if (thirstLost > currentThirst)
        {
            return (maxThirstLossPunishment);
        }

        float percentDamage = (thirstLost / currentThirst);
        return Mathf.Clamp((percentDamage * maxThirstLossPunishment), minThirstLossPunishment, maxThirstLossPunishment);
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
