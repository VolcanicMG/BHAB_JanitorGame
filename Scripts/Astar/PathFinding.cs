using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFinding : MonoBehaviour
{
    public Transform Player; //Sets the Player
    public Transform Destination; //Sets where the player is going - This is what the clicked-base movement needs to set

    Grid grid; //Sets the grid

    //Gets the grid
    void Awake()
    {
        grid = GetComponent<Grid>();
    }

    //Constantly checks for optimal path
    void Update()
    {
        FindPath(Player.position, Destination.position);
    }

    //Finds the Optimal Path
    void FindPath(Vector3 startPos, Vector3 targetPos)
    {
        Tile startTile = grid.TileFromWorldPoint(startPos);
        Tile targetTile = grid.TileFromWorldPoint(targetPos);

        Heap<Tile> openSet = new Heap<Tile>(grid.MaxSize);
        HashSet<Tile> closedSet = new HashSet<Tile>();
        openSet.Add(startTile);

        while(openSet.Count > 0) //This will run as long as the path has not been found and tiles are still unexplored
        {
            Tile currentTile = openSet.RemoveFirst();
            closedSet.Add(currentTile);

            if(currentTile == targetTile) //If Path is Found
            {
                RetracePath(startTile, targetTile);
                return;
            }

            foreach(Tile adjacent in grid.GetAdjacentTiles(currentTile)) //Runs till Path is found
            {
                if(!adjacent.walkable || closedSet.Contains(adjacent))
                {
                    continue;
                }

                int newMovementCost = currentTile.gCost + GetDistance(currentTile, adjacent);
                if(newMovementCost < adjacent.gCost || !openSet.Contains(adjacent))
                {
                    adjacent.gCost = newMovementCost;
                    adjacent.hCost = GetDistance(adjacent, targetTile);
                    adjacent.parent = currentTile;

                    if (!openSet.Contains(adjacent))
                        openSet.Add(adjacent);
                }
            }
        }
    }

    //This Stores the Optimal Path
    void RetracePath(Tile startTile, Tile endTile)
    {
        List<Tile> path = new List<Tile>();
        Tile currentTile = endTile;

        while(currentTile != startTile)
        {
            path.Add(currentTile);
            currentTile = currentTile.parent;
        }
        path.Reverse();

        grid.path = path;
    }

    //This is the Math Algorithm for deciding which path is shortest
    int GetDistance(Tile tileA, Tile tileB)
    {
        int dstX = Mathf.Abs(tileA.gridX - tileB.gridX);
        int dstY = Mathf.Abs(tileA.gridY - tileB.gridY);

        if (dstX > dstY)
            return 14*dstY + 10* (dstX-dstY); //Note that the 14 represents taking an angled path
        return 14* dstX + 10 * (dstY - dstX);
    }
}
