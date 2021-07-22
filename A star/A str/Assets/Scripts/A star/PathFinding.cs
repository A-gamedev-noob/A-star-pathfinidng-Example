using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;

public class PathFinding : MonoBehaviour
{

    public Transform seeker,target;
    PathRequestManager requestManager;

    Grid grid;

    private void Awake() {
        grid = GetComponent<Grid>();
        requestManager = GetComponent<PathRequestManager>();
    }
    

    public void StartPathFinding(Vector3 startPos, Vector3 targetPos,float JumpHieght){
        StartCoroutine(FindingPath(startPos,targetPos,JumpHieght));
    }

    IEnumerator FindingPath(Vector3 startPos, Vector3 targetPos,float jumpHieght){
        Stopwatch sw = new Stopwatch();
        sw.Start();

        Vector3[] wayPoints = new Vector3[0];
        bool pathFound = false;

        Node startNode = grid.NodeFromWorldPoint(startPos);
        Node targetNode = grid.NodeFromWorldPoint(targetPos);

        if(startNode.walkable && targetNode.walkable){
            Heap<Node> openSet = new Heap<Node>(grid.MaxSize);
            HashSet<Node> closedSet = new HashSet<Node>();

            openSet.Add(startNode);
            while (openSet.Count > 0)
            {
                Node currentNode = openSet.RemoveFirst();

                closedSet.Add(currentNode);

                if (currentNode == targetNode)
                {
                    sw.Stop();
                    pathFound = true;
                    
                    break;
                }

                foreach (Node neighbour in grid.GetNeighbours(currentNode))
                {
                    if (!neighbour.walkable || closedSet.Contains(neighbour))
                        continue;

                    int newMovementCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbour) + neighbour.movementPenalty ;//+ HieghtPenalty(neighbour, startPos, jumpHieght);
                    if (newMovementCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
                    {
                        neighbour.gCost = newMovementCostToNeighbour;
                        neighbour.hCost = GetDistance(neighbour, targetNode);
                        neighbour.parent = currentNode;

                        if (!openSet.Contains(neighbour))
                            openSet.Add(neighbour);
                        else
                            openSet.UpdateItem(neighbour);
                    }
                }
            }
        }
        yield return null;
        if(pathFound){
            wayPoints = RetracePath(startNode, targetNode);
        }
        requestManager.FinishedPathFinding(wayPoints,pathFound);
    }

    void FindPath(Vector3 startPos,Vector3 targetPos)
    {
        Stopwatch sw = new Stopwatch();
        sw.Start();
        Node startNode = grid.NodeFromWorldPoint(startPos);
        Node targetNode = grid.NodeFromWorldPoint(targetPos);

        Heap<Node> openSet = new Heap<Node>(grid.MaxSize);
        HashSet<Node> closedSet = new HashSet<Node>();

        openSet.Add(startNode);
        while(openSet.Count>0)
        {
            Node currentNode = openSet.RemoveFirst();

            closedSet.Add(currentNode);

            if(currentNode == targetNode)
            {
                sw.Stop();
                RetracePath(startNode,targetNode);
                return;
            }

            foreach(Node neighbour in grid.GetNeighbours(currentNode))
            {
                if(!neighbour.walkable || closedSet.Contains(neighbour))
                    continue;

                int newMovementCostToNeighbour = currentNode.gCost + GetDistance(currentNode,neighbour);
                if(newMovementCostToNeighbour<neighbour.gCost || !openSet.Contains(neighbour))
                {
                    neighbour.gCost = newMovementCostToNeighbour;
                    neighbour.hCost = GetDistance(neighbour,targetNode);
                    neighbour.parent = currentNode;

                    if(!openSet.Contains(neighbour))
                        openSet.Add(neighbour);
                }
            }
        }

    }

    int GetDistance(Node nodeA,Node nodeB)
    {
        int dstX = Mathf.Abs(nodeA.gridPos.x - nodeB.gridPos.x);
        int dstY = Mathf.Abs(nodeA.gridPos.y - nodeB.gridPos.y);

        if(dstX>dstY)
            return 14*dstY + 10*(dstX-dstY);
        return 14 * dstX + 10 * (dstY - dstX); 
    }

    Vector3[] RetracePath(Node startNode,Node endNode)
    {
        List<Node> path = new List<Node>();
        Node currentNode = endNode;
        while(currentNode!=startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.parent;
            if(currentNode==startNode)
                path.Add(currentNode);
        }

        grid.path = path;
        path.Reverse();
        Vector3[] wayPoints = SimplifyPath(path);
       // Array.Reverse(wayPoints);

        return wayPoints;
    }

    Vector3[] SimplifyPath(List<Node> path){
        List<Vector3> wayPoints = new List<Vector3>();
        Vector2 directionOld = Vector2.zero;
        for(int i=1;i<path.Count;i++){

            Vector2 directionNew = new Vector2(path[i-1].gridPos.x - path[i].gridPos.x, path[i-1].gridPos.y - path[i].gridPos.y);


            if((directionOld!=directionNew || i==path.Count-1)){
                wayPoints.Add(path[i-1].worldPosition);
            }
            directionOld = directionNew;
        }
        List<Vector3> pointsToRemove = new List<Vector3>();
        for(int x=0;x<wayPoints.Count-1;x++){
            int nextIndex = Mathf.Clamp(x + 1, x + 1, wayPoints.Count - 1);
            if(Vector2.Distance(wayPoints[x],wayPoints[nextIndex])<5f){
                pointsToRemove.Add(wayPoints[x]);
            }
        }

        foreach(Vector3 point in pointsToRemove){
            wayPoints.Remove(point);
        }
        return wayPoints.ToArray();
    }

    int HieghtPenalty(Node node, Vector3 unitPos, float jumpHieght)
    {
        float YDistance = Mathf.Abs(node.worldPosition.y - unitPos.y);
        if(YDistance > jumpHieght)
            return 0;
        return 0;
    }

}
