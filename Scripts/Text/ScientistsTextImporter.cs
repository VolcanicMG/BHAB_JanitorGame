using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ScientistsTextImporter : MonoBehaviour
{

    [SerializeField]
    private TextAsset textFile;
    [SerializeField]
    private string[] textLines;

    // Start is called before the first frame update
    void Start()
    {
     if(textFile != null)
        {
            textLines = textFile.text.Split('\n');
        }  
    }
}
