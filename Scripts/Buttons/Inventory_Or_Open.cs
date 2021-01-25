using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class Inventory_Or_Open : MonoBehaviour
{

    //feild for the inv button
    [SerializeField]
    private GameObject InvButton;

    [SerializeField]
    private GameObject ExitButton;

    [SerializeField]
    private GameObject DisableButton;

    //Is the game paused?
    public static bool GameIsPaused = false;

    public GameObject loadItems;
    public GameObject loadUISprite;

    [SerializeField]
    private GameObject itemParent;
    [SerializeField]
    private GameObject UISpriteParent;

    [SerializeField]
    private RectTransform[] items;
    [SerializeField]
    private RectTransform[] UISprites;

    [SerializeField]
    private GameObject itemText;

    [HideInInspector]
    public int randomNumber;

    [SerializeField]
    private int randomMin;

    [SerializeField]
    private int randomMax;

    public UIParticleSystem uIParticleSystem;

    [HideInInspector]
    public bool playParticle = true;

    public ItemVariables itemVariables;
    public CharacterPacket characterPacket;
    public GameObject UISprite;

    private RectTransform[] ItemIncinerator;
    private Inventory2 inventory;




    private void Start()
    {
        //once the game is active set the canvas to ber invisable
        GetComponent<Canvas>().enabled = false;

        itemVariables = itemText.GetComponent<ItemVariables>();

        Addressables.LoadAssetAsync<GameObject>("items").Completed += onLoadDone;

        ItemIncinerator = new RectTransform[9]; 
    }

    private void onLoadDone(UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle<GameObject> obj)
    {
        //This should finish before any battle can conceivably start.
        loadItems = obj.Result;

        items = loadItems.GetComponentsInChildren<RectTransform>();
    }


    // Update is called once per frame
    //If the game is paused, run the pause function, if it isnt make sure the game is runnning
    void Update()
    {
        
    }

    //a function that once the button is clicked you will enter the inventory
    public void enterInv()
    {
        //Paused();

        //items[randomNumber].transform.gameObject.SetActive(true);
        //itemText.transform.gameObject.SetActive(true);

        DisableButton.gameObject.SetActive(false);

        GetComponent<Canvas>().enabled = true;
        Debug.Log("Entering inventory");

        InvButton.gameObject.SetActive(false);
        ExitButton.gameObject.SetActive(true);
        inventory = MGTurnManager.GetAddOnUI().GetCurrentTurn().GetComponentInChildren<Inventory2>();
        //UISprite = Instantiate(MGTurnManager.GetAddOnUI().GetCurrentTurn().GetComponentInChildren<CharacterPacket>().UISprite, UISpriteParent.transform);

        

        for (int i = 0; i < inventory.slotItemRelation.Length; i++)
        {
            print("Running Array");
            if (inventory.slotItemRelation[i] >= 0)
            {
                ItemIncinerator[i] = Instantiate(items[inventory.slotItemRelation[i]], inventory.slots[i].transform);
                ItemIncinerator[i].GetComponent<ItemVariables>().inInventory = true;
                ItemIncinerator[i].GetComponent<ItemVariables>().inventorySpot = i;
            }
        }
    }

    //pause the game
    void Paused()
    {
        Time.timeScale = 0f;
        GameIsPaused = true;
    }

    //resume the game
    void Resume()
    {
        Time.timeScale = 1f;
        GameIsPaused = false;
    }


    public void exitInv()
    {
        Resume();

        DisableButton.gameObject.SetActive(true);

        GetComponent<Canvas>().enabled = false;
        Debug.Log("Exiting inventory");

        InvButton.gameObject.SetActive(true);

        ExitButton.gameObject.SetActive(false);

        Destroy(UISprite);

        for (int i = 0; i < inventory.slots.Length; i++)
        {
            foreach(Transform child in inventory.slots[i].transform)
            {
                Destroy(child.gameObject);
            }
        }

    }

    public void itemGen(int randomMinx, int randomMaxx)
    {
        //Pick a random item
        randomNumber = Random.Range(randomMinx, randomMaxx);
        itemVariables.designation = randomNumber;

        //Activate that item
        //items[randomNumber].transform.gameObject.SetActive(true);
        itemText.transform.gameObject.SetActive(true);

        Instantiate(items[randomNumber], itemParent.transform);
        print("Created new item at "+randomNumber);


        //activate particles
        uIParticleSystem.Play();
    }

}
