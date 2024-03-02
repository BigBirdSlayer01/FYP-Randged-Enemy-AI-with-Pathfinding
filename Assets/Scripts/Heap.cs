using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Heap<Node> where Node : IHeapItem<Node> // Heap class that will be used to optimize the A* pathfinding algorithm
{
    Node[] items; //array to store items in the heap
    int Counter; //counter keeps track of number of items in the heap

    public Heap(int max) //Heap class constructor
    {
        items = new Node[max]; //initialises the array with specified maximum size
    }

    public void addToHeap(Node item) //adds item to the heap
    {
        item.HeapIndex = Counter; //sets the heap index of the item
        items[Counter] = item; // adds item to the array
        Sort(item); //calls the sort method
        Counter++; //increments the counter
    }

    public Node RemoveFirst() // Remove and return the first (highest priority) item from the heap
    {
        Node first = items[0]; // Get the first item
        Counter--; //decrements the counter
        items[0] = items[Counter]; // Move the last item up
        items[0].HeapIndex = 0; // Update the heap index
        ReverseSort(items[0]); //reverse sorts the moved item
        return first; // Return the first item
    }

    public bool ContainsItem(Node item) // Check if the heap contains a specific item
    {
        return Equals(items[item.HeapIndex], item);
    }

    public int CountCheck //get the current count of items in the heap
    { 
        get 
        { 
            return Counter; 
        } 
    }

    public void ItemUpdate(Node item) // Update the position of an item in the heap
    {
        Sort(item);
    }

    private void ReverseSort(Node item)
    {
        while(true)
        {
            // formula find left child of parent = n*2+1
            int childLeft = item.HeapIndex * 2 + 1; //calculates the left child
            // formula find right child of parent = n*2+2
            int childRight = item.HeapIndex * 2 + 2; //calculates the right child
            int swapItem = 0;

            if(childLeft < Counter)
            {
                swapItem = childLeft;

                if(childRight < Counter)
                {
                    if (items[childLeft].CompareTo(items[childRight]) < 0)  // Determine the index of the child with higher priority
                    {
                        swapItem = childRight; //stores the higher priority child
                    }
                }

                if (item.CompareTo(items[swapItem]) < 0) // Swap the item with the higher-priority child if necessary
                {
                    swap(item, items[swapItem]);
                }
                else
                {
                    return;
                }
            }
            else
            {
                return;
            }
        }
    }

    private void Sort(Node item)
    {
        //find parent formula (n-1)/2
        int parent = (item.HeapIndex - 1) / 2;
        while(true)
        {
            Node itemParent = items[parent]; // Get the parent item
            //if item has higher priority than parent
            if (item.CompareTo(itemParent) > 0)
            {
                swap(item, itemParent); // Swap the item with its parent
            }
            else
            {
                break;
            } 
            parent = (item.HeapIndex - 1) / 2; // Update the parent index
        }    
    }

    void swap(Node x, Node y) // used to swap two items in the heap
    {
        items[x.HeapIndex] = y;
        items[y.HeapIndex] = x;
        int tempVal = x.HeapIndex;
        x.HeapIndex = y.HeapIndex;
        y.HeapIndex = tempVal;
    }

}

public interface IHeapItem<Node> : IComparable<Node>
{
    int HeapIndex
    {
        get;
        set;
    }
}
