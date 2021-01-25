using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class DemoCutSceneScript : MonoBehaviour
{
    //Variables:
    public GameObject[] objects;
    public GameObject text;
    public Image blackImage;
    public Animator demoCutsceneAnim;

    private GameObject current;

    private int item = 0;
    private Boolean movingObject = true;
    private static System.Random rng = new System.Random();
    private Vector3 endPos = Vector3.zero;
    private float xVec = 0;
    private float yVec = 0;
    private Boolean disable = false;

    // Start is called before the first frame update
    void Start()
    {
        Shuffle(objects); //Randomizes the order of array
    }

    // Update is called once per frame
    void Update()
    {
        if (!disable)
        {
            if (movingObject)
            {
                if (item >= objects.Length)
                {
                    Shuffle(objects); //Shuffles Array
                    item = 0;
                }
                movingObject = false;
                current = objects[item++];
                if (current.transform.position.x > 0)
                    xVec = -4F;
                else
                    xVec = 4F;
                if (current.transform.position.y > 0)
                    yVec = -2F;
                else
                    yVec = 2F;
                endPos = new Vector3(current.transform.position.x * -1, current.transform.position.y * -1, 0F);

                current.transform.localScale = new Vector3(current.transform.localScale.x * -1F, 1F, 1F); //Flips object
            }
            else
            {
                float xPos = current.transform.position.x;
                float yPos = current.transform.position.y;
                current.transform.position += new Vector3(xVec, yVec, 1/1) * Time.deltaTime;


                if (current.transform.position.y > endPos.y && current.transform.position.y < endPos.y + .4F) //Because screen is a rectangle, this uses the shorter distance of Y to test when object has crossed the screen
                {
                    current.transform.position = endPos;
                    movingObject = true;
                }
            }

            //Updates Text position
            if (text.transform.position.y < 33.5F) //1450 for last line of text ending in center screen
            {
                RectTransform text_RT = text.GetComponent<RectTransform>();
                text_RT.anchoredPosition = new Vector2(text_RT.anchoredPosition.x, text_RT.anchoredPosition.y + 2F * Time.deltaTime);
            }
            else
            {
                current.transform.position = new Vector2(100, 100);
                StartCoroutine(Fading()); //Transitions to MenuScene
                disable = true;
            }
        }
    }

    //Functions:
    public static void Shuffle(GameObject[] objs) //Shuffles an array of GameObjects into a random order
    {
        int n = objs.Length;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            GameObject value = objs[k];
            objs[k] = objs[n];
            objs[n] = value;
        }
    }

    IEnumerator Fading()
    {
        demoCutsceneAnim.SetBool("FadeOut", true);
        yield return new WaitUntil(() => blackImage.color.a > .95F);
        SceneManager.LoadScene(0);
    }
}
