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

    public float hungerDecayRate = 2f; // Health decay rate per second

    public float currentHealth;
    public float currentStamina;
    public float currentHunger;

    public Rigidbody2D rigidbody2D;
    public Resource currentResource;

    [SerializeField] private Transform target;

    public override void Initialize()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();

        if (!trainingMode) MaxStep = 0;
    }

    public override void OnEpisodeBegin()
    {
        if(trainingMode)
        {
            // reest world
        }

        // reet attributes
        currentHealth = maxHealth;
        currentHunger = maxHunger;
        currentStamina = maxStamina;

        //reset movement
        transform.position = new Vector3(-0.36f, 0.18f, 25f);
        target.position = new Vector3(1, Random.Range(1,5), 25);


    }

    private void Start()
    {
        currentHealth = maxHealth;
        currentStamina = maxStamina;
        currentHunger = maxHunger;

        // Start the health decay process
        InvokeRepeating("DecayHunger", 1f, 1f); // Decay health every 1 second
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        base.OnActionReceived(actions);

        float moveX = actions.ContinuousActions[0];
        float moveY = actions.ContinuousActions[1];

        float movementSeed = 5f;

        transform.localPosition += new Vector3(moveX, moveY) * Time.deltaTime * 2;
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> continuousActions = actionsOut.ContinuousActions;
        continuousActions[0] = Input.GetAxisRaw("Horizontal");
        continuousActions[1] = Input.GetAxisRaw("Vertical");
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation((Vector2)transform.position);
        sensor.AddObservation((Vector2)target.position);
    }

    // Update is called once per frame
    private void Update()
    {
        // Handle input for actions that affect health, stamina, and hunger (e.g., sprinting, eating, taking damage).
        HandleInput();
        DecayHealth();
        if(currentHunger > (maxHunger/2))
        {
            Heal(5f);
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
        if (currentHunger <= 0)
        {
            currentHunger = 0;
            currentHealth -= 10;
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
    public void Heal(float amount)
    {
        currentHealth += amount;
        if(currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
    }
}