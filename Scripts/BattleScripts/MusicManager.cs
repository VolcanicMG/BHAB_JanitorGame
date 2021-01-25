using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public Animator mapMusic;
    public Animator battleMusic;

    public AudioSource mMusic;
    public AudioSource bMusic;

    private bool willPause = false;

    private void Start()
    {
        mMusic.Stop();
        bMusic.Stop();
        mMusic.Play();
        bMusic.Play();
        mapMusic.SetBool("isPlaying", true);
        mapMusic.SetTrigger("FullVolume");
        battleMusic.SetTrigger("Mute");
    }

    public void Switch()
    {
        if (mapMusic.GetBool("isPlaying"))
        {
            mapMusic.SetTrigger("FadeOut");
            battleMusic.SetTrigger("FadeIn");
            mapMusic.SetBool("isPlaying", false);
        }
        else
        {
            mapMusic.SetTrigger("FadeIn");
            battleMusic.SetTrigger("FadeOut");
            mapMusic.SetBool("isPlaying", true);
        }
    }
}
