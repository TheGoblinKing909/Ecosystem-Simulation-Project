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
    public float hungerDecayRate = 0.5f;
    public float thirstDecayRate = 0.5f;

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
        if (agent == null) { throw new System.Exception("Agent not set in attributes"); }

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

        Decay(thermocomfortEffect);

        if (currentHunger > (maxHunger / 2) && currentThirst > (maxThirst / 2))
        {
            ModifyHealth(3f * Time.deltaTime * thermocomfortEffect);
        }

        ModifyStamina(3f * Time.deltaTime * thermocomfortEffect);

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
            var reward = rewards.GetAttributeReward(recoveryAmount, currentHealth, maxHealth);
            if (reward > Rewards.Max)
            {
                Debug.Log($" Reward = {reward} > MaxReward = {Rewards.Max}");
            }
            agent.AddReward(reward);
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
        return 0.0f;
    }

    private float CalculateThermocomfortEffect(float worldTemperature)
    {
        if (worldTemperature < thermo_min || worldTemperature > thermo_max)
        {
            float difference = Mathf.Max(worldTemperature - thermo_max, thermo_min - worldTemperature);
            difference *= 100;
            float effect = Mathf.Pow(difference, 1.75f) / 300.0f;
            return Mathf.Max(0, 1 - effect);  // Ensure the result is never negative
        }
        return 1f;
    }

    private void Decay(float thermocomfortEffect)
    {
        ModifyHunger( - (hungerDecayRate * Time.deltaTime * (1 + (1 - thermocomfortEffect) )) );
        ModifyThirst( - (thirstDecayRate * Time.deltaTime * (1 + (1 - thermocomfortEffect) )) );
    }

    public void ModifyHunger(float amount)
    {
        float premodifiedHunger = currentHunger;
        currentHunger += amount;
        var reward = rewards.GetAttributeReward(amount, premodifiedHunger, maxHunger);
        if (currentHunger > maxHunger)
        {
            float hungerGained = maxHunger - currentHunger;
            reward = rewards.GetAttributeReward(hungerGained, premodifiedHunger, maxHunger);
            if (reward > Rewards.Max)
            {
                Debug.Log($" Reward = {reward} > MaxReward = {Rewards.Max}");
            }
            agent.AddReward(reward);
            currentHunger = maxHunger;
            return;
        }
        else if (currentHunger <= 0)
        {
            currentHunger = 0;
            float hungerDamageTaken = -0.5f * Time.deltaTime;
            ModifyHealth(hungerDamageTaken);
            return;
        }
        if (amount < 0)
        {
            if (currentHunger < (maxHunger / 4))
            {
                agent.AddReward(reward);
            }
            return;
        }
        agent.AddReward(reward);
    }

    public void ModifyThirst(float amount)
    {
        var reward = rewards.GetAttributeReward(amount, currentThirst, maxThirst);
        currentThirst += amount;
        if(currentThirst > maxThirst)
        {
            float thirstGained = currentThirst - maxThirst;
            reward = rewards.GetAttributeReward(thirstGained, currentThirst, maxThirst);
            if (reward > Rewards.Max)
            {
                Debug.Log($" Reward = {reward} > MaxReward = {Rewards.Max}");
            }
            agent.AddReward(reward);
            currentThirst = maxThirst;
            return;
        }
        else if (currentThirst <= 0)
        {
            currentThirst = 0;
            float thirstDamageTaken = -0.5f * Time.deltaTime;
            ModifyHealth(thirstDamageTaken);
            return;
        }

        if (amount < 0)
        {
            if (currentThirst < (maxThirst / 4))
            {
                agent.AddReward(reward);
            }
            return;
        }
        agent.AddReward(reward);
    }

    public void ModifyHealth(float amount)
    {

        var reward = rewards.GetAttributeReward(amount, currentHealth, maxHealth);
        currentHealth += amount;
        if(currentHealth > maxHealth)
        {
            float healthGained = currentHealth - maxHealth;
            reward = rewards.GetAttributeReward(amount, currentHealth, maxHealth);
            if (reward > Rewards.Max)
            {
                Debug.Log($" Reward = {reward} > MaxReward = {Rewards.Max}");
            }
            agent.AddReward(reward);
            currentHealth = maxHealth;
            return;
        }
        else if(currentHealth <= 0)
        {
            Die();
            return;
        }

        if(amount < 0)
        {
            if (currentHealth < (maxHealth / 2))
            {
                agent.AddReward(reward);
            }
            return;
        }
        agent.AddReward(reward);


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

        agent.AddReward(-5f);
        agent.EndEpisode();
        Destroy(gameObject);
    }

}