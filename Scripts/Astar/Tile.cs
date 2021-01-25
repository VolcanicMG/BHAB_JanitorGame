using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : IHeapItem<Tile>
{
    public bool walkable;
    public Vector3 worldPos;
    public int gridX;
    public int gridY;

    public int gCost;
    public int hCost;
    public Tile parent;
    int heapIndex;

    //Tile represents each individual tile on the grid
    public Tile(bool _walkable, Vector3 _worldPos, int _gridX, int _gridY)
    {
        walkable = _walkable;
        worldPos = _worldPos;
        gridX = _gridX;
        gridY = _gridY;
    }

    public int fCost //Gets fCost
    {
        get
        {
            return gCost + hCost;
        }
    }

    public int HeapIndex //Updates Heap
    {
        get
        {
            return heapIndex;
        }
        set
        {
            heapIndex = value;
        }
    }

    public int CompareTo(Tile tileToCompare) //Tells Heap to Optimize
    {
        int compare = fCost.CompareTo(tileToCompare.fCost);
        if (compare == 0)
        {
            compare = hCost.CompareTo(tileToCompare.hCost);
        }
        return -compare;
    }
}
