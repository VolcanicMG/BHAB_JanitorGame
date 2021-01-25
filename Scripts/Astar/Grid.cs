using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{
    public Transform player; //This is the player
    public LayerMask unwalkableMask; //Represents the layer that all the obsticals are on
    public Vector2 gridWorldSize; //The X & Y size of the grid
    public float tileRadius; //Sets the size of each tile
    Tile[,] grid; //Tile represents the entire grid covering the map

    float tileDiameter; //Distance accross each tile
    int gridSizeX, gridSizeY; //Variables that hold the individual X & Y of gridWorldSize

    //Start method sets up the grid and tile size
    private void Start()
    {
        tileDiameter = tileRadius * 2;
        gridSizeX = Mathf.RoundToInt(gridWorldSize.x / tileDiameter);
        gridSizeY = Mathf.RoundToInt(gridWorldSize.y / tileDiameter);
        CreateGrid();
    }

    //Sets Max size for the Heap
    public int MaxSize
    {
        get
        {
            return gridSizeX * gridSizeY;
        }
    }

    //This function is called at the start to create the map grid that the play can move on
    void CreateGrid()
    {
        grid = new Tile[gridSizeX, gridSizeY];
        Vector3 worldBottomLeft = transform.position - Vector3.right * gridWorldSize.x / 2 - Vector3.up * gridWorldSize.y / 2;

        for(int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * tileDiameter + tileRadius) + Vector3.up * (y* tileDiameter + tileRadius);
                bool walkable = !(Physics.CheckSphere(worldPoint, tileRadius, unwalkableMask));
                grid[x, y] = new Tile(walkable, worldPoint, x, y);
            }
        }
    }

    //This Searches the tiles around the current tile
    public List<Tile> GetAdjacentTiles(Tile tile)
    {
        List<Tile> AdjacentTiles = new List<Tile>();

        for(int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0)
                    continue;
                
                int checkX = tile.gridX + x;
                int checkY = tile.gridY + y;

                if(checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY)
                {
                    AdjacentTiles.Add(grid[checkX, checkY]);
                }
            }
        }
        return AdjacentTiles;
    }

    //Sets the Players Position
    public Tile TileFromWorldPoint(Vector3 worldPosition)
    {
        float percentX = (worldPosition.x + gridWorldSize.x/2) / gridWorldSize.x;
        float percentY = (worldPosition.y + gridWorldSize.y/2) / gridWorldSize.y;
        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);

        int x = Mathf.RoundToInt((gridSizeX - 1) * percentX);
        int y = Mathf.RoundToInt((gridSizeY - 1.5f) * percentY); //If not working replace '- 1.5f' with '- 1'

        return grid[x,y];
    }

    public List<Tile> path; //Holds the Optimized Path

    //This Function Is Just to provided a visual of what is happening, this is not needed to run the A* Algorithm.
    void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, new Vector3(gridWorldSize.x, gridWorldSize.y, 1)); //Shows the outline of the grid

        if(grid != null)
        {
            Tile playerTile = TileFromWorldPoint(player.position);
            foreach(Tile tile in grid)
            {
                Gizmos.color = (tile.walkable) ? Color.white : Color.red; //Colors obsticals and open areas
                if (path != null)
                    if (path.Contains(tile))
                        Gizmos.color = Color.black;
                if(playerTile == tile)
                {
                    Gizmos.color = Color.cyan; //Colors the tile the player is on
                }
                Gizmos.DrawCube(tile.worldPos, Vector3.one * (tileDiameter - .1f));
            }
        }
    }

}
