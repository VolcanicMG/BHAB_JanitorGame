using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressButton : MonoBehaviour
{
    private float Ready = 0f;

    private SpriteRenderer spriteR;

    [SerializeField]
    private Sprite sprite;

    [SerializeField]
    private GameObject ImageButton;

    [SerializeField]
    //private UnityEngine.Experimental.Rendering.LWRP.Light2D Global_Light_2D;



    //when the scene starts disable the E
    void Start()
    {
        ImageButton.GetComponent<Renderer>().enabled = false;
        spriteR = gameObject.GetComponent<SpriteRenderer>();

        //The lights go out
        //Global_Light_2D.enabled = false;

    }

    //once the player enters the collision area enable the button and see if they want to press E
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (Ready == 0)
        {
            //sets the button visable
            ImageButton.GetComponent<Renderer>().enabled = true;

            //Press E?
            Ready = 1f;
        }
    }

    //once the player leave disable the button and stop checking for E
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (Ready == 1)
        {
            //sets the button invisable
            ImageButton.GetComponent<Renderer>().enabled = false;
            Ready = 0f;
        }
    }

    private void Update()
    {

        if (Ready == 1f)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                //Console
                Debug.Log("Button Pressed");

                ImageButton.GetComponent<Renderer>().enabled = false;

                //if pressed changed the sprite of the game object
                spriteR.sprite = sprite;

                //Once the button is pressed dont let it be pressed again.
                Ready = 2f;


                //the lights turn on
                //Global_Light_2D.enabled = true;




}



        }

    }
}
