using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Unit : MonoBehaviour
{

    public float speed = 5f;
    public float jumpHieght;
    public Transform target;
    Vector3[] path;
    int targetIndex;
    bool isTraversing = false;
    Vector2 currentWayPoint;
    Vector3 playerPos;
    
    void Start()
    {
        //PathRequestManager.RequestPath(transform.position, target.position, OnPathFound);
        if(target == null)
            target = transform.parent.GetComponent<EnemySpawner>().player;
        playerPos = target.position;
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
            PathRequestManager.RequestPath(transform.position, target.position, OnPathFound);
        Traverse();
        if((playerPos-target.position).magnitude>3f){
            PathRequestManager.RequestPath(transform.position, target.position, OnPathFound);
            playerPos = target.position;
        }
    }

    public void OnPathFound(Vector3[] newPath, bool pathFound){
        if(pathFound){
            path = newPath;
            currentWayPoint = path[0];
            targetIndex = 0;
            isTraversing = true;
        }
    }

    void Traverse(){
        if(isTraversing){
            if ((Vector2)transform.position == currentWayPoint){
                targetIndex++;
                if (targetIndex >= path.Length){
                    print("found");
                    isTraversing = false;
                    return;
                }
                currentWayPoint = path[targetIndex];
            }
            DecideMove();
        }
    }

    void DecideMove()
    {
        float DestinationHieght = transform.position.y - currentWayPoint.y;
        print(DestinationHieght);
        if(-DestinationHieght<jumpHieght)
            transform.position = Vector2.MoveTowards(transform.position, currentWayPoint, speed * Time.deltaTime);
    }

    private void OnDrawGizmosSelected() {
        if(path!=null){
            for(int i=targetIndex;i<path.Length;i++){
                Gizmos.color = Color.black;
                Gizmos.DrawCube(path[i],Vector2.one);
                if(i==targetIndex){
                    Gizmos.DrawLine(transform.position,path[i]);
                }else{
                    Gizmos.DrawLine(path[i-1],path[i]);
                }
            }
        }
    }

}
