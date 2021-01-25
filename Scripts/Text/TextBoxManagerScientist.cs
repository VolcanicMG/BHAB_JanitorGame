using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextBoxManagerScientist : MonoBehaviour
{

    [SerializeField]
    private Canvas TalkBox;

    [SerializeField]
    private Text theText;

    [SerializeField]
    private TextAsset textFile;
    [SerializeField]
    private string[] textLines;

    [SerializeField]
    private int lineStartAt;



    // Start is called before the first frame update
    void Start()
    {
        //make sure there is a text file and split lines at enter
        if (textFile != null)
        {
            textLines = textFile.text.Split('\n');
        }

        //disable the textbox at the start of the scene
        TalkBox.enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {

        //sets the Bubble visible
        TalkBox.enabled = true;
        //grabs the indicator for the character that is active
        int characterDes = MGTurnManager.GetAddOnUI().GetCurrentTurn().GetComponentInChildren<CharacterPacket>().charDesignation;
        //adds the starting line for the scientist + the character desegnator so that the line is specialized for each line
        int currentLine = lineStartAt + characterDes;
        //sets the text by an array filled by the txt file
        theText.text = textLines[currentLine];
    }
}