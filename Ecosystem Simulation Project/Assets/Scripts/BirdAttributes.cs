using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Integrations.Match3;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class BirdAttributes : Attributes
{
    private BirdAgent birdAgent;

    public void Awake()
    {
        base.OnAwake();

        birdAgent = GetComponent<BirdAgent>();
        if (birdAgent == null)
        {
            throw new System.Exception("bird Agent not set in attributes");
        }
    }
}