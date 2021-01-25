using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenDoor : MonoBehaviour
{
    [SerializeField]
    private Vector3 targetPos;

    [SerializeField]
    private GameObject Object;

    private float Ready = 0f;

    private SFXManager sfx; //Reference to SFXManager Script

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Console
        Debug.Log("Open Right/Left");


        //change the position of the door
        Ready = 1f;

    }
   

    private void OnDrawGizmosSelected()
    {
        // Draw a semitransparent blue cube at the transforms position
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(targetPos, 0.1f);
    }

    //Start
    private void Start()
    {
        sfx = FindObjectOfType<SFXManager>();
    }

    //Update the position of the object (Door)
    void Update()
    {
        //If the button has been touch the objcet will move
        if(Ready == 1f)
        {
            // Object at position transforming to its position but to the next position
            Object.transform.position = Vector3.Lerp(Object.transform.position, targetPos, 3 * Time.deltaTime);
            if(Object.transform.position == targetPos)
            {
                //once the object reaches its desination stop
                Ready = 0f;
                //sfx.SFXPlayOnEnter(1);//Plays DoorSliding Sound Effect
            }
        }
        
    }
}
