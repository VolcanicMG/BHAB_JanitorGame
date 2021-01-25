using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateSteam : MonoBehaviour
{

    private float Ready = 0f;

    [SerializeField]
    private GameObject ImageButton;

    [SerializeField]
    ParticleSystem Steam;

    [SerializeField]
    ParticleSystem Steam2;

    [SerializeField]
    ParticleSystem Steam3;

    [SerializeField]
    ParticleSystem Steam4;

    //play steam?
    bool playSteam = false;

    //when the scene starts disable the button
    void Start()
    {
        ImageButton.GetComponent<Renderer>().enabled = false;
    }

    //once the player enters the collision area enable the button and see if they want to press E
    void OnTriggerEnter2D(Collider2D collision)
    {


        //sets the button visable
        ImageButton.GetComponent<Renderer>().enabled = true;

        //Start the steam
        Ready = 1f;

    }

    //once the player leave disable the button and stop checking for E
    private void OnCollisionExit2D(Collision2D collision)
    {
        //sets the button invisable
        ImageButton.GetComponent<Renderer>().enabled = false;
        Ready = 0f;
    }

    /*
    private void OnDrawGizmosSelected()
    {
        // Draw a semitransparent blue cube at the transforms position
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(targetPos, 0.1f);
    }
    */

    void Update()
    {

        if (Ready == 1f)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                //Console
                Debug.Log("Steam On");

                playSteam = !playSteam;
            }

            if (playSteam)
            {
                if (!Steam.isPlaying || !Steam2.isPlaying || !Steam3.isPlaying || !Steam4.isPlaying)
                {
                    Steam.Play();
                    Steam2.Play();
                    Steam3.Play();
                    Steam4.Play();
                }
            }
            else
            {
                if (Steam.isPlaying || Steam2.isPlaying || Steam3.isPlaying || Steam4.isPlaying)
                {
                    Steam.Stop();
                    Steam2.Stop();
                    Steam3.Stop();
                    Steam4.Stop();
                }

            }

        }

    }
}
 