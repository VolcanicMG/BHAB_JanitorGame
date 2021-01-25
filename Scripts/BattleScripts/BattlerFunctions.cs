using System;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

public class BattlerFunctions : MonoBehaviour
{
    public event EventHandler OnSelection;
    public CharacterPacket pack;

    private bool trackMouse = false;

    private GameObject card;
    private bool isLoaded = false;
    private Text txt;
    private RectTransform panel;

    private string infotxt = "";

    private BattleUIHandler UIHandler;

    private void Start()
    {
        //Load data for info display card.
        Addressables.LoadAssetAsync<GameObject>("ResizableCard").Completed += onLoadDone;

        infotxt = pack.title +
            "\n" +
            "\nHP: " + pack.hp + "/" + pack.mhp +
            "\nATK: " + pack.atk +
            "\nDEF: " + pack.def;

        
    }

    //Instatiates info display card.
    private void onLoadDone(UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle<GameObject> obj)
    {
        Vector3 pos = new Vector3(0, 0, 0);

        card = Instantiate(obj.Result, pos, Quaternion.identity, transform);

        txt = card.GetComponentInChildren<Text>();
        panel = card.GetComponentInChildren<RectTransform>();
        card.GetComponent<RectTransform>().localPosition = pos;
        card.name = "Card";
        card.SetActive(false);

        isLoaded = true;
    }


    //Runs upwards into BattleScene. Selects this character and deselects all others
    public void Select()
    {
        print("Selected "+pack.title);
        OnSelection?.Invoke(this, EventArgs.Empty);
    }

    //Shows info display card when mouse hovers over character
    public void showCard()
    {
        if (isLoaded)
        {
            infotxt = pack.title +
            "\n" +
            "\nHP: " + pack.hp + "/" + pack.mhp +
            "\nATK: " + pack.atk +
            "\nDEF: " + pack.def;
            txt.text = infotxt;
            panel.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, txt.preferredHeight / 10 + .1f);
            card.SetActive(true);

            trackMouse = true;
        }
    }

    //Hides info display card when mouse stops hovering.
    public void hideCard()
    {
        if (isLoaded)
        {
            card.SetActive(false);
            trackMouse = false;
        }
    }

}
