using System.Collections;
using System.Collections.Generic;
using System.Runtime.Hosting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DemoMenuScript : MonoBehaviour
{
    public void PlayGame()
    {
        SceneManager.LoadScene(sceneName: "CharacterSelection");
        print("Moving");
    }

    public void QuitGame()
    {
        Application.Quit();
    }

}
