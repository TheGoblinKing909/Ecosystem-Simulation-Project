using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents.Actuators;
using UnityEngine;

public class Actions : MonoBehaviour
{
    protected Movement movement;
    protected Attributes attributes;

    [SerializeField] protected int actionDelay;
    [SerializeField] protected int actionDelayMax = 25;

    protected void OnAwake()
    {
        movement = GetComponent<Movement>();
        if (movement == null) throw new System.Exception("Moment not set in HumanActions");

        attributes = GetComponent<Attributes>();
        if (attributes == null) throw new System.Exception("Attributes not set in HumanActions");

        EpisodeBegin();
    }
    void Awake()
    {
        OnAwake();
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

    public virtual void OnCustomActionReceived(ActionBuffers actions)
    {
        if (attributes.shelter == null)
        {
            float moveX = actions.ContinuousActions[0];
            float moveY = actions.ContinuousActions[1];

            movement.SetMovement(moveX, moveY);

            if (actionDelay == actionDelayMax)
            {
                int harvest = actions.DiscreteActions[0];
                if (harvest == 1)
                {
                    if (movement.currentLayer == 1)
                    {
                        HarvestWater();
                    }
                    for (int i = 0; i < movement.collisions.Count; i++)
                    {
                        if (movement.collisions[i] != null)
                        {
                            if (movement.collisions[i].CompareTag("Resource") || movement.collisions[i].CompareTag("Shelter"))
                            {
                                HarvestResource(movement.collisions[i]);
                            }
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

    public void OnActionsRecieved(ActionBuffers actions)
    {
        OnCustomActionReceived(actions);

    }
    public virtual void Heuristic(in ActionBuffers actionsOut)
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
    }

    public void HarvestWater()
    {
        if (attributes.currentStamina >= 10)
        {
            attributes.ModifyStamina(-10);
            attributes.ModifyThirst(10);
        }
    }

    public void HarvestResource(GameObject resource)
    {
        if (attributes.currentStamina >= 10)
        {
            attributes.ModifyStamina(-10);
            Resource harvestItem = resource.GetComponent<Resource>();
            float harvestAmount = harvestItem.Harvest();
            attributes.ModifyHunger(harvestAmount);
        }
    }

    public void AttackEntity(GameObject entity)
    {
        Attributes targetAttributes = entity.GetComponent<Attributes>();
        if (attributes.currentStamina >= 10)
        {
            attributes.ModifyStamina(-10f);
            if (Random.Range(1, 10) >= targetAttributes.agility)
            {
                targetAttributes.currentHealth -= attributes.attack;
            }
        }
    }
    public void AttackResource(GameObject resource)
    {
        if (attributes.currentStamina >= 10)
        {
            attributes.ModifyStamina(-10f);
            Resource targetResource = resource.GetComponent<Resource>();
            targetResource.HealthRemaining -= attributes.attack;
        }
    }
}
