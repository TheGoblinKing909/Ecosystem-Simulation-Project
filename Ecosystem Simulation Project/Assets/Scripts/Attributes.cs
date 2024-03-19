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
    private Movement movement;
    private Resource currentResource;
    [SerializeField] private GameObject deathResource;
    private Rewards rewards;
    private Entity agent;

    protected void OnAwake()
    {
        movement = GetComponent<Movement>();
        if (movement == null) { throw new System.Exception("movement not set in attributes"); }

        rewards = GetComponent<Rewards>();
        if (rewards == null){ throw new System.Exception("Rewards not set in attributes"); }

        agent = GetComponent<Entity>(); 
        if (agent == null) { throw new System.Exception("Human Agent not set in attributes"); }
    }

    void Awake()
    {
        OnAwake();
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

    // FixedUpdate is called once every 0.02 secondsm
    private void FixedUpdate()
    {
        if(currentHealth <= 0) 
        {
            Die();
        }
        Decay();

        if(currentHunger > (maxHunger/2) && currentThirst > (maxThirst/2))
        {
            ModifyHealth(5f * Time.deltaTime);
        }
        ModifyStamina(2 * Time.deltaTime);
    }

    private void Decay()
    {
        currentHunger -= hungerDecayRate * Time.deltaTime;
        currentThirst -= thirstDecayRate * Time.deltaTime;
        if (currentHunger <= 0)
        {
            currentHunger = 0;
            float hungerDamageTaken = -10;
            ModifyHealth(hungerDamageTaken);
        }
        if (currentThirst <=0)
        {
            currentThirst = 0;
            float thirstDamageTaken = -10;
            ModifyHealth(thirstDamageTaken);
        }

    }

    public void ModifyHunger(float amount)
    {
        currentHunger += amount;
        if (currentHunger > maxHunger)
        {
            float hungerGained = currentHunger - maxHunger;
            agent.AddReward(rewards.GetHungerGainedReward(hungerGained));
            currentHunger = maxHunger;
            return;
        }
        else if (currentHunger < 0)
        {
            ModifyHealth(-10);
        }
        agent.AddReward(rewards.GetHungerGainedReward(amount));
    }
    public void ModifyThirst(float amount)
    {
        currentThirst += amount;
        if(currentThirst > maxThirst)
        {
            float thirstGained = currentThirst - maxThirst;
            agent.AddReward(rewards.GetThirstGainedReward(thirstGained));
            currentThirst = maxThirst;
            return;
        }
        else if (currentThirst < 0)
        {
            ModifyHealth(-10);
        }
        agent.AddReward(rewards.GetThirstGainedReward(amount));
    }
    public void ModifyHealth(float amount)
    {
        currentHealth += amount;
        if(currentHealth > maxHealth)
        {
            float healthGained = currentHealth - maxHealth;
            agent.AddReward(rewards.GetHealthGainedReward(healthGained));
            currentHealth = maxHealth;
            return;
        }
        else if(currentHealth <= 0)
        {
            Die();
            return;
        }
        agent.AddReward(rewards.GetThirstGainedReward(amount));
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
    private void Die()
    {
        //Instantiate(deathResource, transform.position, Quaternion.identity);
        Destroy(gameObject);
        agent.AddReward(-10000f);
    }
}