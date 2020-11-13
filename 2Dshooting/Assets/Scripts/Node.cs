using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : IHeapItem<Node>
{
    public bool walkable;
    public Vector3 cellPos;
    public Vector3 worldPos;

    
    public int row;
    public int col;
    public int movePenalty;    

    public int gCost;
    public int hCost;
    public Node parent;
    int heapIndex;

    public Node()
    {
        cellPos = Vector3.zero;
        walkable = true;
    }

    public Node(Vector3 pos)
    {
        cellPos = pos;
        walkable = true;
    }


    public Node(Vector3 _pos, Vector3 _world, bool _walkable, int _row, int _col, int _movePenalty)
    {
        walkable = _walkable;
        worldPos = _world;
        row = _row;
        col = _col;
        movePenalty = _movePenalty;
    }


    
    public int fCost
    {
        get { return gCost + hCost; }
    }

    public int HeapIndex
    {
        get
        {
            return heapIndex;
        }
        set
        {
            heapIndex = value;

        }
    }

    public int CompareTo(Node nodeToCompare)
    {
        int compare = fCost.CompareTo(nodeToCompare.fCost);

        if (compare == 0)
            compare = hCost.CompareTo(nodeToCompare.hCost);

        return -compare;
    }


}
