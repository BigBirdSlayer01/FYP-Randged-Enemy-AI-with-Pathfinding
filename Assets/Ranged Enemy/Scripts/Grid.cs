using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Grid : MonoBehaviour
{
    [HideInInspector] public List<Node> path; //list of nodes that will store the path
    [HideInInspector] public Vector2 gridSize; //the size of the grid
    [HideInInspector] public float nodeSize; //the size of the nodes
    [HideInInspector] public Node[,] nodeGrid; //the grid of nodes
    [HideInInspector] public bool Debugging = false; //bool to check if debugging is on

    float nodeDiameter; //the diameter of the node
    int gridXSize; //the x size of the grid
    int gridYSize;  //the y size of the grid

    public static Grid instance;  //instance of the grid

    private void Awake()
    {
        instance = this; //sets the instance to this
    }

    public int MaxSize
    {
        get
        {
            return gridXSize * gridYSize; //returns the max size of the grid
        }
    }

    public void InitializeGrid()
    {
        nodeDiameter = nodeSize * 2; //sets the diameter of the node
        gridXSize = (int)(gridSize.x / nodeDiameter); //sets the x size of the grid
        gridYSize = (int)(gridSize.y / nodeDiameter); //sets the y size of the grid
    }

    public void CreateNodeGrid() //creates the grid of node used for a* pathfinding
    {
        nodeGrid = new Node[gridXSize, gridYSize]; //creates a new grid of nodes
        Vector3 worldDownLeft = transform.position - Vector3.right * gridSize.x / 2 - Vector3.forward * gridSize.y / 2; //gets the bottom left of the grid

        for (int x = 0; x < gridXSize; x++) //for each node in x axis
        {
            for (int y = 0; y < gridYSize; y++) //for each node in y axis
            {
                bool isWalkable = true; //sets the node to walkable by default
                //gets the world position of the node
                Vector3 worldPos = worldDownLeft + Vector3.right * (x * nodeDiameter + nodeSize) + Vector3.forward * (y * nodeDiameter + nodeSize); 
                Collider[] colliders = Physics.OverlapSphere(worldPos, nodeSize); //checks for colliders in the node
                foreach (Collider collider in colliders) //for each collider on the nodes
                {
                    if(collider.GetComponent<Obstacle>() != null) //if the collider has the obstacle script
                    {
                        Obstacle obstacle = collider.GetComponent<Obstacle>(); //gets the obstacle script
                        if (!obstacle.isWalkable) //if the obstacle is not walkable
                        {
                            isWalkable = false; //sets the node to not walkable
                            break; //breaks the loop
                        }
                    }
                }
                nodeGrid[x, y] = new Node(isWalkable, worldPos, x, y); //creates a new node
            }
        }
    }

    public Node NodeFromWorldPoint(Vector3 position)
    {
        float percentX = (position.x + gridSize.x / 2) / gridSize.x; //gets the percentage of the x position
        float percentY = (position.z + gridSize.y / 2) / gridSize.y; //gets the percentage of the y position
        percentX = Mathf.Clamp01(percentX); //clamps the x position
        percentY = Mathf.Clamp01(percentY); //clamps the y position

        int x = Mathf.RoundToInt((gridXSize - 1) * percentX); //gets the x position
        int y = Mathf.RoundToInt((gridYSize - 1) * percentY); //gets the y position

        return nodeGrid[x, y]; //returns the node
    }

    public List<Node> GetNeighbours(Node node) //gets the neighbours of the node
    {
        List<Node> neighboursList = new List<Node>(); //creates a new list of nodes

        for (int xOffset = -1; xOffset <= 1; xOffset++) //for each node in the x axis
        {
            for (int yOffset = -1; yOffset <= 1; yOffset++) //for each node in the y axis
            {
                if (xOffset == 0 && yOffset == 0) //if the node is the same as the current node
                {
                    continue; //continue the loop
                }

                int xCheck = node.X + xOffset; //gets the x position of the node
                int yCheck = node.Y + yOffset; //gets the y position of the node

                if (xCheck >= 0 && xCheck < gridXSize && yCheck >= 0 && yCheck < gridYSize) //if the node is within the grid
                {
                    neighboursList.Add(nodeGrid[xCheck, yCheck]); //adds the node to the list
                }
            }
        }

        return neighboursList; //returns the list
    }

    public Node GetRandomWalkableNode() //gets a random walkable node
    {
        List<Node> walkableNodes = new List<Node>(nodeGrid.Cast<Node>().Where(node => node.Walkable)); //creates a list of walkable nodes

        if (walkableNodes.Count > 0) //if there are walkable nodes
        {
            int randomIndex = Random.Range(0, walkableNodes.Count); //gets a random index
            return walkableNodes[randomIndex]; //returns the node
        }
        else //if there are no walkable nodes
        {
            Debug.Log("No Walkable Nodes found"); //logs that there are no walkable nodes
            return null; //returns null
        }
    }


    //calculates the heuristic using octile distance
    public float CalculateOctileHeuristic(Node currentNode, Node targetNode) //calculates the heuristic using octile distance
    {
        int dx = Mathf.Abs(currentNode.X - targetNode.X); //gets the x distance
        int dy = Mathf.Abs(currentNode.Y - targetNode.Y); //gets the y distance

        int F = 14; // Movement cost for diagonal movement
        int D = 10; // Movement cost for horizontal/vertical movement

        float h = F * Mathf.Min(dx, dy) + D * Mathf.Abs(dx - dy); //calculates the heuristic

        return h;
    }

    //calculates the heuristic using Manhattan distance
    public float CalculateManhattanHeuristic(Node currentNode, Node targetNode)
    {
        int dx = Mathf.Abs(currentNode.X - targetNode.X); //gets the x distance
        int dy = Mathf.Abs(currentNode.Y - targetNode.Y); //gets the y distance

        float h = dx + dy; //calculates the heuristic
         
        return h;
    }

    // calculates the heuristic using Euclidean distance
    public float CalculateEuclideanHeuristic(Node currentNode, Node targetNode)
    {
        float dx = Mathf.Abs(currentNode.X - targetNode.X); //gets the x distance
        float dy = Mathf.Abs(currentNode.Y - targetNode.Y); //gets the y distance

        float h = Mathf.Sqrt(dx * dx + dy * dy); //calculates the heuristic

        return h;
    }

    // calculates the heuristic using Diagonal distance
    public float CalculateDiagonalHeuristic(Node currentNode, Node targetNode)
    {
        int dx = Mathf.Abs(currentNode.X - targetNode.X); //gets the x distance
        int dy = Mathf.Abs(currentNode.Y - targetNode.Y); //gets the y distance

        float h = Mathf.Max(dx, dy); //calculates the heuristic

        return h;
    }

    public int GetDist(Node a, Node b) //will check how many nodes away the node is away from the target Position
    {
        int DistanceX = Mathf.Abs(a.X - b.X); //gets the x distance
        int DistanceY = Mathf.Abs(a.Y - b.Y); //gets the y distance

        if (DistanceX < DistanceY) //if the x distance is less than the y distance
        {
            return 14 * DistanceY + 10 * (DistanceX - DistanceY); //returns the distance
        }
        else //if the y distance is less than the x distance
        {
            return 14 * DistanceX + 10 * (DistanceY - DistanceX); //returns the distance
        }
    }

    public Vector3 WorldPointFromNode(Node node) //gets the world position of the node
    {
        return node.worldPos;
    }

    private void OnDrawGizmos()
    {
        if (nodeGrid != null && Debugging)
        {
            foreach (Node node in nodeGrid)
            {
                if (node.Walkable)
                {
                    Gizmos.color = Color.white;
                }
                else
                {
                    Gizmos.color = Color.red;
                }
                Gizmos.DrawCube(node.worldPos, Vector3.one * (nodeSize - 0.1f));
            }
        }
    }
}
