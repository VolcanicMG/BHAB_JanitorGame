using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class BattlerSlot : MonoBehaviour
{
    public GameObject battleData; //The battle data GameObject
    public CharacterPacket pack; //Character's stat sheet
    public Transform battleGraphic; //Prefab used to display character in battle
    public int slotID; //This slot's position. 1-4, 1 being front, 4 being rear.
    public HealthSystem healthSystem;//TEMP
    public GameObject healthBar;//TEMP

    public BattlerFunctions functions;
    public event EventHandler OnSelect;

    private Vector3 position;//Slot's position
    private GameObject storage;

    public bool isFull = false;
    public bool isPlayer = false;

    void Start()
    {
        //Fill with empty battler
        //pack = gameObject.AddComponent<CharacterPacket>();
    }

    //Called from BattlerFunctions. Passes function to BattleScene
    private void Functions_OnSelection(object sender, System.EventArgs e)
    {
        OnSelect?.Invoke(this, EventArgs.Empty);
    }

    //Fills slot with a dummy, sets permanent position and player/enemy status
    public BattlerSlot Initialize(GameObject loadStorage, int SlotID, bool isPlayer)
    {
        storage = loadStorage; //Gets addressable asset storage from BattleScene
        battleData = storage.transform.Find("Dummy").gameObject; //Basically, just an empty slot which stores data

        slotID = SlotID;
        this.isPlayer = isPlayer;
        pack = battleData.GetComponent<CharacterPacket>();


        //Unit's transform position
        if (isPlayer)
        {
            position = new Vector3(slotID * -2, (slotID % 2) * -.2f);
        }
        else
        {
            position = new Vector3(slotID * 2, (slotID % 2) * -.2f);
        }

        //Normally adds a sprite to battle, does nothing here.
        battleGraphic = Instantiate(battleData.transform.Find("BattlePrefab").transform, position, Quaternion.identity);
        battleGraphic.name = pack.title;
        battleGraphic.SetParent(transform);

        return this;
    }

    //Removes dummy and places a character in the slot
    public void addBattler(GameObject data)
    {
        battleData = data;
        pack = battleData.GetComponent<CharacterPacket>();
        //print(GetComponentInParent<BattleScene>());
        
        //Loads in addressable asset based on prefab address stored in character packet.
        Addressables.LoadAssetAsync<GameObject>(pack.battlePrefabAddress).Completed += onBattlerLoadDone;
    }

    //Runs once battle prefab has loaded
    private void onBattlerLoadDone(UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle<GameObject> obj)
    {
        //Destroys dummy
        Destroy(battleGraphic.gameObject);
        
        //Creates a new object using battle prefab
        battleGraphic = Instantiate(obj.Result.transform, position, Quaternion.identity);
        float height = battleGraphic.GetComponent<RectTransform>().rect.height; //Scales unit's y position based on unit's size so that all unit's appear to be standing on the same level.
        position = new Vector3(position.x, (slotID % 2) * -.2f + ((height - 1) / 2.5f)); //Creates a new position based on unit's height
        battleGraphic.name = pack.title + slotID; //Name used in dev window
        battleGraphic.localScale = new Vector3(1.5f, 1.5f, 1); //Sets scale used in battle
        battleGraphic.SetParent(transform); //Sets BattleScene as parent
        battleGraphic.localPosition = position; //Places unit in correct location
        battleGraphic.GetComponent<BattlerFunctions>().pack = pack;
        battleGraphic.GetComponentInChildren<SpriteRenderer>().sortingOrder = slotID; //Units in the back will appear to be in front of units in the front.
        battleGraphic.gameObject.SetActive(true);

        functions = battleGraphic.GetComponent<BattlerFunctions>(); //Adds functions to BattlerFunctions
        if (functions != null)
        {
            functions.OnSelection += Functions_OnSelection;
        }

        if (pack.spriteVariations.Length > 0) //Adds sprite variations if any exist
        {
            for (int x = 0; x < pack.spriteVariations.Length; x++)
            {
                battleGraphic.transform.Find("Variations").Find(pack.spriteVariations[x]).gameObject.SetActive(true);
            }
        }

        
        //Creates health bar, soon to be depreciated
        Vector3 newPosition = new Vector3(position.x, (slotID % 2) *-.2f - 1.5f, 0f);
        healthBar = Instantiate(storage.transform.Find("HealthBar").gameObject, newPosition, Quaternion.identity);
        healthBar.transform.SetParent(transform);
        healthBar.transform.localPosition = newPosition;
        healthBar.transform.SetParent(battleGraphic);
        healthBar.name = "HealthBar";
        healthBar.SetActive(true);
        healthBar.GetComponent<HealthBar>().SetHealthbar(pack.hp, pack.mhp);
        
        //Selects new unit and does final battle setup.
        pack.AddToBattle(GetComponentInParent<BattleScene>(), slotID, true);
        OnSelect?.Invoke(this, EventArgs.Empty);
        isFull = true;
    }

    //Removes a battler and replaces them with a dummy
    public void removeBattler()
    {
        //Destroys unit and replaces it with dummy.
        pack.battleScene = null;
        pack.inBattle = false;
        Destroy(battleGraphic.gameObject);
        

        battleData = storage.transform.Find("Dummy").gameObject;

        battleGraphic = Instantiate(battleData.transform.Find("BattlePrefab").transform, position, Quaternion.identity);
        battleGraphic.name = pack.title;
        battleGraphic.localScale = new Vector3(.11f, .11f, 1);
        battleGraphic.SetParent(transform);
        battleGraphic.gameObject.SetActive(true);

        pack = battleData.GetComponent<CharacterPacket>();

        Destroy(healthBar.gameObject);

        isFull = false;
    }

    //Destroys a unit on the map
    public void KillBattler()
    {
        print(pack.transform.parent.gameObject.name);
        MGTurnManager.MGRemoveUnit(pack.transform.parent.gameObject.GetComponent<MGMovementController>());
        removeBattler();
    }

    //Changes this slot's position
    public void repositionBattler(int newPos)
    {
        slotID = newPos;

        if (isPlayer)
        {
            position = new Vector3(slotID * -2, (slotID % 2) * -.5f);
        }
        else
        {
            position = new Vector3(slotID * 2, (slotID % 2) * -.5f);
        }

        battleGraphic.transform.SetPositionAndRotation(position, Quaternion.identity);

        if(battleGraphic.name != "Dummy Slot") battleGraphic.name = pack.title + slotID;

        if (healthBar != null)
        {
            Vector3 newPosition = new Vector3(position.x, position.y - 2f);

            healthBar.transform.SetPositionAndRotation(newPosition, Quaternion.identity);
        }

        
    }
}
