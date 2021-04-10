using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{
  
    public bool drawGizmos;
    public Transform ply;
    public LayerMask unwalkable;
    public Vector2 gridWorldSize;
    public float nodeRadius;
    Node[,] grid;

    public List<Node> path;

    int gridSizeX,gridSizeY;
    float nodeDiameter;

    void Start()
    {
        nodeDiameter = nodeRadius*2;
        gridSizeX = Mathf.RoundToInt(gridWorldSize.x/nodeDiameter);
        gridSizeY = Mathf.RoundToInt(gridWorldSize.y/nodeDiameter);
        CreateGrid();
    }

    public int MaxSize{
        get{
            return gridSizeX*gridSizeY;
        }
    }

    void CreateGrid()
    {
        grid = new Node[gridSizeX,gridSizeY];
        Vector3 worldBottomLeft = transform.position - Vector3.right*gridWorldSize.x/2 - Vector3.up*gridWorldSize.y/2;

        for (int x=0;x<gridSizeX;x++)
        {
            for (int y=0;y<gridSizeY;y++)
            {
                Vector3 worlPoint = worldBottomLeft + Vector3.right*(x*nodeDiameter+nodeRadius) + Vector3.up*(y*nodeDiameter+nodeRadius);
                bool walkable = !(Physics2D.OverlapCircle(worlPoint,nodeRadius-.1f,unwalkable));
                grid[x,y] = new Node(walkable,worlPoint,x,y);
            }
        }
    }

    public List<Node> GetNeighbours(Node node)
    {
        List<Node> neighbours = new List<Node>();
        for(int x=-1;x<=1;x++)
        {
            for (int y = -1;  y<= 1;  y++)
            {
                if(x==0 && y==0)
                    continue;

                Vector2Int checkCord = new Vector2Int();

                checkCord = node.gridPos + new Vector2Int(x,y);
                if(checkCord.x>=0 && checkCord.x<gridSizeX && checkCord.y >= 0 && checkCord.y < gridSizeY)
                {
                    neighbours.Add(grid[checkCord.x,checkCord.y]);
                }
                
            }
        }

        return neighbours;
    }

    public Node NodeFromWorldPoint(Vector3 worldPos)
    {
        float percentX = (worldPos.x + gridWorldSize.x/2)/gridWorldSize.x;
        float percentY = (worldPos.y + gridWorldSize.y/2)/gridWorldSize.y;
        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);
        int x = Mathf.RoundToInt((gridSizeX-1)*percentX);
        int y = Mathf.RoundToInt((gridSizeY-1)*percentY);
        return grid[x,y];
    }

    // private void OnDrawGizmos() {
    //     Gizmos.DrawWireCube(transform.position,gridWorldSize);
    //     if(grid!=null)
    //     {
    //         if(!drawGizmos){
    //             if(path!=null){
    //                 foreach (Node n in grid)
    //                 {
    //                     if (path.Contains(n))
    //                     {
    //                         Gizmos.color = Color.black;
    //                         Gizmos.DrawCube(n.worldPosition, Vector3.one * (nodeDiameter - 0.1f));
    //                     }

    //                 }
    //             }
               
    //         }else{
    //             foreach (Node n in grid)
    //             {
    //                 Gizmos.color = (n.walkable) ? Color.green : Color.red;
    //                 if (path != null)
    //                 {
    //                     if (path.Contains(n))
    //                         Gizmos.color = Color.blue;
    //                     Gizmos.DrawCube(n.worldPosition, Vector3.one * (nodeDiameter - 0.1f));
    //                 }
    //             }
    //         }
            
    //     }
    // }
  
}
