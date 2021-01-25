using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


//A script added onto MGTurnManager meant to add publicly accessible functions and variables.
public class MGAddOnUI : MonoBehaviour
{
    public CameraReference TalkBoxSci;
    public Canvas moveMenu;
    public Button move;
    public Button attack;
    public Button end;
    
    private UnityAction moveButtonEvent;
    private UnityAction attackButtonEvent;
    private UnityAction endButtonEvent;

    private MGMovementController currentTurn;

    // Start is called before the first frame update
    void Start()
    {
        //Setup move, attack, and end turn buttons.
        moveMenu = Camera.main.transform.Find("MoveMenu").GetComponent<Canvas>();
        move = moveMenu.transform.Find("Background").transform.Find("Move").GetComponent<Button>();
        attack = moveMenu.transform.Find("Background").transform.Find("Attack").GetComponent<Button>();
        end = moveMenu.transform.Find("Background").transform.Find("EndTurn").GetComponent<Button>();
        moveButtonEvent += MoveButton;
        attackButtonEvent += AttackButton;
        endButtonEvent += EndButton;
        move.onClick.AddListener(moveButtonEvent);
        attack.onClick.AddListener(attackButtonEvent);
        end.onClick.AddListener(endButtonEvent);
        TalkBoxSci = GetComponent<CameraReference>();
    }

    //Flashlight no longer follows mouse movement.
    public void StopFollowingTarget(GameObject target)
    {
        try
        {
            target.GetComponentInChildren<FlashlightMovement>().followMouse = false;
        }
        catch (System.Exception)
        {
        }
        
    }

    //Camera moves to target and flashlight starts following mouse movement.
    public void FollowTarget(GameObject target)
    {
        currentTurn = target.GetComponent<MGMovementController>();
        Camera.main.GetComponent<CameraController>().FollowTarget(target);
        try
        {
            if (target.GetComponentInChildren<CharacterPacket>().isPlayer)
            {
                Camera.main.GetComponent<CameraController>().SetControllable(true);
            }
            else
            {
                Camera.main.GetComponent<CameraController>().SetControllable(false);
            }
            currentTurn.GetComponentInChildren<Animator>().SetInteger("moveSpeed", 1);
        }
        catch (System.Exception) {}
    }

    public void MoveButton()
    {
        moveMenu.enabled = false;
        currentTurn.MGUnitBeginTurn();
    }

    public void AttackButton()
    {
        moveMenu.enabled = false;
        currentTurn.MGUnitBeginAttack();
    }

    public void EndButton()
    {
        moveMenu.enabled = false;
        MGTurnManager.MGEndTurn();
    }

    public void DelayEnemyTurn(MGMovementController unit) //Waits until enemy is on screen.
    {
        StartCoroutine(DelayAction.ExecuteWhenTrue(() => { return !Camera.main.GetComponent<CameraController>().isMoving; }, () => { unit.MGUnitBeginTurn(); }));
    }

    //Passes the current unit and targeted unit into the BattleSceneManager.
    public bool EnterBattle(Transform target, int distance)
    {
        StartCoroutine(DelayAction.ExecuteAfterTime(.5f, () => { currentTurn.RemoveSelectableTiles(); }));
        return Camera.main.GetComponentInChildren<BattleSceneManager>().CheckBattle(currentTurn.transform.Find("BattleData").gameObject, target.Find("BattleData").gameObject, distance);
    }

    public MGMovementController GetCurrentTurn()
    {
        return currentTurn;
    }

    public void PlayCredits()
    {
        StartCoroutine(DelayAction.ExecuteAfterTime(.5f, () =>
        {
            Camera.main.GetComponentInChildren<BattleSceneManager>().ScreenWipe.SetTrigger("Intro");
            StartCoroutine(DelayAction.ExecuteAfterTime(.5f, () =>
            {
                //This code runs when a team dies. It jumps to credits scene
                SceneManager.LoadScene(2);
            }));
        }));
    }
}
