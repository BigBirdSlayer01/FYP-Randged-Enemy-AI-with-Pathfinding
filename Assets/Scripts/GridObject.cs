using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridObject : MonoBehaviour
{
    //public variables
    public Vector2 gridSize;
    public float nodeSize;

    void Start()
    {
        Grid.instance.gridSize = gridSize;
        Grid.instance.nodeSize = nodeSize;
        Grid.instance.InitializeGrid();
        Grid.instance.CreateNodeGrid();
    }

}
