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

    [SerializeField]
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
    public class VisibleTargetData
    {
        public Transform transform;
        public Entity entity;
        public Resource resource;
        public Water water;
        public Vector3 waterPosition;
    }
    public List<VisibleTargetData> visibleTargets = new List<VisibleTargetData>();

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
            var gameObject = targetsInViewRadius[i].gameObject;
            Entity entity = gameObject.GetComponent<Entity>();
            Resource resource = gameObject.GetComponent<Resource>();
            Water water = gameObject.GetComponent<Water>();
            Transform target = targetsInViewRadius[i].transform;
            Vector3 targetPosition = target.position;
            if (water != null)
            {
                Vector2 waterPos = targetsInViewRadius[i].ClosestPoint(transform.position);
                targetPosition.x = waterPos.x;
                targetPosition.y = waterPos.y;
            }
            Vector3 dirToTarget = (targetPosition - transform.position).normalized;
            float dstToTarget = Vector3.Distance(transform.position, targetPosition);
            if (!Physics.Raycast(transform.position, dirToTarget, dstToTarget, obstacleMask))
            {
                VisibleTargetData data = new();
                if (resource != null)
                {

                    data.transform = target;
                    data.resource = resource;
                    visibleTargets.Add(data);
                }
                if (entity != null)
                {
                    data.transform = target;
                    data.entity = entity;
                    visibleTargets.Add(data);
                }
                if (water != null)
                {
                    data.transform = target;
                    data.water = water;
                    data.waterPosition = targetPosition;
                    visibleTargets.Add(data);
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