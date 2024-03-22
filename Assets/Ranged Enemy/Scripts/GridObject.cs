using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridObject : MonoBehaviour
{
    //public variables
    public Vector2 gridSize; //the size of the grid
    public float nodeSize; //the size of the nodes
    [Header("Only for debugging")]
    public bool Debugging = false; //bool to check if debugging is on

    void Start()
    {
        Grid.instance.Debugging = Debugging; //sets the debugging bool in the grid to the debugging bool in this script
        Grid.instance.gridSize = gridSize; //sets the grid size in the grid to the grid size in this script
        Grid.instance.nodeSize = nodeSize; //sets the node size in the grid to the node size in this script
        Grid.instance.InitializeGrid(); //initializes the grid
        Grid.instance.CreateNodeGrid(); //creates the node grid
    }

}
