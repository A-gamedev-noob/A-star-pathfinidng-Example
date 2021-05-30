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
    public TerrainType[] walkableRegion;
    public int proximityPenalty = 5;
    public LayerMask walkableMask;
    Dictionary<int,int> walkableRegionsDictionary = new Dictionary<int, int>();

    public List<Node> path;
    public int blur = 5;


    int gridSizeX,gridSizeY;
    float nodeDiameter;

    int penaltyMin = int.MaxValue;
    int penaltyMax = int.MinValue;

    void Awake()
    {
        nodeDiameter = nodeRadius*2;
        gridSizeX = Mathf.RoundToInt(gridWorldSize.x/nodeDiameter);
        gridSizeY = Mathf.RoundToInt(gridWorldSize.y/nodeDiameter);
        foreach(TerrainType region in walkableRegion){
            walkableMask |= region.terrainMask.value;
            walkableRegionsDictionary.Add((int)Mathf.Log(region.terrainMask.value,2),region.terrainPenalty);
        }
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
                bool walkable = !(Physics2D.OverlapCircle(worlPoint,nodeRadius-0.05f,unwalkable));

                int movementPenalty = 0;
                RaycastHit2D hit = Physics2D.CircleCast(worlPoint,nodeRadius- 0.05f,Vector2.zero);
                 if(hit.collider!=null){
                     walkableRegionsDictionary.TryGetValue(hit.transform.gameObject.layer,out movementPenalty);
                }

                if(!walkable)
                    movementPenalty += proximityPenalty;
                

                grid[x,y] = new Node(walkable,worlPoint,x,y,movementPenalty);
            }
        }

        BlurPenaltyMap(blur);
    }

    void BlurPenaltyMap(int blurSize)
    {
        int kernelSize = blurSize * 2 + 1;
        int kernelExtents = (kernelSize - 1) / 2;

        int[,] penaltiesHorizontalPass = new int[gridSizeX, gridSizeY];
        int[,] penaltiesVerticalPass = new int[gridSizeX, gridSizeY];

        for (int y = 0; y < gridSizeY; y++)
        {
            for (int x = -kernelExtents; x <= kernelExtents; x++)
            {
                int sampleX = Mathf.Clamp(x, 0, kernelExtents);
                penaltiesHorizontalPass[0, y] += grid[sampleX, y].movementPenalty;
            }

            for (int x = 1; x < gridSizeX; x++)
            {
                int removeIndex = Mathf.Clamp(x - kernelExtents - 1, 0, gridSizeX);
                int addIndex = Mathf.Clamp(x + kernelExtents, 0, gridSizeX - 1);

                penaltiesHorizontalPass[x, y] = penaltiesHorizontalPass[x - 1, y] - grid[removeIndex, y].movementPenalty + grid[addIndex, y].movementPenalty;
            }
        }

        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = -kernelExtents; y <= kernelExtents; y++)
            {
                int sampleY = Mathf.Clamp(y, 0, kernelExtents);
                penaltiesVerticalPass[x, 0] += penaltiesHorizontalPass[x, sampleY];
            }

            int blurredPenalty = Mathf.RoundToInt((float)penaltiesVerticalPass[x, 0] / (kernelSize * kernelSize));
            grid[x, 0].movementPenalty = blurredPenalty;

            for (int y = 1; y < gridSizeY; y++)
            {
                int removeIndex = Mathf.Clamp(y - kernelExtents - 1, 0, gridSizeY);
                int addIndex = Mathf.Clamp(y + kernelExtents, 0, gridSizeY - 1);

                penaltiesVerticalPass[x, y] = penaltiesVerticalPass[x, y - 1] - penaltiesHorizontalPass[x, removeIndex] + penaltiesHorizontalPass[x, addIndex];
                blurredPenalty = Mathf.RoundToInt((float)penaltiesVerticalPass[x, y] / (kernelSize * kernelSize));
                grid[x, y].movementPenalty = blurredPenalty;

                if (blurredPenalty > penaltyMax)
                {
                    penaltyMax = blurredPenalty;
                }
                if (blurredPenalty < penaltyMin)
                {
                    penaltyMin = blurredPenalty;
                }
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

    private void OnDrawGizmos() {
        Gizmos.DrawWireCube(transform.position,new Vector2(gridWorldSize.x,gridWorldSize.y));
        if(drawGizmos && grid!=null){
            foreach (Node n in grid){
                
                Gizmos.color = Color.Lerp(Color.white,Color.black, Mathf.InverseLerp(penaltyMin,penaltyMax,n.movementPenalty));
                
                Gizmos.color = (n.walkable) ? Gizmos.color : Color.red;
                if (path != null)
                {
                    if (path.Contains(n))
                        Gizmos.color = Color.black;
                }
                Gizmos.DrawCube(n.worldPosition, Vector3.one * (nodeDiameter)); 
            }   
        }
        
    }

    [System.Serializable]
    public class TerrainType
    {
        public LayerMask terrainMask;
        public int terrainPenalty;
    }
  
}
