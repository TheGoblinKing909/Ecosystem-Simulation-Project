using Google.Protobuf.WellKnownTypes;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AIManager : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;
    [SerializeField] private ObjectSpawner resourceManager;
    [SerializeField] private float maxTimeBeforeReset = 10f;
    [SerializeField] private float TimeScale = 1.0f;
    public float _TimeScale
    {
        set
        {
            TimeScale = value;
            UpdateTimeScale();
        }
        get => TimeScale;
    }

    private float FixedDeltaTime = 0.02f;

    private Transform resourceManagerTransform { get
        {
            return resourceManager.transform;
        } 
    }
    public Transform[] resources { get { return resourceManager.GetComponentsInChildren<Transform>(); }  }

    [SerializeField] private ObjectSpawner entityManager;
    private Transform entityManagerTransform
    {
        get
        {
            return entityManager.transform;
        }
    }
    public Transform[] entites { get { return entityManager.GetComponentsInChildren<Transform>(); } }

    private float currentTime;

    private void Awake()
    {
        Time.timeScale = _TimeScale;
    }

    // Start is called before the first frame update
    void Start()
    {
        if (!gameManager)
        {
            Debug.Log("No Game Manager Preset");
        }

        if (!resourceManager) Debug.Log("Resource Manager Not set in AI manager");
        if (!entityManager) Debug.Log("Entity Manager Not set in AI manager");
    }

    // Update is called once per frame
    void Update()
    {
        currentTime += Time.deltaTime;
        if(currentTime > maxTimeBeforeReset)
        {
            ResetEpisodes();
        }
    }

    void ResetEpisodes()
    {
        //Delete Everything
        RemoveChildren(resourceManagerTransform);
        RemoveChildren(entityManagerTransform);

        //Add Everything
        resourceManager.PlaceResources();
        entityManager.PlaceEntities();

        HumanAgent[] humansAgents = entityManager.GetComponentsInChildren<HumanAgent>();
        foreach(HumanAgent agent in humansAgents)
        {
            agent.OnEpisodeBegin();
        }

        currentTime = 0;
    }

    void RemoveChildren(Transform parent)
    {
        foreach (Transform child in parent)
        {
            Destroy(child.gameObject);
        }

        parent.DetachChildren();
    }

    void UpdateTimeScale()
    {
        Time.timeScale = _TimeScale;
    }

    private void OnValidate()
    {
        _TimeScale = TimeScale;
    }
}
