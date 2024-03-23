using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


public class CameraMovement : MonoBehaviour
{
    private Camera view;
    private Vector3 dragPosition;
    private EventSystem eventSystem; // Add this line
    public float dragSpeed;
    public float zoomSpeed;
    public float zoomMin;
    public float zoomMax;
    
    void Start()
    {
        view = GetComponent<Camera>();
        eventSystem = EventSystem.current; // Add this line
    }

    void Update()
    {
        // Add the following check to ignore UI interactions
        // Check if eventSystem is null before using it
        if (eventSystem == null || eventSystem.IsPointerOverGameObject())
            return;

        if (Input.GetMouseButtonDown(0))
        {
            dragPosition = Input.mousePosition;
        }
        else if (Input.GetMouseButton(0))
        {
            Vector3 move = dragPosition - Input.mousePosition;
            transform.position += new Vector3(move.x * dragSpeed, move.y * dragSpeed, 0);
            dragPosition = Input.mousePosition;
        }

        if (Input.GetAxis("Mouse ScrollWheel") < 0 && view.orthographicSize < zoomMax)
        {
            view.orthographicSize += zoomSpeed;
        }
        else if (Input.GetAxis("Mouse ScrollWheel") > 0 && view.orthographicSize > zoomMin)
        {
            view.orthographicSize -= zoomSpeed;
        }

    }
}