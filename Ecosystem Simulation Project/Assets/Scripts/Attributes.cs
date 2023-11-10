using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Integrations.Match3;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class Attributes : Agent
{
    // Player attributes
    public bool trainingMode = false;

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

    [SerializeField] private Transform target;
    [SerializeField] private Transform startingPoint;
    [SerializeField] private Transform targetStartingPoint;

    private float distanceToTarget;

    private int actionDelay;
    private int actionDelayMax = 25;

    public override void Initialize()
    {
        // rigidbody2D = GetComponent<Rigidbody2D>();
        movement = GetComponent<Movement>();

        if (!trainingMode) MaxStep = 0;
    }

    public override void OnEpisodeBegin()
    {
        if(trainingMode)
        {
            // reset world
        }

        // reset attributes
        currentHealth = maxHealth;
        currentHunger = maxHunger;
        currentThirst = maxThirst;
        currentStamina = maxStamina;

        //reset movement
        transform.position = startingPoint.position + new Vector3(Random.Range(1, 5), 0, 0);
        target.position = targetStartingPoint.position + new Vector3(0, Random.Range(1,5), 0);

        distanceToTarget = Vector3.Distance(transform.position, target.position);

    }

    private void Start()
    {
        currentHealth = maxHealth;
        currentStamina = maxStamina;
        currentHunger = maxHunger;
        currentThirst = maxThirst;
        actionDelay = actionDelayMax;

        // Start the health decay process
        // InvokeRepeating("DecayHunger", 1f, 1f); // Decay health every 1 second
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        base.OnActionReceived(actions);

        float previosDisatnce = distanceToTarget;

        float moveX = actions.ContinuousActions[0];
        float moveY = actions.ContinuousActions[1];

        // transform.localPosition += new Vector3(moveX, moveY) * Time.deltaTime * 2;
        movement.SetMovement(moveX, moveY);

        distanceToTarget = Vector3.Distance(transform.position, target.position);

        if(distanceToTarget < previosDisatnce)
        {
            AddReward(1f);
        }

        if (actionDelay == actionDelayMax)
        {
            int harvest = actions.DiscreteActions[0];
            if (harvest == 1)
            {
                for (int i = 0; i < movement.collisions.Count; i++)
                {
                    if (movement.collisions[i].CompareTag("Resource"))
                    {
                        HarvestResource(movement.collisions[i]);
                    }
                }
                actionDelay = 0;
            }
            else if (harvest == 2)
            {
                if (movement.currentLayer <= movement.waterLevel)
                {
                    HarvestWater();
                }
                actionDelay = 0;
            }

            int attack = actions.DiscreteActions[1];
            if (attack == 1)
            {
                for (int i = 0; i < movement.collisions.Count; i++)
                {
                    if (movement.collisions[i].CompareTag("Entity"))
                    {
                        AttackEntity(movement.collisions[i]);
                    }
                }
                actionDelay = 0;
            }
            else if (attack == 2)
            {
                for (int i = 0; i < movement.collisions.Count; i++)
                {
                    if (movement.collisions[i].CompareTag("Resource"))
                    {
                        AttackResource(movement.collisions[i]);
                    }
                }
                actionDelay = 0;
            }
        }
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> continuousActions = actionsOut.ContinuousActions;
        ActionSegment<int> discreteActions = actionsOut.DiscreteActions;
        continuousActions[0] = Input.GetAxisRaw("Horizontal");
        continuousActions[1] = Input.GetAxisRaw("Vertical");
        if (Input.GetKey(KeyCode.Alpha1))
        {
            discreteActions[0] = 1;
        }
        else if (Input.GetKey(KeyCode.Alpha2))
        {
            discreteActions[0] = 2;
        }
        else
        {
            discreteActions[0] = 0;
        }
        if (Input.GetKey(KeyCode.Alpha3))
        {
            discreteActions[1] = 1;
        }
        else if (Input.GetKey(KeyCode.Alpha4))
        {
            discreteActions[1] = 2;
        }
        else
        {
            discreteActions[1] = 0;
        }
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation((Vector2)transform.position);
        sensor.AddObservation((Vector2)target.position);
    }

    // FixedUpdate is called once every 0.02 seconds
    private void FixedUpdate()
    {
        if(currentHealth <= 0) 
        {
            Die();
        }
        // Handle input for actions that affect health, stamina, and hunger (e.g., sprinting, eating, taking damage).
        HandleInput();
        DecayHealth();
        if(currentHunger > (maxHunger/2) && currentThirst > (maxThirst/2))
        {
            Heal(5f * Time.deltaTime);
        }
        ModifyStamina(2 * Time.deltaTime);
        if (actionDelay < actionDelayMax)
        {
            actionDelay++;
        }
    }

    // Function to handle player input and update attributes
    private void HandleInput()
    {

        if (Input.GetKeyDown(KeyCode.F) && currentHealth > 0)
        {
            currentHealth -= 20f;
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            Debug.Log("Player Health + " + currentHealth);
            Debug.Log("Player Hunger + " + currentHunger);
            Debug.Log("Player Stamina + " + currentStamina);
        }
    }

    private void DecayHealth()
    {
        currentHunger -= hungerDecayRate * Time.deltaTime;
        currentThirst -= thirstDecayRate * Time.deltaTime;
        if (currentHunger <= 0)
        {
            currentHunger = 0;
            currentHealth -= 10 * Time.deltaTime;
        }
        if (currentThirst <=0)
        {
            currentThirst = 0;
            currentHealth -= 10 * Time.deltaTime;
        }

        AddReward(-.5f);
    }
    public void Eat(float amount)
    {
        currentHunger += amount;
        if(currentHunger > maxHunger)
        {
            currentHunger = maxHunger;
        }

        AddReward(10f);


    }
    public void Drink(float amount)
    {
        currentThirst += amount;
        if(currentThirst > maxThirst)
        {
            currentThirst = maxThirst;
        }
        AddReward(10f);
    }
    public void Heal(float amount)
    {
        currentHealth += amount;
        if(currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
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
    private void HarvestResource(GameObject resource)
    {
        Debug.Log(gameObject.name + " harvested resource " + resource.name);
        if (currentStamina >= 10)
        {
            ModifyStamina(-10);
            Resource harvestItem = resource.GetComponent<Resource>();
            float harvestAmount = harvestItem.Harvest();
            Eat(harvestAmount);
            AddReward(10f);
        }
    }
    private void HarvestWater()
    {
        Debug.Log(gameObject.name + " harvested water");
        if (currentStamina >= 10)
        {
            ModifyStamina(-10);
            Drink(10);
            AddReward(10f);
        }
    }
    private void AttackEntity(GameObject entity)
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
            AddReward(10f);
        }
        else
        {
            AddReward(-10f);
        }
    }
    private void AttackResource(GameObject resource)
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
        Instantiate(deathResource, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}