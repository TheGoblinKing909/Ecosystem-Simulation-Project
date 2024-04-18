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

    public override void OnCustomActionReceived(ActionBuffers actions)
    {
        if (attributes.shelter == null)
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
                int toggleFlying = actions.DiscreteActions[3];
                if (toggleFlying == 1)
                {
                    isFlying = !isFlying;
                    UnityEngine.Debug.Log("flying switch");
                }

                int harvest = actions.DiscreteActions[0];
                if (harvest == 1)
                {
                    for (int i = 0; i < movement.collisions.Count; i++)
                    {
                        if (movement.collisions[i] != null && (movement.collisions[i].CompareTag("Resource") || movement.collisions[i].CompareTag("Shelter")))
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
                        if (movement.collisions[i] != null && (movement.collisions[i].CompareTag("Resource") || movement.collisions[i].CompareTag("Shelter")))
                        {
                            AttackResource(movement.collisions[i]);
                        }   
                    }
                    actionDelay = 0;
                }

                int useShelter = actions.DiscreteActions[2];
                if (useShelter == 1)
                {
                    for (int i = 0; i < movement.collisions.Count; i++)
                    {
                        if (movement.collisions[i] != null && movement.collisions[i].CompareTag("Shelter"))
                        {
                            Shelter shelter = movement.collisions[i].GetComponent<Shelter>();
                            shelter.EnterShelter(gameObject);
                            movement.SetMovement(0,0);
                        }
                    }
                }
                actionDelay = 0;
            }
        }
        else if (actionDelay == actionDelayMax)
        {
            int useShelter = actions.DiscreteActions[2];
            if (useShelter == 2)
            {
                attributes.shelter.ExitShelter(gameObject);
                actionDelay = 0;
            }
        }
    }

    public override void Heuristic(in ActionBuffers actionsOut)
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
        if (Input.GetKey(KeyCode.Alpha5))
        {
            discreteActions[2] = 1;
        }
        else if (Input.GetKey(KeyCode.Alpha6))
        {
            discreteActions[2] = 2;
        }
        else
        {
            discreteActions[2] = 0;
        }
        if (Input.GetKey(KeyCode.Alpha7))
        {
            discreteActions[3] = 1;
        }
        else
        {
            discreteActions[3] = 0;
        }
    }
}
