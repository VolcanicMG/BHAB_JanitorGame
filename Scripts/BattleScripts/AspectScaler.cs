using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AspectScaler : MonoBehaviour
{
    

    // Start is called before the first frame update
    void Start()
    {
        if (Camera.main.aspect >= 1.7)
        {
            Debug.Log("16:9");
        }
        else if (Camera.main.aspect >= 1.6)
        {
            Debug.Log("16:10");
        }
        else if (Camera.main.aspect >= 1.4)
        {
            Debug.Log("3:2");
            Vector2 movement = new Vector2(0, 1);
            this.transform.Translate(0, 0, 0);
        }
        else if (Camera.main.aspect >= 1.3)
        {
            Debug.Log("4:3");
            Screen.SetResolution(400, 300, false);
            this.transform.Translate(0, 0, 0);
        }
        else
        {
            Debug.Log("5:4");
            Screen.SetResolution(500, 400, false);
            this.transform.Translate(0, 0, 0);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
}
