/*
 * Scripted By: Wolfgang Sandtner
 * Date: 3/11/2020
 * Purpose: This script is applied to the SFX Game Object and handles the overall managing and playing of Sound Effects
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXManager : MonoBehaviour
{
    //Variables:
    private AudioSource[] SoundEffects; //Holds the Sound Effects

    public bool SFXEnabled; //Enables overall Sound Effects

    private bool SFXPlay; //Tells SFXManager to play current Sound Effect

    public int SFXNumber; //Current Sound Effect to play

    // Start is called before the first frame update
    void Start()
    {
        SoundEffects = GetComponentsInChildren<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (SFXEnabled)
        {
            if (SFXPlay)
            {
                SFXPlay = false;
                SoundEffects[SFXNumber].Play();
            }
        }
    }

    //Functions:
    public void SFXPlayOnEnter(int num) //Used to play a sound effect when gameObject collides with script object
    {
        //SoundEffects[SFXNumber].Stop();
        //SFXNumber = num;
        //SoundEffects[SFXNumber].Play();
        //SFXPlay = true;
    }

    public void SFXPlayFootstep()
    {
        for (int i = 0; i < SoundEffects.Length; i++)
        {
            if (SoundEffects[i].isPlaying && SoundEffects[i].time < .25f)
            {
                return;
            }
        }

        int x = SFXNumber;
        while (x == SFXNumber)
        {
            SFXNumber = Random.Range(1, 4);
        }

        SoundEffects[SFXNumber].Play();
    }

    public void SFXPlaySE(int SE)
    {
        SoundEffects[SE].Play();
    }
}
