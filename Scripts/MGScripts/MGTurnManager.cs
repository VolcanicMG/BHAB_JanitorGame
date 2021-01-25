/*
 * Scripted By: Wolfgang Sandtner
 * Date: 3/05/2020
 * Purpose: This script is applied to the map grid or any object and handles the turn management between player and enemy //MovemetGrid is always at -64.5 on the z-axis
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MGTurnManager : MonoBehaviour
{
    //Variables:
    static Dictionary<string, List<MGMovementController>> MGUnits = new Dictionary<string, List<MGMovementController>>(); //Holds all player/enemy units
    static Queue<string> MGTurnKey = new Queue<string>(); //Holds whose turn it is
    static Queue<MGMovementController> MGTurnTeam = new Queue<MGMovementController>(); //Determines which team (player or dino)'s turn

    //AddOn
    static MGAddOnUI ui;

    //Start:
    void Awake()
    {
        ui = gameObject.AddComponent<MGAddOnUI>();
    }

    //Update:
    void Update()
    {
        if (MGTurnTeam.Count == 0)
        {
            InitTeamTurnQueue();
        }
    }

    //Functions:
    static void InitTeamTurnQueue() //Creates the team list for turns
    {
        List<MGMovementController> teamList = MGUnits[MGTurnKey.Peek()];

        foreach (MGMovementController unit in teamList)
        {
            MGTurnTeam.Enqueue(unit);
        }

        MGStartTurn();
    }

    public static void MGStartTurn() //Starts a player or enemies turn
    {
        if (MGTurnTeam.Count > 0)
        {
            ui.FollowTarget(MGTurnTeam.Peek().gameObject);

            if (MGTurnTeam.Peek().GetComponentInChildren<CharacterPacket>().inBattle)
            {
                print(MGTurnTeam.Peek().gameObject.name+" Is in battle");
                Camera.main.GetComponentInChildren<BattleSceneManager>().ShowBattleScene(MGTurnTeam.Peek().gameObject);
            }
            else if (MGTurnTeam.Peek().gameObject.CompareTag("Dino"))
            {
                 ui.DelayEnemyTurn(MGTurnTeam.Peek()); //Makes sure that a dino is on screen before it starts moving.
            }
            else
            {
                ui.move.interactable = true;
                ui.moveMenu.enabled = true;
                MGTurnTeam.Peek().MGUnitTurn = true;
                MGTurnTeam.Peek().GetComponentInChildren<FlashlightMovement>().followMouse = true;
            }
        }
    }

    public static void MGEndTurn() //Ends a player or enemies turn
    {
        MGMovementController unit = MGTurnTeam.Dequeue();
        ui.StopFollowingTarget(unit.gameObject);
        unit.MGUnitEndTurn();

        if (MGTurnTeam.Count > 0) //Calls next turn is availible
        {
            MGStartTurn();
        }
        else //Called if all turns for this cycle are completed
        {
            //print("Switching team");
            string team = MGTurnKey.Dequeue(); //Moves current team to end of Queue
            MGTurnKey.Enqueue(team);
            InitTeamTurnQueue();
        }
        if (GameObject.FindGameObjectsWithTag("Dino").Length <= 0 || GameObject.FindGameObjectsWithTag("TestPlayer").Length <= 0)
        {
            ui.PlayCredits();
        }
    }

    public static void MGAddUnit(MGMovementController unit) //Adds units to the Dictionary
    {
        List<MGMovementController> list;

        if (!MGUnits.ContainsKey(unit.tag))
        {
            list = new List<MGMovementController>();
            MGUnits[unit.tag] = list;

            if (!MGTurnKey.Contains(unit.tag))
            {
                MGTurnKey.Enqueue(unit.tag);
            }
        }
        else
        {
            list = MGUnits[unit.tag];
        }

        list.Add(unit);
    }

    public static void MGRemoveUnit(MGMovementController unit) //Remove units from the Dictionary
    {
        if (MGUnits.ContainsKey(unit.tag)) //Checks to see if any units matching ours exist (They should)
        {
            MGUnits[unit.tag].Remove(unit); //If so, removes unit.

            if (MGUnits[unit.tag].Count <= 0) //Checks to see if any other units on that team exist. If not, removes team.
            {
                //This code removes the team but allows gameplay to continue
                Queue<string> teams = new Queue<string>(); //Create a new Queue to temporarily store still existing teams
                int queueLength = MGTurnKey.Count; //Store .Count beforehand as it will change each time we dequeue a team
                for (int x = 0; x < queueLength; x++)
                {
                    if (MGTurnKey.Peek() == unit.tag) //If we find the team we are looking for, we dequeue it and stop searching.
                    {
                        print("Removed " + unit.tag);
                        MGTurnKey.Dequeue();
                        break;
                    }
                    else //If not, we store the current team in teams and look at the next place in the queue
                    {
                        print("Stored " + MGTurnKey.Peek());
                        teams.Enqueue(MGTurnKey.Dequeue());
                    }
                }

                queueLength = teams.Count;
                for (int x = 0; x < queueLength; x++) //After removing said team, we add the others back to MGTurnKey
                {
                    MGTurnKey.Enqueue(teams.Dequeue());
                }

                MGUnits.Remove(unit.tag); //Finally, we remove the tag entirely from MGUnits
            }
        }

        Destroy(unit.gameObject); //Destroys the unit utterly

        //if (GameObject.FindGameObjectsWithTag("Dino").Length <= 0)
        //{
        //    ui.PlayCredits();
        //}
    }

    public static GameObject CheckNextTurn()
    {
        if (MGTurnTeam.Count > 1) //Calls next turn is availible
        {
            print("Checking next in team.");
            return MGTurnTeam.ToArray()[1].gameObject;
        }
        else //Called if all turns for this cycle are completed
        {
            print("Checking other team.");
            return MGUnits[MGTurnKey.ToArray()[1]].ToArray()[0].gameObject;
        }
    }

    public static MGAddOnUI GetAddOnUI()
    {
        return ui;
    }

    //A function is needed that will remove a team member from a team when they die
    //Another function is needed to remove an entire team when all team members are killed

}
