using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : IHeapItem<Node>
{
    public bool Walkable;
    public Vector3 worldPos;
    public int X;
    public int Y;

    public int g;
    public int h;
    public Node parent;
    int index;

    public Node(bool isWalkable, Vector3 NodeWorldPos, int x, int y)
    {
        Walkable = isWalkable;
        worldPos = NodeWorldPos;
        X = x;
        Y = y;
    }

    public int Cost
    {
        get
        {
            return g + h;
        }
    }

    public int HeapIndex
    {
        get
        {
            return index;
        }
        set
        {
            index = value;
        }
    }

    public int CompareTo(Node n)
    {
        int compare = Cost.CompareTo(n.Cost);
        if(compare == 0)
        {
            compare = h.CompareTo(n.h);
        }
        return -compare;
    }
}
