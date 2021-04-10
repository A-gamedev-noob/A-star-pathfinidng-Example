using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : IHeapItem<Node>
{
    public bool walkable;  
    public Vector3 worldPosition;
    public Vector2Int gridPos;

    public int gCost;
    public int hCost;
    public Node parent;
    int heapIndex;

    public int fCost{
        get{
            return gCost+hCost;
        }
    }

    public Node(bool _walkable, Vector3 _worldPosition, int gridX, int gridY)
    {
        walkable = _walkable;
        worldPosition = _worldPosition;
        gridPos = new Vector2Int(gridX,gridY);
    }

    public int HeapIndex{
        get{
            return heapIndex;
        }
        set{
            heapIndex = value;
        }
    }

    public int CompareTo(Node nodeToCompare){
        int compare = fCost.CompareTo(nodeToCompare.fCost);
        if(compare == 0){
            compare = hCost.CompareTo(nodeToCompare.hCost);
        }
        return -compare;
    }

}
