using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory2 : MonoBehaviour
{
    //array checking if inventory slot is filled
    public bool[] isFull;
    //array holding all of the inventory slots
    public GameObject[] slots;
    //array that keeps track of what item is what slot for each character
    public int[] slotItemRelation;

    public void Start()
    {
        print("Created slot item relations");
        slotItemRelation = new int[9];

        for (int i = 0; i < 9; i++)
        {
            slotItemRelation[i] = -1;
        }
    }
}
