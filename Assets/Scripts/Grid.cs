using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Grid : MonoBehaviour
{
    [HideInInspector] public List<Node> path;
    [HideInInspector] public Vector2 gridSize;
    [HideInInspector] public float nodeSize;
    [HideInInspector] public Node[,] nodeGrid;
    [HideInInspector] public bool Debugging = false;

    float nodeDiameter;
    int gridXSize;
    int gridYSize;

    public static Grid instance;

    private void Awake()
    {
        instance = this;
    }

    public int MaxSize
    {
        get
        {
            return gridXSize * gridYSize;
        }
    }

    public void InitializeGrid()
    {
        nodeDiameter = nodeSize * 2;
        gridXSize = (int)(gridSize.x / nodeDiameter);
        gridYSize = (int)(gridSize.y / nodeDiameter);
    }

    public void CreateNodeGrid()
    {
        nodeGrid = new Node[gridXSize, gridYSize];
        Vector3 worldDownLeft = transform.position - Vector3.right * gridSize.x / 2 - Vector3.forward * gridSize.y / 2;

        for (int x = 0; x < gridXSize; x++)
        {
            for (int y = 0; y < gridYSize; y++)
            {
                bool isWalkable = true;
                Vector3 worldPos = worldDownLeft + Vector3.right * (x * nodeDiameter + nodeSize) + Vector3.forward * (y * nodeDiameter + nodeSize);

                Collider[] colliders = Physics.OverlapSphere(worldPos, nodeSize);
                foreach (Collider collider in colliders)
                {
                    if(collider.GetComponent<Obstacle>() != null)
                    {
                        Obstacle obstacle = collider.GetComponent<Obstacle>();
                        if (obstacle != null && !obstacle.isWalkable)
                        {
                            isWalkable = false;
                            break;
                        }
                    }
                }
                nodeGrid[x, y] = new Node(isWalkable, worldPos, x, y);
            }
        }
    }

    public Node NodeFromWorldPoint(Vector3 position)
    {
        float percentX = (position.x + gridSize.x / 2) / gridSize.x;
        float percentY = (position.z + gridSize.y / 2) / gridSize.y;
        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);

        int x = Mathf.RoundToInt((gridXSize - 1) * percentX);
        int y = Mathf.RoundToInt((gridYSize - 1) * percentY);

        return nodeGrid[x, y];
    }

    public List<Node> GetNeighbours(Node node)
    {
        List<Node> neighboursList = new List<Node>();

        for (int xOffset = -1; xOffset <= 1; xOffset++)
        {
            for (int yOffset = -1; yOffset <= 1; yOffset++)
            {
                if (xOffset == 0 && yOffset == 0)
                {
                    continue;
                }

                int xCheck = node.X + xOffset;
                int yCheck = node.Y + yOffset;

                if (xCheck >= 0 && xCheck < gridXSize && yCheck >= 0 && yCheck < gridYSize)
                {
                    neighboursList.Add(nodeGrid[xCheck, yCheck]);
                }
            }
        }

        return neighboursList;
    }

    public Node GetRandomWalkableNode()
    {
        List<Node> walkableNodes = new List<Node>(nodeGrid.Cast<Node>().Where(node => node.Walkable));

        if (walkableNodes.Count > 0)
        {
            int randomIndex = Random.Range(0, walkableNodes.Count);
            return walkableNodes[randomIndex];
        }
        else
        {
            Debug.Log("No Walkable Nodes found");
            return null;
        }
    }

    //calculates the heuristic using octile distance
    public float CalculateOctileHeuristic(Node currentNode, Node targetNode)
    {
        int dx = Mathf.Abs(currentNode.X - targetNode.X);
        int dy = Mathf.Abs(currentNode.Y - targetNode.Y);

        int F = 14; // Movement cost for diagonal movement
        int D = 10; // Movement cost for horizontal/vertical movement

        float h = F * Mathf.Min(dx, dy) + D * Mathf.Abs(dx - dy);

        return h;
    }

    //calculates the heuristic using Manhattan distance
    public float CalculateManhattanHeuristic(Node currentNode, Node targetNode)
    {
        int dx = Mathf.Abs(currentNode.X - targetNode.X);
        int dy = Mathf.Abs(currentNode.Y - targetNode.Y);

        float h = dx + dy;

        return h;
    }

    // calculates the heuristic using Euclidean distance
    public float CalculateEuclideanHeuristic(Node currentNode, Node targetNode)
    {
        float dx = Mathf.Abs(currentNode.X - targetNode.X);
        float dy = Mathf.Abs(currentNode.Y - targetNode.Y);

        float h = Mathf.Sqrt(dx * dx + dy * dy);

        return h;
    }

    // calculates the heuristic using Diagonal distance
    public float CalculateDiagonalHeuristic(Node currentNode, Node targetNode)
    {
        int dx = Mathf.Abs(currentNode.X - targetNode.X);
        int dy = Mathf.Abs(currentNode.Y - targetNode.Y);

        float h = Mathf.Max(dx, dy);

        return h;
    }

    public int GetDist(Node a, Node b) //will check how many nodes away the node is away from the target Position
    {
        int DistanceX = Mathf.Abs(a.X - b.X);
        int DistanceY = Mathf.Abs(a.Y - b.Y);

        if (DistanceX < DistanceY)
        {
            return 14 * DistanceY + 10 * (DistanceX - DistanceY);
        }
        else
        {
            return 14 * DistanceX + 10 * (DistanceY - DistanceX);
        }
    }

    public Vector3 WorldPointFromNode(Node node)
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
