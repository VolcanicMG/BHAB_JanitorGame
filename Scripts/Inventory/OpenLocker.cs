using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class OpenLocker : MonoBehaviour
{

    private float Ready = 0f;

    private SpriteRenderer spriteR;

    //private Vector3[] originalPosition;

    [SerializeField]
    private Sprite sprite;

    [SerializeField]
    private GameObject text;

    //[SerializeField]
    //private GameObject[] items;

    //[SerializeField]
    //private GameObject itemText;

    //[HideInInspector]
    //public int randomNumber;

    [SerializeField]
    private int randomMin;

    [SerializeField]
    private int randomMax;

    //public UIParticleSystem uIParticleSystem;
    //public GameObject particleSystem1;

    //[HideInInspector]
    //public bool playParticle = true;



    //public ItemVariables itemVariables;

    [SerializeField]
    private Button inventoryButton;

    private Vector3 trans = new Vector3(833.8f, 541.3f, -221.1f);
    private GameObject hit;


    //when the scene starts disable the button
    void Start()
    {
        text.GetComponent<Renderer>().enabled = false;
        spriteR = gameObject.GetComponent<SpriteRenderer>();

        //itemVariables = itemText.GetComponent<ItemVariables>();


        ////deactivate all items
        //for (int c = 0; c < items.Length; c++)
        //{
        //items[c].transform.gameObject.SetActive(false);
        //}
        //itemText.transform.gameObject.SetActive(false);

    }

    //once the player enters the collision area enable the button and see if they want to press E
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (Ready == 0)
        {
            hit = collision.transform.parent.gameObject;
            text.GetComponent<Renderer>().enabled = true;

            //clickable
            Ready = 1f;
        }
    }

    //once the player leave disable the button and stop checking for E
    private void OnCollisionExit2D(Collision2D collision)
    {

        if (Ready == 1)
        {
            //not clickable
            Ready = 0f;
            text.GetComponent<Renderer>().enabled = false;
        }

    }

    private void OnTriggerExit2D(Collider2D collision)
    {
            text.GetComponent<Renderer>().enabled = false;
    }

    private void Update()
    {

    

    }

    public void pressed()
    {
        if (Ready == 1f && MGTurnManager.GetAddOnUI().GetCurrentTurn().gameObject == hit)
        {
            text.GetComponent<Renderer>().enabled = false;

                //Console
                Debug.Log("itemText open");

                spriteR.sprite = sprite;
                inventoryButton.onClick.Invoke();
                Ready = 2f;

                Camera.main.GetComponentInChildren<Inventory_Or_Open>().itemGen(randomMin, randomMax);

                ////Pick a random item
                //randomNumber = Random.Range(randomMin, randomMax);
                //itemVariables.randomNumber = randomNumber;

                ////Activate that item
                //items[randomNumber].transform.gameObject.SetActive(true);
                //itemText.transform.gameObject.SetActive(true);
                

                ////activate particles
                //particleSystem1.GetComponent<UIParticleSystem>().ploxPlay = true;
        }
    }
}
