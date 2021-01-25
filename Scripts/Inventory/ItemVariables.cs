using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemVariables : MonoBehaviour
{
    //this helps change the state of an item from clickable to draggable
    public bool inInventory = false;
    //this keeps track of what spot in an inventory an item is
    public int inventorySpot;
    // this is the designation of each individual item, set to 6 by default for testing purposes
    public int designation = 6;
    //this designates the skill id that is attached to the item
    public int skillID;
    //this determines if the item changes movement
    public int range;
}
