using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node
{
    public bool Walkable; //if the node is walkable
    public Vector3 worldPos; //the world position of the node
    public int X; //the x position of the node
    public int Y; //the y position of the node

    public int g; //the g cost of the node
    public int h; //the h cost of the node
    public Node parent; //the parent of the node
    int index; //the index of the node in the heap

    public Node(bool isWalkable, Vector3 NodeWorldPos, int x, int y) //constructor for the node
    {
        Walkable = isWalkable; //sets the walkable bool to the isWalkable bool
        worldPos = NodeWorldPos; //sets the world position of the node to the NodeWorldPos
        X = x; //sets the x position of the node to x
        Y = y; //sets the y position of the node to y
    } 

    public int Cost
    {
        get
        {
            return g + h; //returns the g cost + the h cost
        }
    }

    public int HeapIndex
    {
        get
        {
            return index; //returns the index
        }
        set
        {
            index = value; //sets the index to the value
        }
    }
}
