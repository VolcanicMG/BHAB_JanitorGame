using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DropOff : MonoBehaviour
    , IDropHandler

{

    private Inventory2 inventory;
    public int slotNum;
    public bool wait = false;


    public void OnDrop(PointerEventData eventData)
    {
        Debug.Log("OnDrop");
        if (eventData.pointerDrag.GetComponent<Pickup>().itemVariables.inInventory)
        {
                GameObject item = eventData.pointerDrag;
                //changing the anchored position of the item to the anchored position of the slot it was dropped on
                item.GetComponent<RectTransform>().anchoredPosition = transform.position;
                //changing the position of the item to the position of the slot it was dropped on
                item.GetComponent<RectTransform>().position = transform.position;
                //changing the parent of the item to the slot it was dropped on
                item.GetComponent<RectTransform>().SetParent(transform);
                //setting the is full variable to false for the old position of the item
                item.GetComponent<Pickup>().inventory.isFull[item.GetComponent<ItemVariables>().inventorySpot] = false;
                //designate the slot as empty in numbers
                item.GetComponent<Pickup>().inventory.slotItemRelation[item.GetComponent<ItemVariables>().inventorySpot] = -1;
                //setting the position of the item to be equal to the new slot it is in
                item.GetComponent<ItemVariables>().inventorySpot = GetComponent<SlotDesignator>().slotNum;
                //setting the number value in the relation array equal to the items designation
                item.GetComponent<Pickup>().inventory.slotItemRelation[item.GetComponent<ItemVariables>().inventorySpot] = item.GetComponent<ItemVariables>().designation;
                //set the is full variable to true for the new position of the item
                item.GetComponent<Pickup>().inventory.isFull[GetComponent<SlotDesignator>().slotNum] = true;
                //signal the pick up script that this script did fire (this is so that the pick up script doesn't reset the position of the item to it's previous position)
                item.GetComponent<Pickup>().droppedOnSlot = true;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        //inventory = GameObject.FindGameObjectWithTag("Player").GetComponent<Inventory2>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
