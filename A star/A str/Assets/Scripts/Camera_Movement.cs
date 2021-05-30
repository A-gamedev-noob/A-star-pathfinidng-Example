using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera_Movement : MonoBehaviour
{

    public Transform ply;

    void LateUpdate()
    {
        transform.position = new Vector3(ply.position.x,ply.position.y,transform.position.z);
    }
}
