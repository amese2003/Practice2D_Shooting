using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node
{
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


    public Node(Vector3 pos, Vector3 world, bool _walkable, int _col, int _row)
    {
        cellPos = pos;
        worldPos = world;
        walkable = _walkable;

        col = _col;
        row = _row;
    }


    public Vector3 cellPos;
    public Vector3 worldPos;


    public int col;
    public int row;


    public bool walkable;

    public int gCost;
    public int hCost;
    public int fCost
    {
        get { return gCost + hCost; }
    }

    public Node parent;
}
