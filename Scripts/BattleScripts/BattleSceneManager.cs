using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.AddressableAssets;

//Creates, destroys, and otherwise manages BattleScenes
public class BattleSceneManager : MonoBehaviour
{

    private GameObject storage; //Addressable asset containing non-unique data needed for battle
    private BattleScene currentBattleScene; //Stores current battle scene
    public bool battleSceneActive = false; //Whether or not a battle is currently being shown
    public Animator ScreenWipe;
    private MusicManager music;

    private float tDelay = .5f;

    private void Start()
    {
        music = GetComponentInChildren<MusicManager>();
        ScreenWipe = transform.Find("ScreenWipe").GetComponentInChildren<Animator>();
        //ScreenWipe.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, Screen.width);

        //Loads all data that will be used across ALL battles.
        Addressables.LoadAssetAsync<GameObject>("BattleStorage").Completed += onLoadDone;
    }

    private void onLoadDone(UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle<GameObject> obj)
    {
        //This should finish before any battle can conceivably start.
        storage = obj.Result;
    }

    public bool CheckBattle(GameObject attacker, GameObject target, int distance) //Checks to see if the Target is already in battle. The Attacker can NEVER be in a battle
    {
        if (target.GetComponentInChildren<CharacterPacket>().inBattle) //If target is in battle, adds Attacker to battle and starts their turn.
        {
            if (target.GetComponentInChildren<CharacterPacket>().battleScene.hasEmptySlot(attacker))
            {
                currentBattleScene = target.GetComponentInChildren<CharacterPacket>().battleScene; //Sets currentBattleScene to target's battle
                currentBattleScene.addNewBattler(attacker, distance);
                StartCoroutine(DelayAction.ExecuteWhenTrue(() => { return attacker.GetComponent<CharacterPacket>().inBattle; },
                    () => { ShowBattleScene(attacker); }));
            }
            else
                return false;
        }
        else
            CreateNewBattle(attacker, target, distance); //If target is not in battle, creates a new battle
        return true;
    }

    public void CreateNewBattle(GameObject attacker, GameObject target, int distance)
    {
        GameObject newBattleScene = new GameObject(); //Creates a new battle as a child. Multiple battles can exist at the same time!

        newBattleScene.name = "BattleInstance"; //Bookkeeping. May become more specified later.
        newBattleScene.transform.SetParent(transform); //Sets BattleSceneManager as parent
        newBattleScene.transform.localPosition = new Vector3(0, 0, -1f); //Moves new battle to be centered on the camera

        //Creates and fills battle scene, adding universal objects (storage), and map dependent objects (BattleBackgrounds, etc.)
        newBattleScene.AddComponent<BattleScene>().Initialize(storage, transform.Find("BattleBackgrounds").gameObject);

        currentBattleScene = newBattleScene.GetComponent<BattleScene>(); //Sets currentBattleScene to new battle

        //After battle has been created, inserts the attacker and their target into the battle.
        currentBattleScene.CreateBattle(attacker, target, distance);

        StartCoroutine(DelayAction.ExecuteAfterTime(.3f, () => {
            music.Switch();
            ScreenWipe.SetTrigger("Intro"); //Plays battle transition animation
            StartCoroutine(DelayAction.ExecuteAfterTime(tDelay, () => {
                StartCoroutine(DelayAction.ExecuteWhenTrue(() => {
                    return attacker.GetComponent<CharacterPacket>().inBattle && target.GetComponent<CharacterPacket>().inBattle; }, () => {
                        MGTurnManager.GetAddOnUI().TalkBoxSci.TalkBox.enabled = false;
                        ScreenWipe.SetTrigger("Exit");
                        currentBattleScene.transform.localPosition = new Vector3(0, 0, 9f); //Sets battle to a visible z position
                        currentBattleScene.GetComponentInChildren<Canvas>().enabled = true; //Makes battle UI visible

                        currentBattleScene.StartTurn(attacker.GetComponentInChildren<CharacterPacket>(), 1);
                    }));
            }));
        }));
        battleSceneActive = true;
    }

    //Plays battle scene transition and starts current unit's turn. 
    public void ShowBattleScene(GameObject currentUnit) 
    {
        currentBattleScene = currentUnit.GetComponentInChildren<CharacterPacket>().battleScene; //Sets current battle scene to current unit's battle

        print("Showing "+ currentUnit.name+"'s battle.");

        if (!battleSceneActive) //If there is no active battle scene, plays battle transition. ShowBattleScene will NEVER run when the wrong battle scene is active.
        {
            battleSceneActive = true;

            StartCoroutine(DelayAction.ExecuteAfterTime(.3f, () => {
                music.Switch();
                ScreenWipe.SetTrigger("Intro"); //Plays battle transition animation
                StartCoroutine(DelayAction.ExecuteAfterTime(tDelay, () => {
                    ScreenWipe.SetTrigger("Exit");
                    MGTurnManager.GetAddOnUI().TalkBoxSci.TalkBox.enabled = false;
                    currentBattleScene.transform.localPosition = new Vector3(0, 0, 9f); //Sets battle to a visible z position
                    currentBattleScene.GetComponentInChildren<Canvas>().enabled = true; //Makes battle UI visible

                    currentBattleScene.StartTurn(currentUnit.GetComponentInChildren<CharacterPacket>(), 1);
                }));
            }));
        }
        else
        {
            currentBattleScene.StartTurn(currentUnit.GetComponentInChildren<CharacterPacket>()); //If there is an active battle scene, starts the next turn.
        }
    }

    public void ProcessTurnEnd() //Checks to see if next unit in turn order is part of the current battle. If not, hides battle.
    {
        if (MGTurnManager.CheckNextTurn().GetComponentInChildren<CharacterPacket>().battleScene == currentBattleScene)
        {
            print("Battle Continues");
            MGTurnManager.MGEndTurn();
        }
        else if (MGTurnManager.CheckNextTurn().GetComponentInChildren<CharacterPacket>().inBattle)
        {
            print("Transfering Battle");
            TransferBattleScene();
        }
        else
        {
            print("Hiding Battle");
            HideBattleScene();
        }
    }

    public void TransferBattleScene()
    {
        ScreenWipe.SetTrigger("Intro");
        StartCoroutine(DelayAction.ExecuteAfterTime(tDelay, () => {
            currentBattleScene.transform.localPosition = new Vector3(0, 0, -1);
            currentBattleScene.GetComponentInChildren<Canvas>().enabled = false;
            currentBattleScene = MGTurnManager.CheckNextTurn().GetComponentInChildren<CharacterPacket>().battleScene;
            ScreenWipe.SetTrigger("Exit");
            currentBattleScene.transform.localPosition = new Vector3(0, 0, 9f); //Sets battle to a visible z position
            currentBattleScene.GetComponentInChildren<Canvas>().enabled = true; //Makes battle UI visible
            MGTurnManager.MGEndTurn();
        }));
    }

    public void HideBattleScene() //Plays transition and hides battle window
    {
        if (currentBattleScene != null && battleSceneActive) //If a battle is active, hides battle.
        {
            battleSceneActive = false;
            
            ScreenWipe.SetTrigger("Intro"); //Plays battle transition animation
            StartCoroutine(DelayAction.ExecuteAfterTime(tDelay, () => {
                music.Switch();
                ScreenWipe.SetTrigger("Exit"); //Plays battle transition animation
                currentBattleScene.transform.localPosition = new Vector3(0, 0, -1); //Hides battle at negative z axis.
                currentBattleScene.GetComponentInChildren<Canvas>().enabled = false; //Hides battle UI
                StartCoroutine(DelayAction.ExecuteAfterTime(tDelay, () => {
                    MGTurnManager.MGEndTurn(); //Ends unit's turn after a short delay.
                }));
            }));
        }
    }

    public void EndBattle() //Runs when one team no longer has any units in battle. Hides battle, then destroys it.
    {
        
        ScreenWipe.SetTrigger("Intro");
        battleSceneActive = false;
        StartCoroutine(DelayAction.ExecuteAfterTime(tDelay, () => {
            music.Switch();
            ScreenWipe.SetTrigger("Exit");
            currentBattleScene.transform.localPosition = new Vector3(0, 0, -1);
            Destroy(currentBattleScene.gameObject);
            currentBattleScene = null;
            StartCoroutine(DelayAction.ExecuteAfterTime(tDelay, () => {
                MGTurnManager.MGEndTurn(); //Ends unit's turn after a short delay.
            }));
        }));
    }

    public void RunAway(bool endBattle) //Runs when one team no longer has any units in battle. Hides battle, then destroys it.
    {
        battleSceneActive = false;

        ScreenWipe.SetTrigger("Intro"); //Plays battle transition animation
        StartCoroutine(DelayAction.ExecuteAfterTime(tDelay, () => {
            music.Switch();
            ScreenWipe.SetTrigger("Exit"); //Plays battle transition animation
            currentBattleScene.transform.localPosition = new Vector3(0, 0, -1); //Hides battle at negative z axis.
            currentBattleScene.GetComponentInChildren<Canvas>().enabled = false; //Hides battle UI
            if (endBattle)
            {
                Destroy(currentBattleScene.gameObject);
                currentBattleScene = null;
            }

            //MGTurnManager.GetAddOnUI().attack.interactable = false;
            MGTurnManager.GetAddOnUI().moveMenu.enabled = true;
            MGTurnManager.GetAddOnUI().GetCurrentTurn().GetComponentInChildren<FlashlightMovement>().followMouse = true;
        }));
    }
}
