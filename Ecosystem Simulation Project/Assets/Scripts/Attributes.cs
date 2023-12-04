using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Integrations.Match3;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class Attributes : MonoBehaviour
{
    // Player attributes
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
    public float hungerDecayRate = 2f; // Health decay rate per second
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

    // public Rigidbody2D rigidbody2D;
    public Movement movement;
    public Resource currentResource;
    public GameObject deathResource;
    private HumanRewards humanRewards;
    private HumanAgent humanAgent;
    private float ageTime;

    public void Awake()
    {
        movement = GetComponent<Movement>();
        var test = movement;
        if (movement == null)
        {
            throw new System.Exception("movement not set in attributes");
        }

        humanRewards = GetComponent<HumanRewards>();
        if (humanRewards == null)
        {
            throw new System.Exception("HumanRewards not set in attributes");
        }

        humanAgent = GetComponent<HumanAgent>();
        if (humanAgent == null)
        {
            throw new System.Exception("Human Agent not set in attributes");
        }
    }

    public void EpisodeBegin()
    {
        // reset attributes
        maxHealth = initialHealth;
        maxStamina = initialStamina;
        maxHunger = initialHunger;
        maxThirst = initialThirst;
        agility = initialAgility;
        attack = initialAttack;
        size = initialSize;

        currentHealth = maxHealth;
        currentHunger = maxHunger;
        currentThirst = maxThirst;
        currentStamina = maxStamina;

        currentAge = 0;
        ageTime = 0;
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

        ageTime += Time.deltaTime;
        if (ageTime > ageDelay) {
            IncreaseAge();
            ageTime = 0;
        }

        if (shelter != null && currentHunger > 0 && currentThirst > 0) {
            float recoveryAmount = shelter.recoveryRate * Time.deltaTime;
            Heal(recoveryAmount);
            ModifyStamina(recoveryAmount);
            humanAgent.AddReward(humanRewards.GetHealthGainedReward(recoveryAmount));
        }
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
            Debug.Log(gameObject.name + " harvested resource " + resource.name);
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
            Debug.Log(gameObject.name + " harvested water");
            ModifyStamina(-10);
            Drink(10);
        }
    }
    public void AttackEntity(GameObject entity)
    {
        Attributes targetAttributes = entity.GetComponent<Attributes>();
        if(currentStamina >= 10)
        {
            Debug.Log(gameObject.name + " attacked entity " + entity.name);
            ModifyStamina(-10f);
            if(Random.Range(1,10) >= targetAttributes.agility)
            {
                targetAttributes.currentHealth -= attack;
            }
        }
        if(currentHealth >= targetAttributes.currentHealth)
        {
            humanAgent.AddReward(humanRewards.GetAttackReward());
        }
        else
        {
            humanAgent.AddReward(humanRewards.GetAttackPunishment());
        }
    }
    public void AttackResource(GameObject resource)
    {
        if(currentStamina >= 10)
        {
            Debug.Log(gameObject.name + " attacked resource " + resource.name);
            ModifyStamina(-10f);
            Resource targetResource = resource.GetComponent<Resource>();
            targetResource.HealthRemaining -= attack;
        }
    }

    private void IncreaseAge()
    {
        currentAge++;
        if (currentAge <= primeAge) {
            float ageScaler =  (currentAge * currentAge) / (primeAge * primeAge);

            AgeToPrime(ref currentHealth, ref maxHealth, initialHealth, ageScaler);
            AgeToPrime(ref currentStamina, ref maxStamina, initialStamina, ageScaler);
            AgeToPrime(ref currentHunger, ref maxHunger, initialHunger, ageScaler);
            AgeToPrime(ref currentThirst, ref maxThirst, initialThirst, ageScaler);

            agility = initialAgility + (initialAgility * ageScaler);
            attack = initialAttack + (initialAttack * ageScaler);
            size = initialSize + (initialSize * ageScaler);
        }
        else if (currentAge <= maxAge) {
            float ageScaler =  ((currentAge - primeAge) * (currentAge - primeAge)) / ((maxAge - primeAge) * (maxAge - primeAge));

            AgePastPrime(ref currentHealth, ref maxHealth, initialHealth, ageScaler);
            AgePastPrime(ref currentStamina, ref maxStamina, initialStamina, ageScaler);
            AgePastPrime(ref currentHunger, ref maxHunger, initialHunger, ageScaler);
            AgePastPrime(ref currentThirst, ref maxThirst, initialThirst, ageScaler);

            agility = (initialAgility * 2) - (initialAgility * ageScaler);
            attack = (initialAttack * 2) - (initialAttack * ageScaler);
            size = (initialSize * 2) - (initialSize * ageScaler);
        }
        else {
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
        GameObject deathInstance = Instantiate(deathResource, transform.position, Quaternion.identity);
        GameManager gameManager = FindObjectOfType<GameManager>();

        if (gameManager != null) {

            Transform resourceManagerTransform = gameManager.transform.Find("ResourceManager");

            if (resourceManagerTransform != null) {
                deathInstance.transform.parent = resourceManagerTransform;
            }
        }

        Destroy(gameObject);
        humanAgent.AddReward(-10000f);
    }
}