using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FadeToNext : MonoBehaviour
{
    private bool firstTick;

    [SerializeField]
    public Image BackgroundFade;

    [SerializeField]
    public CharacterList SceneController;

    [SerializeField]
    public TextMeshProUGUI Countdown;

    // Start is called before the first frame update
    void Start()
    {
        BackgroundFade.enabled = false;
        Countdown.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (SceneController.FirstTick && !firstTick)
        {
            firstTick = true; //Only run once

            BackgroundFade.enabled = true; //enable the background again
            Countdown.enabled = true; //enable the text

            StartCoroutine(CountdownText()); //start the countdown
        }

    }

    IEnumerator CountdownText()
    {
        //Start the countdown, after every second go down a number starting from 3
        for(int i = 3; i > -1; i--)
        {
            if (i == 0)
            {
                break;
            }

            yield return new WaitForSeconds(1);
            Countdown.text = $"{i}";
        }

        PlayGame(); //Start the game once we have all the spots ready
    }

    public void PlayGame()
    {
        StopCoroutine(CountdownText());
        SceneManager.LoadScene(sceneName: "DemoLevel");
    }

}
