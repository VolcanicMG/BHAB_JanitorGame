using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScientistTextManager : MonoBehaviour
{

    [SerializeField]
    private GameObject textBox;

    [SerializeField]
    private Text theText;

    [SerializeField]
    private TextAsset textFile;
    [SerializeField]
    private string[] textLines;

    [SerializeField]
    private int currentLine;
    [SerializeField]
    private int endAtLine;

    private bool activeText;

    public ItemVariables itemVariables;

    //designates Individual lockers
    public GameObject itemText;

    // Start is called before the first frame update
    void Start()
    {
        if (textFile != null)
        {
            textLines = textFile.text.Split('\n');
        }

        if (endAtLine == 0)
        {
            endAtLine = textLines.Length - 1;
        }

        itemVariables = itemText.GetComponent<ItemVariables>();
        //theText.text = textLines[currentLine];
        //sets the Text invisible
        // theText.GetComponent<Renderer>().enabled = false;
        //  theText.GetComponent<Renderer>().enabled = false;

    }

    // Update is called once per frame
    void Update()
    {
        currentLine = itemVariables.designation;
        theText.text = textLines[currentLine];
    }



}