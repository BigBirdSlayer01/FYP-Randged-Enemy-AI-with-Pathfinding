using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class AStar : MonoBehaviour
{
    //check this script for plagiarism
    //
    Grid g; //will store the grid reference
    List<Node> path = new List<Node>();
    public GameObject target; //target object (possible to not be used)
    [HideInInspector] public GameObject searcher; //object that will follow the path (the enemy object the script is attached to)

    [HideInInspector] public bool pathFound; //bool that will return true when path found 
    [HideInInspector] private bool _follow; //bool set to false when want to stop following the path

    [Header("Choose Heuristic Method")]
    public HeuristicMethod Heuristic = new HeuristicMethod(); //will decide which heuristic method that will be used
    public bool follow
    {
        get { return _follow; }
        set
        {
            _follow = value;
            onFollowChanged();
        }
    }
    Coroutine followThePath;
    
    int targetIndex; //index of the node the target is on

    private void Awake()
    {
        g = Grid.instance; //sets g to the grid index
        searcher = this.gameObject; // sets searcher to the object the script is stored on
    }

    private void Start()
    {
        pathFound = false; // path is not currently found 
        follow = true; // searcher should follow path
        g = Grid.instance; //sets g to the grid index
        searcher = this.gameObject; // sets searcher to the object the script is stored on
    }

    public void FindPath(Vector3 start, Vector3 target)
    {
        Pathfinding.aStar(start, target, ref g, ref targetIndex, ref path, ref pathFound, Heuristic); //calls the find path method from the pathfinding class
    }
    
    public void FollowPath()
    {
        followThePath = StartCoroutine(followingPath()); //calls the follow path method from the pathfinding class);
    }

    IEnumerator followingPath() //coroutine to follow the path
    {
        Vector3 currentPoint = path[targetIndex].worldPos; //gets current point of the target node
        while (follow) //while the follow bool is set to true
        {
            if ((int)searcher.transform.position.x == (int)currentPoint.x && (int)searcher.transform.position.z == (int)currentPoint.z)
            {
                targetIndex++;// Move to the next target node if the current one is reached
                if (targetIndex >= path.Count)
                {
                    yield break; // Exit the coroutine if all nodes are visite
                }
                currentPoint = path[targetIndex].worldPos; // Update the position of the current target node
            }
            // Calculate direction to move
            Vector3 direction = currentPoint - searcher.transform.position;
            direction.y = 0; // Ensure no rotation in the y-axis (vertical)

            // Rotate the searcher to face the direction of movement
            if (direction != Vector3.zero) // Check if there is a direction to move
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction, Vector3.up);
                searcher.transform.rotation = Quaternion.Slerp(searcher.transform.rotation, targetRotation, Time.deltaTime * 2);
            }
            searcher.transform.position = Vector3.MoveTowards(searcher.transform.position, currentPoint, Time.deltaTime * 10.0f); // Move towards the current target node
            yield return null;
        }
    }

    void onFollowChanged()
    {
        if(!follow && followThePath != null)
        {
            StopCoroutine(followThePath); //stops the coroutine
            path.Clear();
        }
    }

    private class Pathfinding // the class used to find the path
    {
        public static void aStar(Vector3 start, Vector3 target, ref Grid g, ref int targetIndex, ref List<Node> path, ref bool pathFound, HeuristicMethod Heuristic)
        {
            targetIndex = 0;
            Node startingNode = g.NodeFromWorldPoint(start); //gets the node from the world point
            Node endNode = g.NodeFromWorldPoint(target); //gets the node from the world point

            HashSet<Node> openSet = new HashSet<Node>(); //creates and initializes the open set
            HashSet<Node> closedSet = new HashSet<Node>(); //creates and initializes the closed set
            openSet.Add(startingNode); //adds the start node to the open set

            while(openSet.Count > 0) // while the open set is not empty 
            {
                Node currentNode = null; //creates a node and sets it to null
                float MinCost = float.MaxValue; // creates a float and sets it to the max posible float value
                foreach(Node node in openSet) //for each node in the open set
                {
                    float cost = node.Cost + CalculateHCost(node, endNode, Heuristic); //calculates the heuristic cost using a mehtod in grid
                    if (cost < MinCost) // if the cost is less than min cost
                    {
                        MinCost = cost; // sets the min cost to the calculated cost value
                        currentNode = node; //sets teh currentNode to the node in the foreach loop
                    }
                }

                openSet.Remove(currentNode); //remove the current node from the open set
                closedSet.Add(currentNode); //add that node to the closed set

                if(currentNode == endNode) //if the current node is the last node
                {
                    pathFound = true; //sets path found to true
                    TracePath(startingNode, endNode, ref path, ref pathFound);   //call the trace path method 
                    break; //break from the loop
                }

                //check Neighbours
                foreach(Node n in g.GetNeighbours(currentNode)) //for each neighbour of the current node
                {
                    if(closedSet.Contains(n) || !n.Walkable) //if the neighbour is in the closed set or not walkable
                    {
                        continue; //skip the rest of the loop
                    }

                    float gVal = n.g + g.GetDist(currentNode, n); //calculates the g value by getting the distance between the current node and the neighbour

                    //if the open set doesnt contain the neighbour or the gVal is less than the neighbours g value
                    if(!openSet.Contains(n) || gVal < n.g)
                    {
                        n.g = (int)gVal; //sets the g value
                        n.h = (int)CalculateHCost(n, endNode, Heuristic); //sets the new heuristic cost 
                        n.parent = currentNode; //sets the parent of the neighbour to the current node

                        if(!openSet.Contains(n)) //if the open set does not contain the neighbour
                        {
                            openSet.Add(n); //add the neighbour to the open set
                        }
                    }

                }
            }
        }

        static float CalculateHCost(Node startNode, Node endNode, HeuristicMethod Heuristic)
        {
            float h = 0;

            if(Heuristic == HeuristicMethod.Octile)
            {
                h = Grid.instance.CalculateOctileHeuristic(startNode, endNode); //sets the heuristic using the Octile method
            }
            else if(Heuristic == HeuristicMethod.Euclidean)
            {
                h = Grid.instance.CalculateEuclideanHeuristic(startNode, endNode); //sets the heuristic using the Euclidean method
            }
            else if (Heuristic == HeuristicMethod.Diagonal)
            {
                h = Grid.instance.CalculateDiagonalHeuristic(startNode, endNode); //sets the heuristic using the Diagonal method
            }
            else if (Heuristic == HeuristicMethod.Manhattan)
            {
                h = Grid.instance.CalculateManhattanHeuristic(startNode, endNode); //sets the heuristic using the Manhattan method
            }

            return h; //returns the heuristic value 
        }

        static void TracePath(Node start, Node Finish, ref List<Node> path, ref bool pathFound) //reverses the path to the correct order and stores in the path
        {
            path.Clear(); // Empty the path list

            Node curr = Finish; // current node set to last node

            while (curr != start) //while the current node is not the start node
            {
                path.Add(curr); //adds each node to the path
                curr = curr.parent; // moves to the parent
            }

            path.Reverse(); // reverses path to make it from start to finish

            for (int i = 0; i < path.Count - 1; i++) //draws the line for debugging purposes
            {
                Debug.DrawLine(Grid.instance.WorldPointFromNode(path[i]), Grid.instance.WorldPointFromNode(path[i+1]), Color.red, 1.0f);
            }

            pathFound = true; //set pathfound bool to true
        }
    }
}

//enum used to decide whgich heuristic method to use for the A star pathfinding algorithm
public enum HeuristicMethod
{
    Octile,
    Euclidean,
    Manhattan,
    Diagonal
}
