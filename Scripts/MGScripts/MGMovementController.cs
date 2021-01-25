/*
 * Scripted By: Wolfgang Sandtner
 * Date: 3/05/2020
 * Purpose: This script is applied to nothing and called from the MGPlayerMove Script. It handles the character movement and path finding via BFS
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MGMovementController : MonoBehaviour
{
    //Variables:

    public bool MGUnitTurn = false;

    List<MGTile> MGSelectableTiles = new List<MGTile>(); //List of selectable Tile
    GameObject[] MGGameObjectTiles; //Holds all of the Game Tiles

    public Stack<MGTile> MGPath = new Stack<MGTile>(); //Creates a stack that holds the path to target
    public MGTile MGCurrentTile; //The current tile

    public bool MGMoving = false; //Determines if the player is moving
    public int MGMoveDistance = 5; //Distance that can be moved
    public float MGJumpHeight = 2; //Jump height
    public float MGMoveSpeed = 3; //MoveSpeed

    Vector3 MGVelocity = new Vector3(); //The velocity to move
    Vector3 MGHeading = new Vector3(); //The direction to move

    private Vector2 startingPos;
    private Vector2 targetPos;

    float MGHalfHeight = 0; //A float used for jumping in-between full tiles

    private Animator spriteAnimator;
    public SFXManager sfx;


    public MGTile MGActualTargetTile;

    //Functions:
    protected void MGInit() //Instatiates the Tile Grid Array
    {
        sfx = FindObjectOfType<SFXManager>();
        MGGameObjectTiles = GameObject.FindGameObjectsWithTag("MGTile");

        MGHalfHeight = GetComponent<Collider>().bounds.extents.z; //y

        MGTurnManager.MGAddUnit(this); //Adds this unit to the TurnManager

        MGGetCurrentTile();
        MGCurrentTile.MGOccupied = true;
        transform.position = (Vector2)MGCurrentTile.transform.position;

        spriteAnimator = GetComponentInChildren<Animator>();
    }

    public void MGGetCurrentTile() //Gets the current Tile
    {
        MGCurrentTile = MGGetTargetTile(gameObject);
        MGCurrentTile.MGCurrent = true;
    }

    public void MGRemoveCurrentTile()
    {
        if (MGCurrentTile != null) //Destroys current tile
        {
            MGCurrentTile.MGCurrent = false;
            MGCurrentTile = null;
        }
    }

    public MGTile MGGetTargetTile(GameObject target) //Gets the Target Tile
    {
        if (target.tag == "MGTile")
        {
            return target.GetComponent<MGTile>();
        }
        RaycastHit hit;
        MGTile tile = null;

        if (Physics.Raycast(target.transform.position, Vector3.forward, out hit, Mathf.Infinity))
        {
            return hit.collider.GetComponent<MGTile>(); //Finds the Target Tile via a Raycast
        }
        
        return tile;
    }

    public void MGComputeAdjacencyLists(float jumpHeight, MGTile target) //This finds optimal AdjacencyList
    {
        //MGGameObjectTiles = GameObject.FindGameObjectsWithTag("MGTile");

        foreach (GameObject tile in MGGameObjectTiles) //Finds the neighbors for tiles in Adjacency List
        {
            MGTile t = tile.GetComponent<MGTile>();
            t.MGFindNeighbors(jumpHeight, target);
            t.MGDistance = 0;
        }
    }

    public void MGComputeAdjacencyListsLight(MGTile target) //Created to fix graphical error. Finds adjacent tiles while resetting as little information as possible.
    {
        foreach (GameObject tile in MGGameObjectTiles) //Finds the neighbors for tiles in Adjacency List
        {
            MGTile t = tile.GetComponent<MGTile>();
            t.MGFindNeighborsLight(target);
            t.MGDistance = 0;
        }
    }

    public void FindDistance(MGTile target, int distance) //Find Distance Method
    {
        MGComputeAdjacencyListsLight(target);

        Queue<MGTile> process = new Queue<MGTile>(); //Queue holds the path of tiles

        process.Enqueue(target);
        target.MGVisited = true;
        //MGCurrentTile.MGParent = ?? //leave as null

        while (process.Count > 0) //Runs while there are tiles in the Queue
        {
            MGTile t = process.Dequeue(); //Pops the current tile out of the queue
            
            if (MGSelectableTiles.Contains(t)) //Runs while max distance has not been reached
            {
                foreach (MGTile tile in t.MGAdjacencyList) //Sets the values for each tile in the Adjacency list
                {
                    if (!tile.MGVisited)
                    {
                        tile.MGParent = t;
                        tile.MGVisited = true;
                        tile.MGDistance = 1 + t.MGDistance;
                        process.Enqueue(tile);
                    }
                }
            }
        }
    }

    public void MGFindSelectableTiles(bool resetTiles) //Also the BFS, this algorithm finds the path to target tile
    {
        if (resetTiles) //Whether or not tiles should be completely reset when adjacency search is done.
            MGComputeAdjacencyLists(MGJumpHeight, null);
        else
            MGComputeAdjacencyListsLight(null);

        MGGetCurrentTile();

        Queue<MGTile> process = new Queue<MGTile>(); //Queue holds the path of tiles
        
        process.Enqueue(MGCurrentTile);
        MGCurrentTile.MGVisited = true;
        //MGCurrentTile.MGParent = ?? //leave as null

        while (process.Count > 0) //Runs while there are tiles in the Queue
        {
            MGTile t = process.Dequeue(); //Pops the current tile out of the queue
            bool passable = true;

            if (t.MGOccupied) //Checks to see if tile is occupied by a hostile unit. If so, simply does not mark it as selectable. This is bound to be extremely buggy with enemy movement.
            {
                try
                {
                    Ray ray = new Ray();
                    ray.origin = new Vector3(t.transform.position.x, t.transform.position.y, -100); //Places ray at a negative z
                    ray.direction = new Vector3(0, 0, 100); //Sends ray in a positive z direction
                    if (Physics.Raycast(ray, out RaycastHit hit))
                    {
                        if (hit.collider.GetComponentInChildren<CharacterPacket>().isPlayer != GetComponentInChildren<CharacterPacket>().isPlayer) //Checks for player hits
                        {
                            passable = false;
                        }
                    }
                }
                catch (System.Exception) { print("Raycast Unidentified object."); }
            }

            
            MGSelectableTiles.Add(t); //Adds tile to selectable tiles List

            if (t.MGDistance <= MGMoveDistance) //Runs while max distance has not been reached
            {
                t.MGAttackable = true;

                if (passable)
                {
                    t.MGSelectable = true;
                    foreach (MGTile tile in t.MGAdjacencyList) //Sets the values for each tile in the Adjacency list
                    {
                        if (!tile.MGVisited)
                        {
                            tile.MGParent = t;
                            tile.MGVisited = true;
                            tile.MGDistance = 1 + t.MGDistance;
                            process.Enqueue(tile);
                        }
                    }
                }
            }
            else if (t.MGDistance > MGMoveDistance && t.MGDistance <= GetComponentInChildren<CharacterPacket>().range + MGMoveDistance) //Runs while max distance has not been reached
            {
                t.MGAttackable = true;
                foreach (MGTile tile in t.MGAdjacencyList) //Sets the values for each tile in the Adjacency list
                {
                    if (!tile.MGVisited)
                    {
                        tile.MGParent = t;
                        tile.MGVisited = true;
                        tile.MGDistance = 1 + t.MGDistance;
                        process.Enqueue(tile);
                    }
                }
            }
        }
    }

    public void MGMoveToTile(MGTile tile) //This function moves the character towards the target Tile
    {
        MGPath.Clear(); //Clears previous paths

        FindDistance(tile, MGMoveDistance);
        MGFindSelectableTiles(false);

        tile.MGTarget = true; //Sets tile as target tile
        MGMoving = true;

        MGTile next = tile;

        while(next != null) //Traverses the stack
        {
            MGPath.Push(next);
            next = next.MGParent;
        }

        if (!Camera.main.GetComponent<CameraController>().isControlled)
            Camera.main.GetComponent<CameraController>().FollowTarget(gameObject);

        spriteAnimator.SetInteger("moveSpeed", 2);
        MGPath.Pop();

        startingPos = transform.position;
        targetPos = MGPath.Peek().transform.position;

        MGCalculateHeading(targetPos);
        MGSetHorizontalVelocity();
        sfx.SFXPlayFootstep();

        //RemoveSelectableTiles();
    }

    public bool MoveToNearestPointInRange(MGTile tile, int range) //This function moves the character towards the target Tile
    {
        MGPath.Clear(); //Clears previous paths
        int minMovement = MGMoveDistance - range + 1;

        FindDistance(tile, minMovement);

        foreach (MGTile t in MGSelectableTiles)
        {
            if (t.MGDistance <= range && !t.MGOccupied && t.MGSelectable)
            {
                print ("Found attack point at "+t.gameObject.name+" "+ (tile.MGDistance - t.MGDistance) +" tiles away from target.");
                MGFindSelectableTiles(false);
                t.MGTarget = true; //Sets tile as target tile
                MGMoving = true;

                MGTile next = t;
                while (next != null) //Traverses the stack
                {
                    MGPath.Push(next);
                    next = next.MGParent;
                }

                break;
            }
        }

        if (MGPath.Count <= 0) return false;
        

        if (!Camera.main.GetComponent<CameraController>().isControlled)
            Camera.main.GetComponent<CameraController>().FollowTarget(gameObject);

        spriteAnimator.SetInteger("moveSpeed", 2);
        MGPath.Pop();

        startingPos = transform.position;
        targetPos = MGPath.Peek().transform.position;

        MGCalculateHeading(targetPos);
        MGSetHorizontalVelocity();
        sfx.SFXPlayFootstep();

        //RemoveSelectableTiles();
        return true;
    }

    public void MGMove() //Moves the character tile by tile
    {
        if(MGPath.Count > 0) //Has a tile to move towards
        {
            //MGTile t = MGPath.Peek();
            //Vector3 target = t.transform.position;

            ////Calculates the units position on top of the target tile
            //target.z = 0f;//-2.5f;//-= MGHalfHeight - t.GetComponent<Collider>().bounds.extents.z; //y += HH + y //Sets the character ontop of the floor, z-axis
            
            //if(Vector2.Distance(transform.position, targetPos) >= 0.05f && Vector2.Distance(transform.position, startingPos) <= 1.05f) //Determines if character has reached target
            //{

                //Jumping here
                //Not included as it is not needed

                //Movement here
                //MGCalculateHeading(target);
                //MGSetHorizontalVelocity();
                //print(MGVelocity);

                //transform.up = MGHeading; //This turns character towards the heading
                transform.position += MGVelocity * Time.deltaTime;
            //}
            //else //Target Reached
            //{
            if (Vector2.Distance(transform.position, targetPos) <= 0.05f || Vector2.Distance(transform.position, startingPos) >= 1.05f) //Determines if character has reached target
            {
                transform.position = targetPos;
                MGPath.Pop();

                if (MGPath.Count > 0) //Has a tile to move towards
                {
                    startingPos = transform.position;
                    targetPos = MGPath.Peek().transform.position;

                    MGCalculateHeading(targetPos);
                    MGSetHorizontalVelocity();
                    sfx.SFXPlayFootstep();
                }
            }
        }
        else //No longer has tiles to move towards
        {
            //ResetTileLocation();
            MGGetCurrentTile();
            MGCurrentTile.MGReset();
            RemoveSelectableTiles();

            MGMoving = false;
            spriteAnimator.SetInteger("moveSpeed", 1);

            //Checks to see if current unit is a player
            if (!gameObject.GetComponentInChildren<CharacterPacket>().isPlayer)
            {
                //If the current unit is not a player, runs AttackCheck
                if (AttackCheck())
                    //If AttackCheck finds a target, does NOT end turn, transfers unit into battle
                    return;
            }
            else
            {
                //If the current unit is a player, disables move button.
                MGTurnManager.GetAddOnUI().move.interactable = false;
                MGTurnManager.GetAddOnUI().moveMenu.enabled = true;
                
                return;
            }


            //-------------------------------------------------------------Combat or Item Pickup happens here after move has been completed

            //If no other conditions are met, ends turn.
            MGTurnManager.MGEndTurn(); //Ends turn
        }
    }

    public void RemoveSelectableTiles() //Removes all selectable tiles and resets their values
    {
        MGRemoveCurrentTile();

        foreach(MGTile tile in MGSelectableTiles) //Resets the Tiles' values
        {
            if (!tile.MGTarget)
            {
                tile.MGReset();
            }
        }

        MGSelectableTiles.Clear(); //Clears list of selectable tiles
    }

    void MGCalculateHeading(Vector3 target) //Sets the direction to Travel
    {
        MGHeading = target - transform.position;
        MGHeading.Normalize();
    }

    void MGSetHorizontalVelocity() //Starts traveling
    {
        MGVelocity = MGHeading * MGMoveSpeed;
        if (MGVelocity.x > 0.05f) spriteAnimator.SetBool("faceLeft", false);
        else if (MGVelocity.x < 0.05f) spriteAnimator.SetBool("faceLeft", true);
    }

    protected MGTile MGFindLowestF(List<MGTile> list)
    {
        MGTile lowest = list[0];

        foreach(MGTile t in list)
        {
            if(t.MGAstarF < lowest.MGAstarF)
            {
                lowest = t;
            }
        }

        list.Remove(lowest);

        return lowest;
    }

    protected MGTile MGFindEndTile(MGTile t) //Finds the actual target tile for A* (the tile next to player, not players tile)
    {
        Stack<MGTile> tempPath = new Stack<MGTile>();

        MGTile next = t; //Parent tile will be the tile closet to target (player) tile
        MGTile endTile = null;

        while (next!= null)
        {
            tempPath.Push(next);
            next = next.MGParent;
        }

        if(tempPath.Count <= MGMoveDistance)
        {
            endTile = t;
        }
        else
        {
            for (int i = 0; i <= MGMoveDistance; i++)
            {
                endTile = tempPath.Pop();
            }
        }

        while (endTile.MGOccupied)
        {
            endTile = endTile.MGParent;
        }

        print("Moving to " + endTile + ", " + endTile.transform.parent);

        return endTile;
    }

    protected bool MGFindPath(MGTile target) //Uses A* to find optimal path to player
    {
        MGComputeAdjacencyLists(MGJumpHeight, target);
        MGGetCurrentTile();

        List<MGTile> openList = new List<MGTile>(); //Tiles that need to be checked
        List<MGTile> closedList = new List<MGTile>(); //Tiles that have been checked

        openList.Add(MGCurrentTile);
        //MGCurrentTile.MGParent = ??
        MGCurrentTile.MGAstarH = Vector3.Distance(MGCurrentTile.transform.position, target.transform.position);
        MGCurrentTile.MGAstarF = MGCurrentTile.MGAstarH;

        while(openList.Count > 0)
        {
            MGTile t = MGFindLowestF(openList);
            
            closedList.Add(t);

            if(t == target) //Found the target and have the completed path
            {
                //Move next to target, not on top of it
                MGActualTargetTile = MGFindEndTile(t);
                if (MGActualTargetTile != null)
                {
                    MGMoveToTile(MGActualTargetTile);
                    return true;
                }
            }

            foreach (MGTile tile in t.MGAdjacencyList)
            {
                //BUGGY??
                if (t.MGOccupied)
                {
                    try
                    {
                        Ray ray = new Ray();
                        ray.origin = new Vector3(t.transform.position.x, t.transform.position.y, -100); //Places ray at a negative z
                        ray.direction = new Vector3(0, 0, 100); //Sends ray in a positive z direction
                        if (Physics.Raycast(ray, out RaycastHit hit))
                        {
                            if (hit.collider.GetComponentInChildren<CharacterPacket>().isPlayer != GetComponentInChildren<CharacterPacket>().isPlayer) //Checks for player hits
                            {
                                closedList.Add(t);
                                openList.Remove(t);
                                break;
                            }
                        }
                    }
                    catch (System.Exception) { print("Raycast Unidentified object at " + t + ", " + t.transform.parent.name); }
                }
                //BUGGY??

                if (closedList.Contains(tile)) //Do Nothing
                {
                    //Do Nothing, already processed
                }
                else if (openList.Contains(tile)) //See if the new way is faster then the old way
                {

                    float tempG = t.MGAstarG + Vector3.Distance(tile.transform.position, t.transform.position);

                    if (tempG < tile.MGAstarG)
                    {
                        tile.MGParent = t;

                        tile.MGAstarG = tempG;
                        tile.MGAstarF = tile.MGAstarG + tile.MGAstarH;
                    }
                }
                else //Process the new tile
                {
                    tile.MGParent = t;

                    tile.MGAstarG = t.MGAstarG + Vector3.Distance(tile.transform.position, t.transform.position);
                    tile.MGAstarH = Vector3.Distance(tile.transform.position, target.transform.position);
                    tile.MGAstarF = tile.MGAstarG + tile.MGAstarH;

                    openList.Add(tile);
                }
            }
        }

        //Todo - What to do if there is no path to the current tile?
        Debug.Log("Error - No Path Availble");
        return false;
    }
    

    public void MGUnitBeginTurn() //This holds everything the unit can do during its turn
    {
        MGUnitTurn = true;

        //Added to fix tile animation
        MGFindSelectableTiles(true);

        MGGetCurrentTile();
        MGCurrentTile.MGOccupied = false;
    }

    public void MGUnitEndTurn() //This is called to end the units turn
    {
        if (spriteAnimator.GetInteger("moveSpeed") != 0) spriteAnimator.SetInteger("moveSpeed", 0);
        RemoveSelectableTiles();
        MGUnitTurn = false;

        MGGetCurrentTile();
        MGCurrentTile.MGOccupied = true;

        MGTurnManager.GetAddOnUI().TalkBoxSci.TalkBox.enabled = false;
    }

    public void MGUnitBeginAttack() //This holds everything the unit can do during its turn
    {
        MGUnitTurn = true;

        //Added to fix tile animation
        MGFindAttackableTiles(GetComponentInChildren<CharacterPacket>().range);
    }

    public void MGFindAttackableTiles(int range) //Also the BFS, this algorithm finds the path to target tile
    {
        MGComputeAdjacencyLists(MGJumpHeight, null);
        MGGetCurrentTile();

        Queue<MGTile> process = new Queue<MGTile>(); //Queue holds the path of tiles

        process.Enqueue(MGCurrentTile);
        MGCurrentTile.MGVisited = true;
        //MGCurrentTile.MGParent = ?? //leave as null

        while (process.Count > 0) //Runs while there are tiles in the Queue
        {
            MGTile t = process.Dequeue(); //Pops the current tile out of the queue

            MGSelectableTiles.Add(t); //Adds tile to selectable tiles List
            t.MGAttackable = true;

            if (t.MGDistance < range) //Runs while max distance has not been reached
            {
                foreach (MGTile tile in t.MGAdjacencyList) //Sets the values for each tile in the Adjacency list
                {
                    if (!tile.MGVisited)
                    {
                        tile.MGParent = t;
                        tile.MGVisited = true;
                        tile.MGDistance = 1 + t.MGDistance;
                        process.Enqueue(tile);
                    }
                }
            }
        }
    }
    
    public bool AttackCheck() //Checks tiles in range to see if any player units exist
    {
        MGFindAttackableTiles(GetComponentInChildren<CharacterPacket>().range); //Finds tiles in range
        foreach (MGTile tile in MGSelectableTiles) //Runs through each tile
        {
            if (tile.MGVisited)
            {
                Ray ray = new Ray();
                ray.origin = new Vector3(tile.transform.position.x, tile.transform.position.y, -100); //Places ray at a negative z
                ray.direction = new Vector3(0, 0, 100); //Sends ray in a positive z direction
                if (Physics.Raycast(ray, out RaycastHit hit))
                {
                    if (hit.collider.tag == "TestPlayer") //Checks for player hits
                    {
                        RaycastHit[] hits = Physics.RaycastAll(ray);

                        foreach (RaycastHit x in hits)
                        {
                            if (x.collider.tag == "MGTile") //Tests if selected item is a tile
                            {
                                MGUnitTurn = false; //Ends turn in MGEnemyMove
                                if (MGTurnManager.GetAddOnUI().EnterBattle(hit.transform, x.collider.GetComponent<MGTile>().MGDistance - 1))
                                    return true;
                                else
                                    return false;
                            }
                        }
                    }
                }
            }
        }

        return false;
    }

    public List<GameObject> CheckAttackPoints()
    {
        GameObject[] targets = GameObject.FindGameObjectsWithTag("TestPlayer"); //Set tag to whatever the player tag is
        List<GameObject> attackPoints = new List<GameObject>();
        MGTile targetTile;

        MGGetCurrentTile();
        foreach (GameObject obj in targets)
        {
            targetTile = MGGetTargetTile(obj);
            //print(targetTile + ", " + targetTile.transform.parent);

            if (!MGSelectableTiles.Contains(targetTile) 
                || obj.GetComponentInChildren<CharacterPacket>().battleScene != null 
                && !obj.GetComponentInChildren<CharacterPacket>().battleScene.hasEmptySlot(transform.Find("BattleData").gameObject))
            {
                //print("Battle is full.");
            }
            else
            {
                if (targetTile.MGDistance <= MGMoveDistance + GetComponentInChildren<CharacterPacket>().range)
                {
                    foreach (MGTile tile in MGSelectableTiles)
                    {
                        if (targetTile.CalcDistance(tile) <= GetComponentInChildren<CharacterPacket>().range)
                        {
                            //print(targetTile.CalcDistance(tile));
                            if (!tile.MGOccupied && tile.MGDistance <= MGMoveDistance && tile.MGDistance != 0 && !attackPoints.Contains(tile.gameObject))
                            {
                                //print("Found attack point at " + tile + ", " + tile.transform.parent);
                                attackPoints.Add(tile.gameObject);
                            }
                        }
                    }
                }
            }
        }

        if (attackPoints.Count > 0)
        {
            return attackPoints;
        }
        //print("No attack points.");
        return null;
    }
}
