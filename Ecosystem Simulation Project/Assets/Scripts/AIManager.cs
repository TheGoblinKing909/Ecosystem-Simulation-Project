using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIManager : MonoBehaviour
{

    [SerializeField] private bool isTrainingmode = true;
    [SerializeField] private bool resetEpisodes = true;
    [SerializeField] private float maxTimeBeforeReset = 10f;
    [SerializeField] private ObjectSpawner objectSpawner;
    private float currentTime;


    // Start is called before the first frame update
    void Start()
    {
        if (!objectSpawner) Debug.Log("Object Spawner Not set in AI manager");
        
    }

    // Update is called once per frame
    void Update()
    {
        currentTime += Time.deltaTime;
        if (resetEpisodes)
        {
            Debug.Log(currentTime);
            if(currentTime > maxTimeBeforeReset)
            {
                ResetEpisodes();
            }
        }
    }

    void ResetEpisodes()
    {
        currentTime = 0;
        Debug.Log("ResetEpisodeCalled");
    }
}
