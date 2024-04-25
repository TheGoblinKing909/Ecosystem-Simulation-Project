using System.Collections.Generic;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Integrations.Match3;
using Unity.MLAgents.Sensors;
using Unity.VisualScripting;
using UnityEngine;
using weather.effects;

public enum AgentType
{
    None = 0,
    Eagle = 1,
    Owl,
    Wolf,
    Chicken,
    ChirpingBird,
    Cow,
    Pig,
    Bear,
    Fox,
    Gorilla,
    Oragatan,
}

public class Entity : Agent
{
    public AgentType agentType = AgentType.None;
    public float AvgRewardsPerStep;
    public float CReward;
    private float _AvgRewardPerStep { get => (GetCumulativeReward() / StepCount); set{ } }

    private float _CReward { get => (this.GetCumulativeReward()); set { } }
    private Attributes attributes;
    private Actions actions;
    private AIManager manager;
    private FieldOfView fov;

    protected void OnInitalize()
    {
        attributes = GetComponent<Attributes>();
        if (attributes == null) throw new System.Exception("Attributes not set for human agent");

        actions = GetComponent<Actions>();
        manager = FindObjectOfType<AIManager>();
        if (manager == null) throw new System.Exception("AIManager not set in HumanAgent parent");

        fov = GetComponent<FieldOfView>();
        if (fov == null) throw new System.Exception("fov not set in "+ agentType +" agent parent");
    }

    public override void Initialize()
    {
        OnInitalize();
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        AvgRewardsPerStep = _AvgRewardPerStep;
        CReward = _CReward;
        sensor.AddObservation(transform.position);
        sensor.AddObservation(AvgRewardsPerStep);

        foreach (FieldOfView.VisibleTargetData data in fov.visibleTargets)
        {
                Entity entity = data.entity;
                Transform target = data.transform;
                AgentType agentType = AgentType.None;
                if (entity != null) { agentType = entity.agentType; }
                Vector2 direction = (target.position - transform.position);
                Vector3 entityData = new((float)agentType, direction.x, direction.y);
                float distance = Vector2.Distance(transform.position, target.position);
                sensor.AddObservation(entityData);
                sensor.AddObservation(distance);
                Vector3 resourceObservation = new Vector3(distance, direction.x, direction.y);
                sensor.AddObservation(resourceObservation);
        }

        //foreach(Transform entites in manager.entites)
        //{
        //    sensor.AddObservation(2);
        //    Vector2 direction = (entites.position - transform.position).normalized;
        //    float distance = Vector3.Distance(transform.position, entites.position);
        //    Vector3 entityObservation = new Vector3(distance, direction.x, direction.y);
        //    sensor.AddObservation(entityObservation);
        //}
    }

    public override void OnEpisodeBegin()
    {
        //reset episodes
        attributes.EpisodeBegin();
        actions.EpisodeBegin();
    }

    public override void OnActionReceived(ActionBuffers input)
    {
        actions.OnActionsRecieved(input);
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        actions.Heuristic(actionsOut);
    }

    public void ApplyWeatherEffects(AttributeEffects effects)
    {
        if (attributes == null)
        {
            Debug.LogError("Attributes component not found on entity.");
            return;
        }

        attributes.currentHealth += effects.HealthEffect;
        attributes.currentStamina += effects.StaminaEffect;
        attributes.currentHunger += effects.HungerEffect;
        attributes.currentThirst += effects.ThirstEffect;
        attributes.agility += effects.AgilityEffect;

        attributes.hungerDecayRate += effects.HungerDecayRateEffect;
        attributes.thirstDecayRate += effects.ThirstDecayRateEffect;

        attributes.currentHealth = Mathf.Clamp(attributes.currentHealth, 0, attributes.maxHealth);
        attributes.currentStamina = Mathf.Clamp(attributes.currentStamina, 0, attributes.maxStamina);
        attributes.currentHunger = Mathf.Clamp(attributes.currentHunger, 0, attributes.maxHunger);
        attributes.currentThirst = Mathf.Clamp(attributes.currentThirst, 0, attributes.maxThirst);

        Debug.Log($"Weather effects applied to Entity: {effects.HealthEffect} Health, {effects.StaminaEffect} Stamina.");
    }

}