using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Integrations.Match3;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class Attributes : MonoBehaviour
{
    public float initialHealth = 100f;
    public float initialStamina = 100f;
    public float initialHunger = 100f;
    public float initialThirst = 100f;
    public float initialAgility = 2f;
    public float initialAttack = 10f;
    public float initialSize = 5f;

    public float maxAge = 100f;
    public float primeAge = 50f;
    public float ageDelay = 10f;
    public float hungerDecayRate = 2f;
    public float thirstDecayRate = 2f;

    public float maxHealth;
    public float maxStamina;
    public float maxHunger;
    public float maxThirst;

    public float currentHealth;
    public float currentStamina;
    public float currentHunger;
    public float currentThirst;
    public float currentAge;

    public float agility;
    public float attack;
    public float size;

    public Shelter shelter = null;
    public Movement movement;
    public Resource currentResource;
    public GameObject deathResource;
    private Rewards rewards;
    private Entity agent;
    private AttributeBar attributeBar;

    // Thermocomfort attributes
    public float thermo_min = 0.45f;
    public float thermo_max = 0.75f;
    private WeatherManager weatherManager;

    public float ageTime;

    public EntityType entityType = EntityType.Diurnal;

    public int prefabIndex;
    public bool isLoaded = false;

    protected void OnAwake()
    {
        movement = GetComponent<Movement>();
        if (movement == null) { throw new System.Exception("movement not set in attributes"); }

        rewards = GetComponent<Rewards>();
        if (rewards == null) { throw new System.Exception("Rewards not set in attributes"); }

        agent = GetComponent<Entity>();
        if (agent == null) { throw new System.Exception("Human Agent not set in attributes"); }

        attributeBar = GetComponentInChildren<AttributeBar>();
        if (attributeBar == null) { throw new System.Exception("Attribute Bar not set in attributes"); }
    }

    public void Awake()
    {
        OnAwake();
    }

    protected void OnStart()
    {
        EpisodeBegin();
        weatherManager = FindObjectOfType<WeatherManager>();
    }

    public void Start()
    {
        OnStart();
    }

    public void EpisodeBegin()
    {
        maxHealth = initialHealth;
        maxStamina = initialStamina;
        maxHunger = initialHunger;
        maxThirst = initialThirst;
        agility = initialAgility;
        attack = initialAttack;
        size = initialSize;
        if(!isLoaded)
        {
            currentHealth = maxHealth;
            currentStamina = maxStamina;
            currentHunger = maxHunger;
            currentThirst = maxThirst;
            currentAge = 0;
            ageTime = 0;
        }
    }

    public void FixedUpdate()
    {
        if (currentHealth <= 0) 
        {
            Die();
        }

        float worldTemperature = GetWorldTemperatureNormalized();
        float thermocomfortEffect = CalculateThermocomfortEffect(worldTemperature);

        DecayHealth(thermocomfortEffect);

        if (currentHunger > (maxHunger / 2) && currentThirst > (maxThirst / 2))
        {
            ModifyHealth(5f * Time.deltaTime * (2 - thermocomfortEffect));
        }

        ModifyStamina(2 * Time.deltaTime * (2 - thermocomfortEffect));

        ageTime += Time.deltaTime;
        if (ageTime > ageDelay) 
        {
            IncreaseAge();
            attributeBar.UpdateAgeNumber(currentAge);
            ageTime = 0;
        }

        if (shelter != null && currentHunger > 0 && currentThirst > 0) {
            float recoveryAmount = shelter.recoveryRate * Time.deltaTime;
            ModifyHealth(recoveryAmount);
            ModifyStamina(recoveryAmount);
            agent.AddReward(rewards.GetHealthGainedReward(recoveryAmount));
        }

        attributeBar.UpdateHealthBar(currentHealth, maxHealth);
        attributeBar.UpdateStaminaBar(currentStamina, maxStamina);
        attributeBar.UpdateHungerBar(currentHunger, maxHunger);
        attributeBar.UpdateThirstBar(currentThirst, maxThirst);
    }

    private float GetWorldTemperatureNormalized()
    {
        if (weatherManager != null)
        {
            float currentTemperature = weatherManager.GetCurrentTemperature();
            return currentTemperature / 100.0f;
        }
        return 0.0f; // Default value if weatherManager is null or not set
    }

    private float CalculateThermocomfortEffect(float worldTemperature)
    {
        if (worldTemperature < thermo_min || worldTemperature > thermo_max)
        {
            float difference = Mathf.Max(worldTemperature - thermo_max, thermo_min - worldTemperature);
            return 1 + (difference * difference);
        }
        return 1;
    }

    private void DecayHealth(float thermocomfortEffect)
    {
        currentHunger -= hungerDecayRate * Time.deltaTime * thermocomfortEffect;
        currentThirst -= thirstDecayRate * Time.deltaTime * thermocomfortEffect;
        if (currentHunger <= 0)
        {
            currentHunger = 0;
            float hungerDamageTaken = 10 * Time.deltaTime;
            currentHealth -= hungerDamageTaken;
        }
        if (currentThirst <= 0)
        {
            currentThirst = 0;
            float thirstDamageTaken = 10 * Time.deltaTime;
            currentHealth -= thirstDamageTaken;
        }
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
        if (currentThirst <= 0)
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
    private void IncreaseAge()
    {
        currentAge++;
        if (currentAge <= primeAge)
        {
            float ageScaler = (currentAge * currentAge) / (primeAge * primeAge);

            AgeToPrime(ref currentHealth, ref maxHealth, initialHealth, ageScaler);
            AgeToPrime(ref currentStamina, ref maxStamina, initialStamina, ageScaler);
            AgeToPrime(ref currentHunger, ref maxHunger, initialHunger, ageScaler);
            AgeToPrime(ref currentThirst, ref maxThirst, initialThirst, ageScaler);

            agility = initialAgility + (initialAgility * ageScaler);
            attack = initialAttack + (initialAttack * ageScaler);
            size = initialSize + (initialSize * ageScaler);
        }
        else if (currentAge <= maxAge)
        {
            float ageScaler = ((currentAge - primeAge) * (currentAge - primeAge)) / ((maxAge - primeAge) * (maxAge - primeAge));

            AgePastPrime(ref currentHealth, ref maxHealth, initialHealth, ageScaler);
            AgePastPrime(ref currentStamina, ref maxStamina, initialStamina, ageScaler);
            AgePastPrime(ref currentHunger, ref maxHunger, initialHunger, ageScaler);
            AgePastPrime(ref currentThirst, ref maxThirst, initialThirst, ageScaler);

            agility = (initialAgility * 2) - (initialAgility * ageScaler);
            attack = (initialAttack * 2) - (initialAttack * ageScaler);
            size = (initialSize * 2) - (initialSize * ageScaler);
        }
        else
        {
            Die();
        }
    }

    private void AgeToPrime(ref float current, ref float max, float initial, float scaler)
    {
        float previousMax = max;
        max = initial + (initial * scaler);
        current += max - previousMax;
    }

    private void AgePastPrime(ref float current, ref float max, float initial, float scaler)
    {
        float previousMax = max;
        max = (initial * 2) - (initial * scaler);
        current += max - previousMax;
    }

    private void Die()
    {
        if (shelter != null)
        {
            shelter.ExitShelter(gameObject);
        }

        GameObject deathInstance = Instantiate(deathResource, transform.position, Quaternion.identity);
        GameManager gameManager = FindObjectOfType<GameManager>();

        if (gameManager != null) {

            Transform resourceManagerTransform = gameManager.transform.Find("ResourceManager");

            if (resourceManagerTransform != null) {
                deathInstance.transform.parent = resourceManagerTransform;
                Resource res = deathInstance.GetComponent<Resource>();
                res.MaxRemaining();
                res.PrefabIndex = 1;
            }
        }

        Destroy(gameObject);
        agent.AddReward(-10f);
    }

}