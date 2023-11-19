using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Integrations.Match3;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class Attributes : MonoBehaviour
{
    // Player attributes

    public float maxHealth = 100f;
    public float maxStamina = 100f;
    public float maxHunger = 100f;
    public float maxThirst = 100f;

    public float hungerDecayRate = 2f; // Health decay rate per second
    public float thirstDecayRate = 2f;

    public float currentHealth;
    public float currentStamina;
    public float currentHunger;
    public float currentThirst;

    public float agility;
    public float attack;
    public float size;    

    // public Rigidbody2D rigidbody2D;
    public Movement movement;
    public Resource currentResource;
    public GameObject deathResource;
    private HumanRewards humanRewards;

    private HumanAgent humanAgent;

    public void Awake()
    {
        movement = GetComponent<Movement>();
        if (movement == null) throw new System.Exception("movement not set in attributes");

        humanRewards = GetComponent<HumanRewards>();
        if (humanRewards == null) throw new System.Exception("HumanRewards not set in attributes");

        humanAgent = GetComponent<HumanAgent>();
        if (humanAgent == null) throw new System.Exception("Human Agent not set in attributes");
    }

    public void EpisodeBegin()
    {
        // reset attributes
        currentHealth = maxHealth;
        currentHunger = maxHunger;
        currentThirst = maxThirst;
        currentStamina = maxStamina;
    }
    private void Start()
    {
        EpisodeBegin();
    }

    // FixedUpdate is called once every 0.02 seconds
    private void FixedUpdate()
    {
        if(currentHealth <= 0) 
        {
            Die();
        }
        DecayHealth();

        if(currentHunger > (maxHunger/2) && currentThirst > (maxThirst/2))
        {
            Heal(5f * Time.deltaTime);
        }
        ModifyStamina(2 * Time.deltaTime);
    }

    private void DecayHealth()
    {
        currentHunger -= hungerDecayRate * Time.deltaTime;
        currentThirst -= thirstDecayRate * Time.deltaTime;
        if (currentHunger <= 0)
        {
            currentHunger = 0;
            float hungerDamageTaken = 10 * Time.deltaTime;
            humanAgent.AddReward(humanRewards.GetHealthLossReward(hungerDamageTaken));
            currentHealth -= hungerDamageTaken;
        }
        if (currentThirst <=0)
        {
            currentThirst = 0;
            float thirstDamageTaken = 10 * Time.deltaTime;
            humanAgent.AddReward(humanRewards.GetHealthLossReward(thirstDamageTaken));
            currentHealth -= thirstDamageTaken;
        }

    }
    public void Eat(float amount)
    {
        currentHunger += amount;
        if(currentHunger > maxHunger)
        {
            float hungerGained = currentHunger - maxHunger;
            humanAgent.AddReward(humanRewards.GetHungerGainedReward(hungerGained));
            currentHunger = maxHunger;
            return;
        }
        humanAgent.AddReward(humanRewards.GetHungerGainedReward(amount));
    }
    public void Drink(float amount)
    {
        currentThirst += amount;
        if(currentThirst > maxThirst)
        {
            float thirstGained = currentThirst - maxThirst;
            humanAgent.AddReward(humanRewards.GetThirstGainedReward(thirstGained));
            currentThirst = maxThirst;
            return;
        }
        humanAgent.AddReward(humanRewards.GetThirstGainedReward(amount));
    }
    public void Heal(float amount)
    {
        currentHealth += amount;
        if(currentHealth > maxHealth)
        {
            float healthGained = currentHealth - maxHealth;
            humanAgent.AddReward(humanRewards.GetHealthGainedReward(healthGained));
            currentHealth = maxHealth;
        }
        humanAgent.AddReward(humanRewards.GetThirstGainedReward(amount));
    }

    public void ModifyStamina(float amount)
    {
        currentStamina += amount;
        if(currentStamina > maxStamina)
        {
            currentStamina = maxStamina;
        }
        else if(currentStamina < 0)
        {
            currentStamina = 0;
        }
    }
    public void HarvestResource(GameObject resource)
    {
        if (currentStamina >= 10)
        {
            ModifyStamina(-10);
            Resource harvestItem = resource.GetComponent<Resource>();
            float harvestAmount = harvestItem.Harvest();
            Eat(harvestAmount);
        }
    }
    public void HarvestWater()
    {
        if (currentStamina >= 10)
        {
            ModifyStamina(-10);
            Drink(10);
        }
    }
    public void AttackEntity(GameObject entity)
    {
        Debug.Log(gameObject.name + " attacked entity " + entity.name);
        Attributes targetAttributes = entity.GetComponent<Attributes>();
        if(currentStamina >= 10)
        {
            ModifyStamina(-10f);
            if(Random.Range(1,10) >= targetAttributes.agility)
            {
                targetAttributes.currentHealth -= attack;
            }
        }
        if(currentHealth >= targetAttributes.currentHealth)
        {
            humanAgent.AddReward(10f);
        }
        else
        {
            humanAgent.AddReward(-10f);
        }
    }
    public void AttackResource(GameObject resource)
    {
        Debug.Log(gameObject.name + " attacked resource " + resource.name);
        if(currentStamina >= 10)
        {
            ModifyStamina(-10f);
            Resource targetResource = resource.GetComponent<Resource>();
            targetResource.HealthRemaining -= attack;
        }
    }
    private void Die()
    {
        //Instantiate(deathResource, transform.position, Quaternion.identity);
        Destroy(gameObject);
        humanAgent.AddReward(-10000f);
    }
}