using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldOfView : MonoBehaviour
{
    public float viewRadius;
    [Range(0,360)]
    public float upperViewAngle, lowerViewAngle;
    Transform ply;

    void Start(){
        ply = transform.GetComponent<Unit>().playerPos;
    }

    public Vector2 DiFromAngle(float angleInDeg){
        return new Vector2(Mathf.Sin(angleInDeg*Mathf.Deg2Rad), Mathf.Cos(angleInDeg*Mathf.Deg2Rad));
    }

    public bool IsSteep(){
        Vector2 dirToTarget = (ply.position-transform.position).normalized;
        bool notVisible = Physics2D.Raycast(transform.position,dirToTarget,Vector2.Distance(transform.position,ply.position),GetComponent<Unit>().groundLayer);
        if((Vector2.Angle(transform.up, dirToTarget)<upperViewAngle/2) || (Vector2.Angle(transform.up*-1, dirToTarget) < lowerViewAngle / 2)){
            return true;
        }
        return false;
    } 

}
