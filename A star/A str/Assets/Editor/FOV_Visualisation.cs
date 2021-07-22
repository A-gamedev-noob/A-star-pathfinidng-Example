using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(FieldOfView))]
public class FOV_Visualisation : Editor
{
   private void OnSceneGUI() {
        FieldOfView fov = (FieldOfView)target;
        Handles.color = Color.white;
        Handles.DrawWireArc(fov.transform.position,Vector3.forward, Vector3.up,360,fov.viewRadius);

        Vector2 viewAnglA = fov.DiFromAngle(-fov.upperViewAngle/2);
        Vector2 viewAnglB = fov.DiFromAngle(fov.upperViewAngle/2);

        Vector2 viewAnglC = fov.DiFromAngle(-fov.lowerViewAngle / 2);
        Vector2 viewAnglD = fov.DiFromAngle(fov.lowerViewAngle / 2);

        Handles.DrawLine(fov.transform.position, fov.transform.position + (Vector3)viewAnglA * fov.viewRadius);
        Handles.DrawLine(fov.transform.position, fov.transform.position + (Vector3)viewAnglB * fov.viewRadius);

        Handles.DrawLine(fov.transform.position, fov.transform.position - (Vector3)viewAnglC * fov.viewRadius);
        Handles.DrawLine(fov.transform.position, fov.transform.position - (Vector3)viewAnglD * fov.viewRadius);
   }
}
