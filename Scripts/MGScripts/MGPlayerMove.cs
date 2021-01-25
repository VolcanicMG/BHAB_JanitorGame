/*
 * Scripted By: Wolfgang Sandtner
 * Date: 3/05/2020
 * Purpose: This script is applied to the player, it's purpose is to check for mouse clicks and determine if the player can move or not
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MGPlayerMove : MGMovementController
{
    //Variables:
    private bool disableMouse = false;

    //Start:
    void Start()
    {
        MGInit();
    }

    //Update:
    void Update()
    {
        if (!MGUnitTurn) //Returns if its not this units turn
        {
            return;
        }

        if (!MGMoving) //Checks if player can move
        {
            //Moved to MGMovementController to fix tile animation.
            //MGFindSelectableTiles(); //Finds tiles that can be moved to
            CheckMouse(); //Checks for mouse click
        }
        else
        {
            MGMove();
        }
    }

    //Functions:
    void CheckMouse() //This function responds to mouse clicked, checks if the player has selected where to move
    {
        if (Input.GetMouseButtonUp(0))
        {
            Ray ray = new Ray();

            Vector2 mousePos = new Vector2();
            mousePos.x = Camera.main.pixelWidth - Input.mousePosition.x;
            mousePos.y = Camera.main.pixelHeight - Input.mousePosition.y;

            ray.origin = Camera.main.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, Camera.main.transform.position.z - Camera.main.nearClipPlane * 3));
            ray.direction = new Vector3(0, 0, 1);
            
            //print(ray);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.tag == "MGTile") //Tests if selected item is a tile
                {
                    MGTile t = hit.collider.GetComponent<MGTile>();

                    if (t.MGSelectable) //Tests if tile is able to be moved to
                    {
                        MGMoveToTile(t); //Moves the player to the selected tile
                    }
                }
                else if (hit.collider.tag == "Dino")
                {
                    RaycastHit[] hits = Physics.RaycastAll(ray);

                    foreach (RaycastHit x in hits)
                    {
                        if (x.collider.tag == "MGTile") //Tests if selected item is a tile
                        {
                            MGTile t = x.collider.GetComponent<MGTile>();

                            if (t.MGAttackable) //Tests if tile is able to be moved to
                            {
                                t.MGAttackable = false;
                                if (t.MGDistance > GetComponentInChildren<CharacterPacket>().range)
                                {
                                    if (MoveToNearestPointInRange(t, GetComponentInChildren<CharacterPacket>().range))
                                    { //Moves the player to the selected tile
                                        StartCoroutine(DelayAction.ExecuteWhenTrue(() => { return !MGMoving; }, () =>
                                        {
                                            GetComponentInChildren<FlashlightMovement>().followMouse = false;
                                            MGTurnManager.GetAddOnUI().EnterBattle(hit.transform, GetComponentInChildren<CharacterPacket>().range - 1);
                                        }));
                                    }
                                }
                                else
                                {
                                    GetComponentInChildren<FlashlightMovement>().followMouse = false;
                                    MGTurnManager.GetAddOnUI().EnterBattle(hit.transform, t.MGDistance - 1);
                                }
                            }
                            break;
                        }
                    }
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.LeftControl) || Input.GetKeyDown(KeyCode.RightControl) || Input.GetKeyDown("f"))
        {
            if (GetComponentInChildren<FlashlightMovement>().followMouse)
                GetComponentInChildren<FlashlightMovement>().followMouse = false;
            else
                GetComponentInChildren<FlashlightMovement>().followMouse = true;
        }

        if (Input.GetMouseButtonUp(1))
        {
            if (!MGTurnManager.GetAddOnUI().moveMenu.enabled)
            {
                RemoveSelectableTiles();
                MGTurnManager.GetAddOnUI().moveMenu.enabled = true;
            }
        }
    }

    private void OnMouseUp()
    {
            if (MGUnitTurn && MGTurnManager.GetAddOnUI().move.interactable)
                MGTurnManager.GetAddOnUI().MoveButton();
    }
}
