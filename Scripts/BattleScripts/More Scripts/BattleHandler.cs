using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleHandler : MonoBehaviour
{

    private Transform pfCharacterBattle;
    private Transform pfEnemyBattle;


    //public Sprite playerSpritesheet;
    //public Sprite enemySpritesheet;

    private CharacterBattle playerCharacterBattle;
    private CharacterBattle enemyCharacterBattle;
    private CharacterBattle activeCharacterBattle;
    private State state;

    private ArrayList playerHP;
    private ArrayList enemyHP;

    private int signIndex;
    public Transform pfHealthBar;

    private enum State
    {
        WaitingForPlayer,
        Busy
    }

    public void setBattlers(Transform p, Transform e)
    {
        pfCharacterBattle = p;
        pfEnemyBattle = e;
    }

    private static BattleHandler instance;

    public static BattleHandler GetInstance()
    {
        return instance;
    }
    
    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        playerCharacterBattle = SpawnCharacter(true);
        enemyCharacterBattle = SpawnCharacter(false);

        signIndex = 0;

        SetActiveCharacterBattle(playerCharacterBattle);
        state = State.WaitingForPlayer;
    }


    private void Update()
    {
        
        if (state == State.WaitingForPlayer) {
            Debug.Log("Made it.");
            if (Input.GetKeyDown(KeyCode.Space))
            {
                
                state = State.Busy;
                playerCharacterBattle.Attack(enemyCharacterBattle, () =>
                {
                    ChooseNextActiveCharacter();
                });
            }
        }
    }

    private CharacterBattle SpawnCharacter(bool isPlayerTeam)
    {
        Vector3 position;
        if (isPlayerTeam)
        {
            position = new Vector3(-2, 0);
            Transform characterTransform = Instantiate(pfCharacterBattle, position, Quaternion.identity);

            position = new Vector3(-2, -1.5f);
            Instantiate(pfHealthBar, position, Quaternion.identity);
            


            CharacterBattle characterBattle = characterTransform.GetComponent<CharacterBattle>();
            characterBattle.Setup(isPlayerTeam);

            signIndex++;

            return characterBattle;
        }
        else
        {
            position = new Vector3(2, 0);
            Transform characterTransform = Instantiate(pfEnemyBattle, position, Quaternion.identity);

            position = new Vector3(2, -1.5f);
            Instantiate(pfHealthBar, position, Quaternion.identity);

            CharacterBattle characterBattle = characterTransform.GetComponent<CharacterBattle>();
            characterBattle.Setup(isPlayerTeam);

            signIndex++;

            return characterBattle;
        }

        
    }

    private void SetActiveCharacterBattle(CharacterBattle characterBattle)
    {
        if (activeCharacterBattle != null)
        {
            activeCharacterBattle.HideSelectionCircle();
        }
        activeCharacterBattle = characterBattle;
        activeCharacterBattle.ShowSelectionCircle();
    }

    private void ChooseNextActiveCharacter()
    {
        if (testBattleOver())
        {
            return;
        }
        if (activeCharacterBattle == playerCharacterBattle)
        {
            SetActiveCharacterBattle(enemyCharacterBattle);
            state = State.Busy;

            enemyCharacterBattle.Attack(playerCharacterBattle, () =>
            {
                ChooseNextActiveCharacter();
            });
        }
        else
        {
            SetActiveCharacterBattle(playerCharacterBattle);
            state = State.WaitingForPlayer;
        }
    }

    private bool testBattleOver()
    {
        if (playerCharacterBattle.isDead())
        {
            return true;
        }
        if (enemyCharacterBattle.isDead())
        {
            return true;
        }
        return false;
    }
}
