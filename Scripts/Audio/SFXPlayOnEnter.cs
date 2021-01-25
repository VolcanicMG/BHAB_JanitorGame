/*
 * Scripted By: Wolfgang Sandtner
 * Date: 3/11/2020
 * Purpose: This script is applied to Game Objects that have colliders that trigger Sound Effects
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXPlayOnEnter : MonoBehaviour
{
    //Variables:
    public int SoundEffectNumber; //Sound Effect to play

    private SFXManager sfx; //Reference to SFXManager Script

    // Start is called before the first frame update
    void Start()
    {
        sfx = FindObjectOfType<SFXManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            sfx.SFXPlayOnEnter(SoundEffectNumber); //Plays the Sound Effect assigned to gameObject
        }
    }
}
