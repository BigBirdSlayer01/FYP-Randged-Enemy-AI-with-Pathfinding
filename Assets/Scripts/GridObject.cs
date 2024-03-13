using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridObject : MonoBehaviour
{
    //public variables
    public Vector2 gridSize;
    public float nodeSize;
    [Header("Only for debugging")]
    public bool Debugging = false;

    void Start()
    {
        Grid.instance.Debugging = Debugging;
        Grid.instance.gridSize = gridSize;
        Grid.instance.nodeSize = nodeSize;
        Grid.instance.InitializeGrid();
        Grid.instance.CreateNodeGrid();
    }

}
