using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shelter : MonoBehaviour
{
    public List<GameObject> shelteredEntities = new List<GameObject>();
    public float capacity = 20f;
    public float recoveryRate = 10f;
    private ShelterBar shelterBar;

    private void Awake()
    {
        shelterBar = GetComponentInChildren<ShelterBar>();
        if (shelterBar == null) { throw new System.Exception("Shelter Bar not set in attributes"); }
    }

    public float GetSizeRemaining() 
    {
        float sizeUsed = 0;
        for (int i = 0; i < shelteredEntities.Count; i++) {
            Attributes entityAttributes = shelteredEntities[i].GetComponent<Attributes>();
            sizeUsed += entityAttributes.size;
        }
        return capacity - sizeUsed;
    }

    public void EnterShelter(GameObject entity) 
    {
        Attributes entityAttributes = entity.GetComponent<Attributes>();
        if (entityAttributes.size <= GetSizeRemaining()) {
            shelteredEntities.Add(entity);
            entityAttributes.shelter = this;
            
            CapsuleCollider2D entityCollider = entity.GetComponent<CapsuleCollider2D>();
            entityCollider.enabled = false;

            SpriteRenderer[] entityRenderer = entity.GetComponentsInChildren<SpriteRenderer>();
            entityRenderer[0].enabled = false;

            // Debug.Log(entity.name + " entered " + gameObject.name);

            shelterBar.UpdateCapacityBar(GetSizeRemaining(), capacity);
        }
        else {
            // Debug.Log(entity.name + " could not enter " + gameObject.name + ": remaining size is " + GetSizeRemaining());
        }
    }

    public void ExitShelter(GameObject entity) 
    {
        shelteredEntities.Remove(entity);
        Attributes entityAttributes = entity.GetComponent<Attributes>();
        entityAttributes.shelter = null;

        CapsuleCollider2D entityCollider = entity.GetComponent<CapsuleCollider2D>();
        entityCollider.enabled = true;

        SpriteRenderer[] entityRenderer = entity.GetComponentsInChildren<SpriteRenderer>();
        entityRenderer[0].enabled = true;

        // Debug.Log(entity.name + " exited " + gameObject.name);

        shelterBar.UpdateCapacityBar(GetSizeRemaining(), capacity);
    }
}
