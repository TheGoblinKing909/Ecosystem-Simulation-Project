using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents.Actuators;
using UnityEngine;

public class HumanActions : MonoBehaviour
{
    [SerializeField] private int actionDelay;
    [SerializeField] private int actionDelayMax = 25;

    private Movement movement;
    private Attributes attributes;

    void Awake()
    {
        movement = GetComponent<Movement>();
        if (movement == null) throw new System.Exception("Moment not set in HumanActions");

        attributes = GetComponent<Attributes>();
        if (attributes == null) throw new System.Exception("Attributes not set in HumanActions");

        EpisodeBegin();
    }
    
    public void EpisodeBegin()
    {
        actionDelay = actionDelayMax;
    }

    private void FixedUpdate()
    {
        if (actionDelay < actionDelayMax)
        {
            actionDelay++;
        }
    }

    public void ActionsRecieved(ActionBuffers actions)
    {
        float moveX = actions.ContinuousActions[0];
        float moveY = actions.ContinuousActions[1];

        movement.SetMovement(moveX, moveY);

        if (actionDelay == actionDelayMax)
        {
            int harvest = actions.DiscreteActions[0];
            if (harvest == 1)
            {
                for (int i = 0; i < movement.collisions.Count; i++)
                {
                    if (movement.collisions[i] != null && movement.collisions[i].CompareTag("Resource"))
                    {
                        attributes.HarvestResource(movement.collisions[i]);
                    }
                }
                actionDelay = 0;
            }
            else if (harvest == 2)
            {
                if (movement.currentLayer <= movement.waterLevel)
                {
                    attributes.HarvestWater();
                }
                actionDelay = 0;
            }

            int attack = actions.DiscreteActions[1];
            if (attack == 1)
            {
                for (int i = 0; i < movement.collisions.Count; i++)
                {
                    if (movement.collisions[i] != null && movement.collisions[i].CompareTag("Entity"))
                    {
                        attributes.AttackEntity(movement.collisions[i]);
                    }
                }
                actionDelay = 0;
            }
            else if (attack == 2)
            {
                for (int i = 0; i < movement.collisions.Count; i++)
                {
                    if (movement.collisions[i] != null && movement.collisions[i].CompareTag("Resource"))
                    {
                        attributes.AttackResource(movement.collisions[i]);
                    }
                }
                actionDelay = 0;
            }
        }
    }

    public void Heuristic(in ActionBuffers actionsOut)
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
}
