using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathRequestManager : MonoBehaviour
{
    Queue<PathRequest> pathRequestQueue = new Queue<PathRequest>();
    PathRequest currentPathRequest;
    PathFinding pathFinding;
    bool isProcessingPath;

    #region Singletone
    private static PathRequestManager _instance;
    public static PathRequestManager Instance{
        get{
            if(_instance==null)
                _instance = FindObjectOfType<PathRequestManager>();
            return _instance;
        }
    }
    #endregion

    private void Awake() {
        pathFinding = GetComponent<PathFinding>();
    }

    public static void RequestPath(Vector3 pathStart, Vector3 pathEnd, Action<Vector3[],bool> callback, float jumpHieght){
        PathRequest newRequest = new PathRequest(pathStart, pathEnd, callback, jumpHieght);
        Instance.pathRequestQueue.Enqueue(newRequest); 
        Instance.TryProcessNext();
    }
    
    void TryProcessNext(){
        if(!isProcessingPath && pathRequestQueue.Count>0){
            currentPathRequest = pathRequestQueue.Dequeue();
            isProcessingPath =  true;
            pathFinding.StartPathFinding(currentPathRequest.pathStart, currentPathRequest.pathEnd,currentPathRequest.jumpHieght);
        }
    }

    public void FinishedPathFinding(Vector3[] path, bool success){
        currentPathRequest.callback(path,success);
        isProcessingPath = false;
        TryProcessNext();
    }

    struct PathRequest{
        public Vector3 pathStart;
        public Vector3 pathEnd;
        public Action<Vector3[], bool> callback;

        public float jumpHieght;

        public PathRequest(Vector3 _start, Vector3 _end, Action<Vector3[], bool> _callback,float _jumpHieght){
            pathStart = _start;
            pathEnd = _end;
            callback = _callback;
            jumpHieght = _jumpHieght;
        }
    }

}
