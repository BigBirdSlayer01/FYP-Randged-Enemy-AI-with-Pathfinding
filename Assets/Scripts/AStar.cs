using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class AStar : MonoBehaviour
{
    Grid g; //will store the grid reference
    public List<Node> p; //list to store the path
    List<Node> path = new List<Node>();
    public GameObject target; //target object (possible to not be used)
    [HideInInspector] public GameObject searcher; //object that will follow the path (the enemy object the script is attached to)


    [HideInInspector] public bool pathFound; //bool that will return true when path found 
    [HideInInspector] public bool follow; //bool set to false when want to stop following the path
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

    public void FindPath(Vector3 start, Vector3 target) //finds the path to the target
    {
        targetIndex = 0; //sets the target index to 0 to initalise it

        Node startNode = g.NodeFromWorldPoint(start); //gets the Node from the start vector3 position
        Node targetNode = g.NodeFromWorldPoint(target); //gets the target node from the target Vector3 position

        Heap<Node> SetOpen = new Heap<Node>(Grid.instance.Max); //Queue for the open set using a Heap
        HashSet<Node> SetClosed = new HashSet<Node>(); // set for the closed set using heaps
        SetOpen.addToHeap(startNode); //adds the start node to the open set

        while(SetOpen.CountCheck > 0)
        {
            Node curr = SetOpen.RemoveFirst(); //gets the node with the lowest cost from the open set stores in curr variable

            SetClosed.Add(curr); //adds the current node to the closed set

            if(curr == targetNode) // if the current node is the target
            {
                TracePath(startNode, targetNode); //call the trace path method
                return; //exit
            }

            foreach(Node n in g.Neighbours(curr)) //checks each neighbouring node 
            {
                if (!n.Walkable || SetClosed.Contains(n))
                {
                    continue; //skip any nodes that arent walkable or if they are in the closed set
                }
                int newCost = curr.Cost + GetDist(curr, n); //calculates the cost reach the neighbour

                if (newCost < n.Cost || !SetOpen.ContainsItem(n)) //if the new cost is lower
                {
                    // Update neighbor's cost, heuristic, and parent
                    n.g = newCost;
                    n.h = GetDist(n, targetNode);
                    n.parent = curr;

                    if (!SetOpen.ContainsItem(n)) //if the open set does not contain the node
                    {
                        SetOpen.addToHeap(n); //add to open set 
                    }
                }
            }
        }
    }

    int GetDist(Node a, Node b) //will check how many nodes away the node is away from the target Position
    {
        int DistanceX = Mathf.Abs(a.X - b.X);
        int DistanceY = Mathf.Abs(a.Y - b.Y);

        if(DistanceX < DistanceY)
        {
            return 14 * DistanceY + 10 * (DistanceX - DistanceY);
        }
        else
        {
            return 14 * DistanceX + 10 * (DistanceY - DistanceX);
        }
    }

    void TracePath(Node start, Node Finish) //reverses the path to the correct order and stores in the path
    {
        path.Clear(); // Empty the path list

        Node curr = Finish; // current node set to last node

        while(curr != start) //while the current node is not the start node
        {
            path.Add(curr); //adds each node to the path
            curr = curr.parent; // moves to the parent
        }

        path.Reverse(); // reverses path to make it from start to finish

        pathFound = true; //set pathfound bool to true

        //g.path = path; //sets the grid path to the path (only for debugging purposes)
    }


    public IEnumerator followingPath() //coroutine to follow the path
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
            searcher.transform.position = Vector3.MoveTowards(searcher.transform.position, currentPoint, Time.deltaTime * 10.0f); // Move towards the current target node
            yield return null; 
        }
    }
}
