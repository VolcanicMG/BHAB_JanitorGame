using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndDemoScript : MonoBehaviour
{
    //Variables:
    
    //Start
    void Start()
    {
        
    }

    //Functions
    void OnTriggerEnter2D(Collider2D other)
    {
        
        if (other.tag == ("Player"))// = GameObject.FindGameObjectsWithTag("Player"))
        {
            SceneManager.LoadScene(2);
        }
    }
}
