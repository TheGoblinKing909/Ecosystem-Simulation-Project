using System.Collections.Generic;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Integrations.Match3;
using Unity.MLAgents.Sensors;
using Unity.VisualScripting;
using UnityEngine;

public class Entity : Agent
{
    private Attributes attributes;
    private Actions actions;
    private AIManager manager;

    protected void OnInitalize()
    {
        attributes = GetComponent<Attributes>();
        if (attributes == null) throw new System.Exception("Attributes not set for human agent");

        actions = GetComponent<Actions>();
        manager = FindObjectOfType<AIManager>();
        if (manager == null) throw new System.Exception("AIManager not set in HumanAgent parent");
    }

    public override void Initialize()
    {
        OnInitalize();
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation((Vector2)transform.position);

        var seeResource = false;

        foreach (Transform resource in manager.resources)
        {
            if (seeResource)
            {
                sensor.AddObservation(1);
                Vector2 direction = (resource.position - transform.position);
                float distance = Vector2.Distance(transform.position, resource.position);
                Vector3 resourceObservation = new Vector3(distance, direction.x, direction.y);
                sensor.AddObservation(resourceObservation);
            }
            seeResource = !seeResource;
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
}