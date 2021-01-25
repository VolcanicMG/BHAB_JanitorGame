using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Pickup : MonoBehaviour
    //interfaces my function (interface is a class that can't be instanciated as an object and nothing can be defined. It's basically a collection of methods and property declarations)
    , IPointerClickHandler
    , IPointerDownHandler
    , IDragHandler
    , IBeginDragHandler
    , IEndDragHandler
    , IDropHandler// 2
{

    //creates inventroy
    public Inventory2 inventory;
    //designates Individual Items
    public GameObject itemButton;
    //the transform of the object
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Vector3 DefaultPosition;
    private Transform previousParent;

    public bool droppedOnSlot = false;

    //canvas
    [SerializeField]
    private Canvas canvas;

    public ItemVariables itemVariables;
    private int inventorySpot;

    //item name and description GUI
    [SerializeField]
    private GameObject itemText;



    //private bool inInventory;
    // Start is called before the first frame update
    void Start()
    {
        //attaches inventory to the player
        inventory = MGTurnManager.GetAddOnUI().GetCurrentTurn().GetComponentInChildren<Inventory2>();
        canvas = this.GetComponentInParent<Canvas>();
        Debug.Log(canvas.gameObject);
    }

    public void Awake()
    {
        canvas = GetComponentInParent<Canvas>();
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("OnPointerDown");
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        //let this script access the local variables on the game object
        itemVariables = itemButton.GetComponent<ItemVariables>();
        //recognize what spot the item is in
        inventorySpot = itemVariables.inventorySpot;

        //check to see if it is in the inventory yet
        if (itemVariables.inInventory)
        {
            Debug.Log("onBeginDrag");
            //mouse only interacts with held item while dragging
            canvasGroup.blocksRaycasts = false;
            //makes item a little bit transparent
            canvasGroup.alpha = .6f;
            //sets a position to go back to if not dropped on inventory spot
            DefaultPosition = gameObject.transform.position;
            //set's parent tot go back to if not dropped on inventory spot
            previousParent = gameObject.transform.parent;
            //snaps item's center to mouse position
            itemButton.transform.position = Input.mousePosition;
            //makes item draw above everything else in scene
            gameObject.transform.parent = gameObject.transform.parent.parent.parent;
            //allows other script to control if item was dropped off 
            droppedOnSlot = false;    
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        //let this script access the local variables on the game object
        itemVariables = itemButton.GetComponent<ItemVariables>();
        //recognize what spot the item is in
        inventorySpot = itemVariables.inventorySpot;

        //check to see if it is in the inventory yet
        if (itemVariables.inInventory)
        {
            Debug.Log("onDrag");
            //changes the position of the item to follow the mouse devided by the scale factor of the canvas to make it follow properly
            
            rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        //let this script access the local variables on the game object
        itemVariables = itemButton.GetComponent<ItemVariables>();
        //recognize what spot the item is in
        inventorySpot = itemVariables.inventorySpot;

        if (itemVariables.inInventory)
        {
            //Calls on the drop off script to make sure it landed on an inventory spot and if not the item defaults back to where it was
            if (!droppedOnSlot)
            {
                eventData.pointerDrag.GetComponent<RectTransform>().position = DefaultPosition;
                eventData.pointerDrag.GetComponent<RectTransform>().SetParent(previousParent);
            }

            Debug.Log("OnEndDrag");
            canvasGroup.blocksRaycasts = true;
            canvasGroup.alpha = 1f;
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
        Debug.Log("OnDrop");
    }


    //mouse listener for items
    public void OnPointerClick(PointerEventData eventData)
    {
        //itterates through inventory
        for (int i = 0; i < inventory.slots.Length; i++)
        {
                //checks if inventory slot is empty
                if (inventory.isFull[i] == false)
                {
                    //let this script access the local variables on the game object
                    itemVariables = itemButton.GetComponent<ItemVariables>();
                    //recognize what spot the item is in
                    inventorySpot = itemVariables.inventorySpot;

                    //check to see if it is in the inventory yet
                    if (!itemVariables.inInventory)
                    {
                    //designates slot full
                    inventory.isFull[i] = true;
                   
                    
                    
                    //marking the spot in the inventory
                    itemVariables.inventorySpot = i;
                    
                    //creates copy of item at the location of first available inventory
                    GameObject itemCopy = Instantiate(itemButton, inventory.slots[i].transform, false);
                    DefaultPosition = inventory.slots[i].transform.position;

                    print(inventory.gameObject);
                    inventory.slotItemRelation[i] = itemCopy.GetComponent<ItemVariables>().designation;
                        
                        //destroys original version of the item
                        if (!itemVariables.inInventory)
                        {
                            Destroy(itemButton);
                            itemText = canvas.transform.Find("InventoryMenu").Find("ItemText").gameObject;
                            itemText.transform.gameObject.SetActive(false);
                        }
                        else
                        {
                           Destroy(itemButton);
                        }
                    //sets itemvariable to accessing copy of item
                    itemVariables = itemCopy.GetComponent<ItemVariables>();
                    //copy is in inventory
                    itemVariables.inInventory = true;
                    print("Item is in inventory " + itemVariables.inInventory);

                    MGTurnManager.GetAddOnUI().GetCurrentTurn().GetComponentInChildren<CharacterPacket>().equippedSkillIDs[0] = itemVariables.skillID;
                    MGTurnManager.GetAddOnUI().GetCurrentTurn().GetComponentInChildren<CharacterPacket>().range = itemVariables.range;

                    //stops itteration
                    break;
                    }
            }
        }
    }
}
