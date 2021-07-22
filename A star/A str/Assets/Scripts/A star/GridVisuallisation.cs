using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class GridVisuallisation : MonoBehaviour
{

    public bool drawGizmos;
    public LayerMask unwalkable;
    public Vector2 gridWorldSize;
    public float nodeRadius;
    public LayerMask walkableMask;
    public TerrainType[] walkableRegion;

    int gridSizeX, gridSizeY;
    float nodeDiameter;
    Node[,] grid;
    Dictionary<int, int> walkableRegionsDictionary = new Dictionary<int, int>();


    int penaltyMin = int.MaxValue;
    int penaltyMax = int.MinValue;

    void Start()
    {
        unwalkable = GetComponent<Grid>().unwalkable;
        gridWorldSize = GetComponent<Grid>().gridWorldSize;
        nodeRadius = GetComponent<Grid>().nodeRadius;
        walkableMask = GetComponent<Grid>().walkableMask;

        walkableRegion = GetComponent<Grid>().walkableRegion;

        nodeDiameter = nodeRadius * 2;
        gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
        gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);
        foreach (TerrainType region in walkableRegion)
        {
            walkableMask |= region.terrainMask.value;
            walkableRegionsDictionary.Add((int)Mathf.Log(region.terrainMask.value, 2), region.terrainPenalty);
        }
        CreateGrid();
    }

    // Update is called once per frame
    void Update()
    {
        if(drawGizmos){
            grid = new Node[gridSizeX, gridSizeY];
            Vector3 worldBottomLeft = transform.position - Vector3.right * gridWorldSize.x / 2 - Vector3.up * gridWorldSize.y / 2;

            for (int x = 0; x < gridSizeX; x++)
            {
                for (int y = 0; y < gridSizeY; y++)
                {
                    Vector3 worlPoint = worldBottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.up * (y * nodeDiameter + nodeRadius);
                    bool walkable = !(Physics2D.OverlapCircle(worlPoint, nodeRadius - 0.1f, unwalkable));

                    int movementPenalty = 10;
                    if (walkable)
                    {
                        RaycastHit2D hit = Physics2D.CircleCast(worlPoint, nodeRadius - 0.05f, Vector2.zero);
                        if (hit.collider != null)
                        {
                            walkableRegionsDictionary.TryGetValue(hit.transform.gameObject.layer, out movementPenalty);
                        }
                    }
                    if (!walkable)
                        movementPenalty = 0;
                    grid[x, y] = new Node(walkable, worlPoint, x, y, movementPenalty);
                }
            }
        }
    }

    void CreateGrid()
    {
        grid = new Node[gridSizeX, gridSizeY];
        Vector3 worldBottomLeft = transform.position - Vector3.right * gridWorldSize.x / 2 - Vector3.up * gridWorldSize.y / 2;

        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                Vector3 worlPoint = worldBottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.up * (y * nodeDiameter + nodeRadius);
                bool walkable = !(Physics2D.OverlapCircle(worlPoint, nodeRadius - 0.1f, unwalkable));

                int movementPenalty = 10;
                if (walkable)
                {
                    RaycastHit2D hit = Physics2D.CircleCast(worlPoint, nodeRadius - 0.05f, Vector2.zero);
                    if (hit.collider != null)
                    {
                        walkableRegionsDictionary.TryGetValue(hit.transform.gameObject.layer, out movementPenalty);
                    }
                }
                if (!walkable)
                    movementPenalty = 0;
                grid[x, y] = new Node(walkable, worlPoint, x, y, movementPenalty);
            }
        }
        
    }



    void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireCube(transform.position, new Vector2(gridWorldSize.x, gridWorldSize.y));
        
        if (drawGizmos && grid != null)
        {
            foreach (Node n in grid)
            {

                Gizmos.color = Color.Lerp(Color.white, Color.black, Mathf.InverseLerp(penaltyMin, penaltyMax, n.movementPenalty));
                Gizmos.color = (n.walkable) ? Gizmos.color : Color.red;

                Gizmos.color = new Color(Gizmos.color.r,Gizmos.color.g,Gizmos.color.b, 0.5f);
                
                Gizmos.DrawCube(n.worldPosition, Vector3.one * (nodeDiameter - 0.1f));
            }
        }

    }
}

// [System.Serializable]
// public class TerrainType
// {
//     public LayerMask terrainMask;
//     public int terrainPenalty;
// }   
