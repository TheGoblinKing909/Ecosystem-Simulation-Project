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

public enum ObservationType
{
    Entity,
    Resource,
    Water,
}

public class Observation
{
    public ObservationType ObservationType;
    public int ObservationID = -1;
    public Vector3 TargetPosition;

}

public class Entity : Agent
{
    public AgentType agentType = AgentType.None;
    public float AvgRewardsPerStep;
    public float CReward;
    private float _AvgRewardPerStep { get => (GetCumulativeReward() / StepCount); set{ } }

    private float _CReward { get => (this.GetCumulativeReward()); set { } }
    private Attributes attributes;
    private AttributeBar attributeBar;
    private Actions actions;
    private FieldOfView fov;
    private Movement movement;

    protected void OnInitalize()
    {
        attributes = GetComponent<Attributes>();
        if (attributes == null) throw new System.Exception("Attributes not set for " + agentType  + " agent");

        attributeBar = GetComponentInChildren<AttributeBar>();
        if (attributeBar == null) throw new System.Exception("Attribute Bar not set in " + agentType + " agent ");

        actions = GetComponent<Actions>();

        fov = GetComponent<FieldOfView>();
        if (fov == null) throw new System.Exception("fov not set in "+ agentType +" agent ");

        movement = GetComponent<Movement>();
        if (movement == null) throw new System.Exception("movment not set in " + agentType + " agent ");
    }

    public override void Initialize()
    {
        OnInitalize();
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        Observation observation = new();
        AvgRewardsPerStep = _AvgRewardPerStep;
        CReward = _CReward;
        attributeBar.UpdateAvgReward(AvgRewardsPerStep);
        sensor.AddObservation(attributes.currentHealth);
        sensor.AddObservation(attributes.currentHunger);
        sensor.AddObservation(attributes.currentThirst);
        sensor.AddObservation(attributes.currentStamina);
        sensor.AddObservation(transform.position);

        foreach (FieldOfView.VisibleTargetData data in fov.visibleTargets)
        {
            Transform target = data.transform;
            if(target != null)
            {
                Entity entity = data.entity;
                Resource resource = data.resource;
                Water water = data.water;
                Vector3 targetPosition = target.position;
                if (entity != null) {
                    observation.ObservationType = ObservationType.Entity;
                    observation.ObservationID = (int)entity.agentType;
                }
                else if (resource != null) {
                    observation.ObservationType = ObservationType.Resource;
                    observation.ObservationID = (int)resource.resourceType;
                }
                else if (water != null)
                {
                    observation.ObservationType = ObservationType.Water;
                    observation.ObservationID = (int)ResourceType.Water;
                    targetPosition = data.waterPosition;
                }
                Vector2 transformPos2 = new Vector2(transform.position.x, transform.position.y);
                Vector2 targetPos2 = new Vector2(targetPosition.x, targetPosition.y);
                Vector2 direction = (targetPos2 - transformPos2).normalized;
                float distance = Vector2.Distance(transformPos2, targetPos2);
                observation.TargetPosition = new Vector3(distance, direction.x, direction.y);
                if (distance != 0)
                {
                    SendObservations(observation, sensor);
                }
            }
        }

    }

    private void SendObservations(Observation observation, VectorSensor sensor)
    {
        if(observation.ObservationID < 0) { return; }
        sensor.AddObservation((int)observation.ObservationType);
        sensor.AddObservation(observation.ObservationID);
        sensor.AddObservation(observation.TargetPosition);
    }

    public override void OnEpisodeBegin()
    {
        //reset episodes
        // attributes.EpisodeBegin();
        // actions.EpisodeBegin();
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
        // attributes.agility += effects.AgilityEffect;

        attributes.hungerDecayRate += effects.HungerDecayRateEffect;
        attributes.thirstDecayRate += effects.ThirstDecayRateEffect;

        attributes.currentHealth = Mathf.Clamp(attributes.currentHealth, 0, attributes.maxHealth);
        attributes.currentStamina = Mathf.Clamp(attributes.currentStamina, 0, attributes.maxStamina);
        attributes.currentHunger = Mathf.Clamp(attributes.currentHunger, 0, attributes.maxHunger);
        attributes.currentThirst = Mathf.Clamp(attributes.currentThirst, 0, attributes.maxThirst);

        Debug.Log($"Weather effects applied to Entity: {effects.HealthEffect} Health, {effects.StaminaEffect} Stamina.");
    }

}