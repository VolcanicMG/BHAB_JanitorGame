using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using static CharacterList;

public class CharacterPacket : MonoBehaviour
{
    //A script carried by EVERY unit that stores ALL data relevant to battle (And possibly beyond.)

    private List<CharacterSelectObject> Picked;

    //Stat variables
    [Header("Stats - They get overwritten if they are a player")]
    public string title;
    public float mhp;
    public float hp;
    public float atk;
    public float def;
    public int range;
    public Sprite UISprite;
    public int charDesignation;

    [SerializeField]
    private Image CharacterImage;

    //Meta BattleScene variables
    [Header("Identifiability and settings for the player")]
    public int[] equippedSkillIDs; //The int ID of equipped skills
    public string battlePrefabAddress; //The address of the unit's BattleData
    public string[] spriteVariations; //String corresponding to sprite variations. Corresponds to GameObjects under variations tab in BattlePrefabs
    public BattleScene battleScene; //BattleScene CharacterPacket is currently part of. Null if not in battle.
    public int slotID; //ID of the slot CharacterPacket is currently in.

    [Header("bools - Don't touch unless its a player")]
    public bool isPlayer; //True if player
    public bool inBattle; //True if inBattle

    [Header("What player? 0-3")]
    public byte player;

    private enum Players : byte
    {//Player 1 starts at 0
        Player1,
        Player2,
        Player3,
        Player4
    }

    public void Start() //pass in all the info from the character selection screen
    {
        Picked = CharacterList.PickedCharacters;

        Players SelectedPlayer = (Players)player;

        switch (SelectedPlayer)
        {
            case Players.Player1:
                title = Picked[0].CharacterName;
                mhp = Picked[0].Health;
                def = Picked[0].Defense;
                atk = Picked[0].Attack;
                UISprite = Picked[0].Splash;
                range = Picked[0].range;
                CharacterImage.sprite = Picked[0].CharacterPortrait;
                break;
            case Players.Player2:
                title = Picked[1].CharacterName;
                mhp = Picked[1].Health;
                def = Picked[1].Defense;
                atk = Picked[1].Attack;
                UISprite = Picked[1].Splash;
                range = Picked[1].range;
                CharacterImage.sprite = Picked[1].CharacterPortrait;
                break;
            case Players.Player3:
                title = Picked[2].CharacterName;
                mhp = Picked[2].Health;
                def = Picked[2].Defense;
                atk = Picked[2].Attack;
                UISprite = Picked[2].Splash;
                range = Picked[2].range;
                CharacterImage.sprite = Picked[2].CharacterPortrait;
                break;
            case Players.Player4:
                title = Picked[3].CharacterName;
                mhp = Picked[3].Health;
                def = Picked[3].Defense;
                atk = Picked[3].Attack;
                UISprite = Picked[3].Splash;
                range = Picked[3].range;
                CharacterImage.sprite = Picked[3].CharacterPortrait;
                break;
            default:
                //preset in the script, anything other than a player will go here
                print("Non-player");
                break; 
        }

        hp = mhp;
    }

    public void Awake()
    {
        inBattle = false;
    }

    public void createPacket(float MHP, float ATK, float DEF) //Easy data entry for stat changes
    {
        mhp = MHP;
        hp = MHP;
        atk = ATK;
        def = DEF;
    }

    public void AddToBattle(BattleScene BS, int ID, bool IB) //Adds data relevant to battle when added
    {
        battleScene = BS;
        slotID = ID;
        inBattle = IB;
    }

    public void RemoveFromBattle() //Removes data from CharacterPacket when unadded
    {
        battleScene = null;
        slotID = -1;
        inBattle = false;
    }

    public void modifyHP(float subfromHP) //Subtracts amount from HP
    {
        hp -= subfromHP;
    }
}
