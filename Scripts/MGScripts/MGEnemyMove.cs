/*
 * Scripted By: Wolfgang Sandtner
 * Date: 3/05/2020
 * Purpose: This script is applied to the enemy, it's purpose is to use A* to located the player and then move towards the player if it is the enemies turn
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MGEnemyMove : MGMovementController
{
    //Variables:
    GameObject MGEnemyTarget;
    List<GameObject> attackables;
    

    //Start:
    void Start()
    {
        MGInit();
        sfx.SFXPlaySE(4);
        attackables = new List<GameObject>();
    }

    //Update:
    void Update()
    {
        if (!MGUnitTurn) //Returns if its not this units turn
        {
            return;
        }

        if (!MGMoving) //Checks if enemy can move
        {
            MGFindSelectableTiles(true);
            MGFindNearestTarget();
            MGFindSelectableTiles(true); //Finds tiles that can be moved to
            //MGMoveToTile(MGActualTargetTile);
        }
        else
        {
            MGMove();
        }

        
    }

    //Functions:
    void MGCalculatePath() //Sets starting tile and begins to find path to target
    {
        MGTile targetTile = MGGetTargetTile(MGEnemyTarget);

        //int distance = MGMoveDistance;

        //if (attackables.Contains(MGEnemyTarget))
        //{
        //    MGTile tempTile = targetTile;

        //    attackables.Remove(MGEnemyTarget);

        //    foreach (MGTile tile in targetTile.MGAdjacencyList)
        //    {
        //        if (!tile.MGOccupied && tile.MGDistance <= distance)
        //        {
        //            distance = tile.MGDistance;
        //            tempTile = tile;
        //        }
        //    }
        //    if (tempTile != null)
        //    {
        //        MGFindPath(tempTile);
        //    }
        //}
        //else
        MGFindPath(targetTile);
    }

    void MGFindNearestTarget() //This finds the closet GameObject with "Player" tag based on distance - Line Of Sight algorithm will go here
    {
        List<GameObject> targets = new List<GameObject>();
        targets.AddRange(GameObject.FindGameObjectsWithTag("TestPlayer")); //Set tag to whatever the player tag is

        

        GameObject nearest = null;
        float distance;

        bool pathFound = false;

        List<GameObject> attackPoints = CheckAttackPoints();
        if (attackPoints != null)
        {
            //targets = new GameObject[attackPoints.Count];
            //attackPoints.CopyTo(targets);
            targets = attackPoints;
        }

        while (!pathFound)
        {
            nearest = null;
            distance = Mathf.Infinity;
            foreach (GameObject obj in targets)
            {
                float d = Vector3.Distance(transform.position, obj.transform.position);

                if (d < distance)
                {
                    distance = d;
                    nearest = obj;
                }
            }

            MGGetCurrentTile();

            //MGEnemyTarget = nearest;
            //MGCalculatePath();
            pathFound = MGFindPath(MGGetTargetTile(nearest));
            if (!pathFound)
            {
                targets.Remove(nearest);
            }
        }
    }
}
