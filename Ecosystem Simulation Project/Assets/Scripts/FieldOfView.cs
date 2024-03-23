using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EntityType
{
    Diurnal,
    Nocturnal,
    Both
}

public class FieldOfView : MonoBehaviour
{
    public EntityType entityType = EntityType.Diurnal;

    private TimeManager timeManager;
    
    [SerializeField]
    private float maxViewRadius = 60f; // Define the maximum view radius
    [SerializeField]
    private float maxViewAngle = 90f; // Define the maximum view angle

    private float _viewRadius;
    public float viewRadius
    {
        get { return _viewRadius; }
        set { _viewRadius = value; AdjustFieldOfView(); }
    }

    [SerializeField, Range(0, 360)]
    private float _viewAngle;
    public float viewAngle
    {
        get { return _viewAngle; }
        set { _viewAngle = value; AdjustFieldOfView(); }
    }

    public LayerMask targetMask;
    public LayerMask obstacleMask;

    [HideInInspector]
    public List<Transform> visibleTargets = new List<Transform>();

    private void Start()
    {
        timeManager = FindObjectOfType<TimeManager>(); // Dynamically find the TimeManager
        if (timeManager == null)
        {
            Debug.LogError("No TimeManager found in the scene.");
        }

        StartCoroutine(FindTargetsWithDelay(0.2f));
    }

    IEnumerator FindTargetsWithDelay(float delay)
    {
        while (true)
        {
            yield return new WaitForSeconds(delay);
            FindVisibleTargets();
        }
    }

    void FindVisibleTargets()
    {
        visibleTargets.Clear();
        Collider2D[] targetsInViewRadius = Physics2D.OverlapCircleAll(transform.position, viewRadius, targetMask);

        for (int i = 0; i < targetsInViewRadius.Length; i++)
        {
            Transform target = targetsInViewRadius[i].transform;
            Vector3 dirToTarget = (target.position - transform.position).normalized;
            if (Vector3.Angle(transform.forward, dirToTarget) < viewAngle / 2)
            {
                float dstToTarget = Vector3.Distance(transform.position, target.position);

                if (!Physics.Raycast(transform.position, dirToTarget, dstToTarget, obstacleMask))
                {
                    visibleTargets.Add(target);
                }
            }
        }
    }

    public Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal)
    {
        if (!angleIsGlobal)
        {
            angleInDegrees += transform.eulerAngles.z;
        }
        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), Mathf.Cos(angleInDegrees * Mathf.Deg2Rad), 0);
    }

    void AdjustFieldOfView()
    {
        if (timeManager != null)
        {
            DateTime currentTime = timeManager.GetCurrentDateTime();
            int hour = currentTime.Hour;

            float multiplier = 1.0f; // Default multiplier for 'Both' or default state
            if (entityType == EntityType.Diurnal)
            {
                if (hour >= 6 && hour < 12) // Post dawn
                    multiplier = 1.0f;
                else if (hour >= 12 && hour < 18) // Pre dusk
                    multiplier = 0.75f;
                else // Post dusk and Pre dawn
                    multiplier = 0.5f;
            }
            else if (entityType == EntityType.Nocturnal)
            {
                if (hour >= 6 && hour < 12) // Post dawn
                    multiplier = 0.5f;
                else if (hour >= 12 && hour < 18) // Pre dusk
                    multiplier = 0.75f;
                else // Post dusk and Pre dawn
                    multiplier = 1.0f;
            }

            // Adjust viewRadius and viewAngle based on the multiplier
            _viewRadius = maxViewRadius * multiplier;
            _viewAngle = maxViewAngle * multiplier;
        }
    }

}