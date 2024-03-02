using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Grid : MonoBehaviour 
{
    public List<Node> path;

    public Vector2 gridSize;
    public float NodeSize;
    public LayerMask notWalkable;
    public Node[,] grid;
    public bool ShowDebugging;

    float NodeDiam;
    int gridXSize;
    int gridYSize;


    public static Grid instance;

    private void Awake()
    {
        instance = this;
    }

    private void OnDrawGizmos()
    {
        if(ShowDebugging)
        {
            Gizmos.DrawWireCube(transform.position, new Vector3(gridSize.x, 1, gridSize.y));
            if (grid != null)
            {
                Node playerNode = NodeFromWorldPoint(GameManager.instance.thePlayer.transform.position);
                foreach (Node n in grid)
                {
                    if (n.Walkable)
                    {
                        Gizmos.color = Color.white;
                    }
                    else
                    {
                        Gizmos.color = Color.red;
                    }
                    if (playerNode == n)
                    {
                        Gizmos.color = Color.yellow;
                    }
                    if (path != null)
                    {
                        if (path.Contains(n))
                        {
                            Gizmos.color = Color.blue;
                        }
                    }
                    Gizmos.DrawCube(n.worldPos, Vector3.one * (NodeDiam - 0.1f));
                }
            }
        }
    }

    private void Start()
    {
        NodeDiam = NodeSize * 2;
        gridXSize = Mathf.RoundToInt(gridSize.x / NodeDiam);
        gridYSize = Mathf.RoundToInt(gridSize.y / NodeDiam);
        CreateGrid();
    }

    //returns the Max size of the grid;
    public int Max
    {
        get
        {
            return gridXSize * gridYSize;
        }
    }

    void CreateGrid()
    {
        grid = new Node[gridXSize, gridYSize];
        Vector3 worldDownLeft = transform.position - Vector3.right * gridSize.x / 2 - Vector3.forward * gridSize.y / 2;
        for(int x = 0; x < gridXSize; x++)
        {
            for (int y = 0; y < gridYSize; y++)
            {
                bool walkable;
                Vector3 WorldPos = worldDownLeft + Vector3.right * (x * NodeDiam + NodeSize) + Vector3.forward * (y * NodeDiam + NodeSize);
                if(Physics.CheckSphere(WorldPos, NodeSize, notWalkable))
                {
                    walkable = false;
                }
                else
                {
                    walkable = true; 
                }
                grid[x, y] = new Node(walkable, WorldPos , x, y);
            }
        }
    }

    public Node NodeFromWorldPoint(Vector3 Pos)
    {
        float perX = (Pos.x + gridSize.x / 2) / gridSize.x;
        float perY = (Pos.z + gridSize.y / 2) / gridSize.y;
        perX = Mathf.Clamp01(perX);
        perY = Mathf.Clamp01(perY);

        int x = Mathf.RoundToInt((gridXSize - 1) * perX);
        int y = Mathf.RoundToInt((gridYSize - 1) * perY);

        return grid[x, y];
    }

    public List<Node> Neighbours(Node n)
    {
        List<Node> nList = new List<Node>();

        for(int x =-1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if(x == 0 && y == 0)
                {
                    continue;
                }
                int xCheck = n.X + x;
                int yCheck = n.Y + y;

                if(xCheck >= 0 && xCheck < gridXSize && yCheck >= 0 && yCheck < gridYSize)
                {
                    nList.Add(grid[xCheck, yCheck]);
                }
            }
        }

        return nList;
    }

    public Node RandomPoint()
    {
        List<Node> WalkableNodes = new List<Node>();
        foreach(Node n in grid)
        {
            if(n.Walkable)
            {
                WalkableNodes.Add(n);
            }
        }
        if(WalkableNodes.Count > 0)
        {
            int rand = Random.Range(0, WalkableNodes.Count);
            return WalkableNodes[rand];
        }
        else
        {
            Debug.Log("No Walkable Nodes found");
            return null;
        }
    }

    public Vector3 WorldPointFromNode(Node node)
    {
        return node.worldPos;
    }
}
