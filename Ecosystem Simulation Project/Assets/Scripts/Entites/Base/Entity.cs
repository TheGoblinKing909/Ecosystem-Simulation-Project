using System.Collections.Generic;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Integrations.Match3;
using Unity.MLAgents.Sensors;
using Unity.VisualScripting;
using UnityEngine;

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
    public float CurrentThirst = -1;
    public float CurrentHunger = -1;
    public float CurrentHealth = -1;
    public float CurrentStamina = -1;
    public Vector3 Position; 
    public Vector3 TargetPosition;

}

public class Entity : Agent
{
    public  AgentType agentType = AgentType.None;
    public float AvgRewardsPerStep;
    public float CReward;
    private float _AvgRewardPerStep { get => (GetCumulativeReward() / StepCount); set{ } }

    private float _CReward { get => (this.GetCumulativeReward()); set { } }
    private Attributes attributes;
    private Actions actions;
    private FieldOfView fov;
    private Movement movement;

    protected void OnInitalize()
    {
        attributes = GetComponent<Attributes>();
        if (attributes == null) throw new System.Exception("Attributes not set for " + agentType  + " agent");

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
        observation.CurrentHealth = attributes.currentHealth;
        observation.CurrentHunger = attributes.currentHunger;
        observation.CurrentThirst = attributes.currentThirst;
        observation.CurrentStamina = attributes.currentStamina;
        observation.Position = transform.position;

        foreach (FieldOfView.VisibleTargetData data in fov.visibleTargets)
        {
            Entity entity = data.entity;
            Resource resource = data.resource;
            if(data.transform != null)
            {
                if (entity != null) {
                    observation.ObservationType = ObservationType.Entity;
                    observation.ObservationID = (int)entity.agentType;
                }
                else if (resource != null) {
                    observation.ObservationType = ObservationType.Resource;
                    observation.ObservationID = (int)resource.resourceType;
                    if(resource.resourceType == ResourceType.Water) 
                    { 
                        observation.ObservationType = ObservationType.Water;
                    }
                }
                Vector2 direction = (target.position - transform.position);
                float distance = Vector2.Distance(transform.position, target.position);
                observation.TargetPosition = new Vector3(distance, direction.x, direction.y);
                SendObservations(observation, sensor);

            }
        }

    }

    private void SendObservations(Observation observation, VectorSensor sensor)
    {
        if(observation.ObservationID < 0) { return; }
        sensor.AddObservation((int)observation.ObservationType);
        sensor.AddObservation(observation.ObservationID);
        sensor.AddObservation(observation.CurrentHealth);
        sensor.AddObservation(observation.CurrentHunger);
        sensor.AddObservation(observation.CurrentThirst);
        sensor.AddObservation(observation.CurrentStamina);
        sensor.AddObservation(observation.Position);
        sensor.AddObservation(observation.TargetPosition);
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
}