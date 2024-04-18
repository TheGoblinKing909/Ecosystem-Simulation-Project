using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor (typeof (FieldOfView))]
public class FieldOfViewEditor : Editor
{
    void OnSceneGUI()
    {
        FieldOfView fov = (FieldOfView)target;
        Handles.color = Color.white;
        Handles.DrawWireArc(fov.transform.position, Vector3.forward, Vector3.right, 360, fov.viewRadius);
        Vector2 viewAngleA = fov.DirFromAngle(-fov.viewAngle / 2, false);
        Vector2 viewAngleB = fov.DirFromAngle(fov.viewAngle / 2, false);

        Handles.DrawLine(fov.transform.position, (Vector2)fov.transform.position + viewAngleA * fov.viewRadius);
        Handles.DrawLine(fov.transform.position, (Vector2)fov.transform.position + viewAngleB * fov.viewRadius);

        Handles.color = Color.red;
        foreach (FieldOfView.VisibleTargetData visibleTarget in fov.visibleTargets)
        {
            Transform targetTransform = visibleTarget.transform;
            Handles.DrawLine (fov.transform.position, targetTransform.position);
        }
    }
}
