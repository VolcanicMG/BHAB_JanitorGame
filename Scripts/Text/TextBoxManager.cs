using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextBoxManager : MonoBehaviour
{
    //the box the text sits in
    [SerializeField]
    private GameObject textBox;
    //the actual text on screen
    [SerializeField]
    private Text theText;
    //the file the text is drawn from
    [SerializeField]
    private TextAsset textFile;
    //the array that is filled 1 line at a time from the text file
    [SerializeField]
    private string[] textLines;
    //this is the spot in the array that is displayed
    [SerializeField]
    private int currentLine;

    //calls on the variables in this script
    public ItemVariables itemVariables;

    //designates Individual lockers
    public GameObject itemText;

    // Start is called before the first frame update
    void Start()
    {
        //makes sure there is a texfile and splits lines
        if (textFile != null)
        {
            textLines = textFile.text.Split('\n');
        }
        //stores the variables for the items
        itemVariables = itemText.GetComponent<ItemVariables>();

    }

    // Update is called once per frame
    void Update()
    {
        //sets the current line to be the specific item
        currentLine = itemVariables.designation;
        //sets the text to match the item that shares the same number as the text's spot in the array
        theText.text = textLines[currentLine];
    }



}