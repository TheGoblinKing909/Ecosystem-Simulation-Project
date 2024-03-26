using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents.Actuators;
using UnityEngine;

public class FlyingActions : Actions
{
    [SerializeField] private bool isFlying = false;
    private float groundSpeed = 10;
    public float additionalAirSpeed = 10;

    void Awake()
    {
        base.OnAwake();
        groundSpeed = attributes.agility;
    }

    public new void OnActionsRecieved(ActionBuffers actions)
    {
        float moveX = actions.ContinuousActions[0];
        float moveY = actions.ContinuousActions[1];
        if(isFlying)
        {
            movement.runSpeed = groundSpeed + additionalAirSpeed;
            movement.SetMovement(moveX, moveY);
        }
        else 
        {
            movement.runSpeed = groundSpeed;
            movement.SetMovement(moveX, moveY);
        }

        if (actionDelay == actionDelayMax)
        {
            int toggleFlying = actions.DiscreteActions[2];
            if ( toggleFlying == 1)
            {
                isFlying = !isFlying;
            }

            int harvest = actions.DiscreteActions[0];
            if (harvest == 1)
            {
                for (int i = 0; i < movement.collisions.Count; i++)
                {
                    if (movement.collisions[i] != null && movement.collisions[i].CompareTag("Resource"))
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
                    if (movement.collisions[i] != null && movement.collisions[i].CompareTag("Entity"))
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
                    if (movement.collisions[i] != null && movement.collisions[i].CompareTag("Resource"))
                    {
                        AttackResource(movement.collisions[i]);
                    }   
                }
                actionDelay = 0;
            }
        }
    }
}
