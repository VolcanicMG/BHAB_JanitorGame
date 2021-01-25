/*
 * Scripted By: Wolfgang Sandtner
 * Date: 3/05/2020
 * Purpose: This script is applied to every tile that makes up the map. It holds the tiles conditions such as walkable or unwalkable.
 * It also holds functions related to the tile and it's neighbors.
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MGTile : MonoBehaviour
{
    //Variables:
    public bool MGWalkable = true; //This will determine if the tile can or can't be walked on, eg. walls or floors
    public bool MGCurrent = false; //True if tile is currently under the player
    public bool MGTarget = false; //True if the player clicked on tile and is moving towards it
    public bool MGSelectable = false; //True if it is in players range to be clicked on
    public bool MGAttackable = false; //True if it is in players range to be clicked on
    public bool MGOccupied = false;

    public List<MGTile> MGAdjacencyList = new List<MGTile>(); //List that holds adjacent tiles

    //Needed Variables for BFS(Breath First Search)
    public bool MGVisited = false; //Tests if BFS has already checked it
    public MGTile MGParent = null; //The starting position
    public int MGDistance = 0; //The allowed distance to search

    //Needed Variables for A*
    public float MGAstarF = 0; //G + H
    public float MGAstarG = 0; //Cost from parent to current tile
    public float MGAstarH = 0; //Cost from processed tile to destination

    public Material MGCurrentMaterial;
    public Material MGTargetMaterial;
    public Material MGSelectableMaterial;
    public Material MGDefaultMaterial;
    public Material MGUnwalkableMaterial;

    //Update:
    void Update()
    {
        if (MGCurrent) //Tile that player is standing on
        {
            GetComponent<MeshRenderer>().material = MGCurrentMaterial;
        }
        else if (MGTarget) //Tile that player is moving to
        {
            GetComponent<MeshRenderer>().material = MGTargetMaterial;

            if (!transform.Find("SelectableTile").gameObject.activeSelf)
            {
                transform.Find("SelectableTile").gameObject.SetActive(true);
            }
        }
        else if (MGSelectable) //Tile that the player can move to
        {
            GetComponent<MeshRenderer>().material = MGSelectableMaterial;

            if (!transform.Find("SelectableTile").gameObject.activeSelf)
            {
                transform.Find("SelectableTile").gameObject.SetActive(true);
            }

            if (!transform.Find("TileHighlight").gameObject.activeSelf)
            {
                if (MGTurnManager.GetAddOnUI().GetCurrentTurn().GetComponentInChildren<CharacterPacket>().isPlayer)
                {
                    transform.Find("TileHighlight").gameObject.SetActive(true);
                }
            }

            /*var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.transform.position = new Vector3(this.transform.position.x, this.transform.position.y, this.transform.position.z - 0.1f);
            cube.GetComponent<MeshRenderer>().material = mat;*/
        }
        else if (MGAttackable) //Tile that the player can move to
        {
            GetComponent<MeshRenderer>().material = MGSelectableMaterial;

            if (!transform.Find("AttackableTile").gameObject.activeSelf)
            {
                transform.Find("AttackableTile").gameObject.SetActive(true);
            }

            /*var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.transform.position = new Vector3(this.transform.position.x, this.transform.position.y, this.transform.position.z - 0.1f);
            cube.GetComponent<MeshRenderer>().material = mat;*/
        }
        else if (!MGWalkable)
        {
            GetComponent<MeshRenderer>().material = MGUnwalkableMaterial;
        }
        else //Any other tile
        {
            GetComponent<MeshRenderer>().material = MGDefaultMaterial;
        }
    }

    //Functions:
    public void MGReset() //Resets the tile to its default state
    {
        //Variables:
        MGAdjacencyList.Clear();

        MGCurrent = false;
        MGTarget = false;
        MGSelectable = false;
        MGAttackable = false;
        transform.Find("SelectableTile").gameObject.SetActive(false);
        transform.Find("AttackableTile").gameObject.SetActive(false);
        transform.Find("TileHighlight").gameObject.SetActive(false);

        MGVisited = false;
        MGParent = null;
        //MGDistance = 0;

        MGAstarF = 0;
        MGAstarG = 0;
        MGAstarH = 0;
    }

    public void MGFindNeighbors(float jumpheight, MGTile target) //Finds the tiles on all 4 sides of this tile
    {
        MGReset(); //Resets the tile

        MGCheckTile(Vector3.up, jumpheight, target); //Checks for tile above
        MGCheckTile(Vector3.right, jumpheight, target); //Checks for tile right
        MGCheckTile(-Vector3.up, jumpheight, target); //Checks for tile below
        MGCheckTile(-Vector3.right, jumpheight, target); //Checks for tile left
    }

    public void MGFindNeighborsLight(MGTile target) //Finds the tiles on all 4 sides of this tile while resetting as little info as possible
    {
        MGAdjacencyList.Clear();

        MGVisited = false;
        MGParent = null;

        MGAstarF = 0;
        MGAstarG = 0;
        MGAstarH = 0;

        MGCheckTile(Vector3.up, 2, target); //Checks for tile above
        MGCheckTile(Vector3.right, 2, target); //Checks for tile right
        MGCheckTile(-Vector3.up, 2, target); //Checks for tile below
        MGCheckTile(-Vector3.right, 2, target); //Checks for tile left
    }

    void OnMouseOver()
    {
        //Debug.Log("Hello");
    }

    void OnMouseExit()
    {
        
    }

    public void MGCheckTile(Vector3 direction, float jumpHeight, MGTile target) //Holds the algorithm that finds the tiles on the sides of this tile
    {
        Vector3 halfExtents = new Vector3(0.25f, 0.25f, 0.25f);//(-1 - jumpHeight) / 2.0f); //Provides a little wiggle room if tiles aren't exactly touching
        Collider[] colliders = Physics.OverlapBox(transform.position + direction, halfExtents); //Creates overlaping boxes between tiles to see if they are touching

        foreach (Collider item in colliders) //Runs through the found tiles
        {
            MGTile tile = item.GetComponent<MGTile>();
            if (tile != null && tile.MGWalkable) //Checks if the found tile exists and can be walked on
            {
                RaycastHit hit;

                if (!Physics.Raycast(tile.transform.position, Vector3.forward, out hit, 1) || (tile == target))
                {
                    MGAdjacencyList.Add(tile); //Adds the found tile to the list of adjacent tiles
                }
            }
        }
    }

    public int CalcDistance(MGTile target)
    {
        return Mathf.RoundToInt(Mathf.Abs(transform.position.x - target.transform.position.x) + Mathf.Abs(transform.position.y - target.transform.position.y));
    }

}
