using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextBubbles : MonoBehaviour
{

    //[SerializeField]
    //private GameObject TalkBox;

    [SerializeField]
    private Canvas TalkBox;

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


    // Start is called before the first frame update
    void Start()
    {
        TalkBox.enabled = false;

        if (textFile != null)
        {
            textLines = textFile.text.Split('\n');
        }

        if (endAtLine == 0)
        {
            endAtLine = textLines.Length - 1;
        }

        theText.text = textLines[currentLine];


    }

    //once the players enter the collision area enable the bubble
    private void OnTriggerEnter2D(Collider2D collision)
    {
        //sets the Bubble visible
        TalkBox.enabled = false;
        currentLine += 1;
        theText.text = textLines[currentLine];
        activeText = true;
    }
    //once the players leave disable the bubble
    private void OnCollisionExit2D(Collision2D collision)
    {
        //sets the Bubble invisible
        TalkBox.enabled = true;
        currentLine = 0;
        theText.text = textLines[currentLine];
        activeText = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (activeText)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                currentLine += 1;
                theText.text = textLines[currentLine];
            }
        }
        if(currentLine>endAtLine)
        {
            //sets the Bubble invisible
            TalkBox.enabled = false;
            currentLine = 0;
            theText.text = textLines[currentLine];
            activeText = false;
        }

    }
}
