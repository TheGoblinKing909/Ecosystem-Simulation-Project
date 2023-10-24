using UnityEngine;

public class Attributes : MonoBehaviour
{
    // Player attributes
    public float maxHealth = 100f;
    public float maxStamina = 100f;
    public float maxHunger = 100f;

    public float hungerDecayRate = 2f; // Health decay rate per second

    private float currentHealth;
    private float currentStamina;
    private float currentHunger;

    private void Start()
    {
        currentHealth = maxHealth;
        currentStamina = maxStamina;
        currentHunger = maxHunger;

        // Start the health decay process
        InvokeRepeating("DecayHunger", 1f, 1f); // Decay health every 1 second
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
    }
    public void Eat(float amount)
    {
        currentHunger += amount;
        if(currentHunger > maxHunger)
        {
            currentHunger = maxHunger;
        }
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