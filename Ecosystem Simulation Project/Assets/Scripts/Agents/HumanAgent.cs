using System.Collections.Generic;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Integrations.Match3;
using Unity.MLAgents.Sensors;
using Unity.VisualScripting;
using UnityEngine;

public class HumanAgent : Agent
{
    private Attributes attributes;
    private HumanActions humanActions;
    [SerializeField] private AIManager manager;

    private void Awake()
    {
        Initialize();
    }

    public override void Initialize()
    {
        attributes = GetComponent<Attributes>();
        if(attributes == null) throw new System.Exception("Attributes not set for human agent");
        
        humanActions = GetComponent<HumanActions>();
        if (humanActions == null) throw new System.Exception("HumanActions not set in HumanAgent");

        manager = FindObjectOfType<AIManager>();
        if (manager == null) throw new System.Exception("AIManager not set in HumanAgent parent");
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation((Vector2)transform.position);

        foreach(Transform resource in manager.resources)
        {
            sensor.AddObservation(1);
            Vector2 direction = (resource.position - transform.position);
            float distance = Vector2.Distance(transform.position, resource.position);
            Vector3 resourceObservation = new Vector3(distance, direction.x, direction.y);
            sensor.AddObservation(resourceObservation);
        }
        
        foreach(Transform entites in manager.entites)
        {
            sensor.AddObservation(2);
            Vector2 direction = (entites.position - transform.position).normalized;
            float distance = Vector3.Distance(transform.position, entites.position);
            Vector3 entityObservation = new Vector3(distance, direction.x, direction.y);
            sensor.AddObservation(entityObservation);
        }
    }
    public override void OnEpisodeBegin()
    {
        //reset episodes
        attributes.EpisodeBegin();
        humanActions.EpisodeBegin();
    }
    public override void OnActionReceived(ActionBuffers actions)
    {
        humanActions.ActionsRecieved(actions);
    }
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        humanActions.Heuristic(actionsOut);
    }
}
