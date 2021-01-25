using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSlectionBox2 : MonoBehaviour
{
    private int SelectCharacterIndex; //index for the list
    internal int SelectedCharacter; //The selected character for this box
    private Color SelectedColor; //Used to change the color for the blinking effect

    private bool startBlinking = false;

    internal bool ReadyToGo = false; //The character has been selected and it ready to go

    [Header("Background Speed")]
    [SerializeField]
    private float BackgroundTransitionSpeed = 5f;

    [Header("Character List")]
    [SerializeField]
    private CharacterList List;

    [Header("UI References")]
    [SerializeField]
    private TextMeshProUGUI characterName;

    [SerializeField]
    private Image CharacterSplash;

    [SerializeField]
    private Image backgroundColor;

    [Header("Stats")]
    [SerializeField]
    private TextMeshProUGUI CharacterStats;


    [Header("Check UI References")]
    [SerializeField]
    private Image CheckReady;

    [SerializeField]
    private Sprite Ready;

    [SerializeField]
    private Sprite NotReady;

    [SerializeField]
    private Button ConfirmButton;


    [Header("UI Click Sound")]
    [SerializeField]
    private AudioSource ClickSource;
    [SerializeField]
    private AudioClip Click;

    [Header("UI Confirm Sound")]
    [SerializeField]
    private AudioClip Confirm;
    [SerializeField]
    private AudioSource ConfirmSource;

    private void UpdateCharacterSelectionUI()
    {

        //Update the Splash, Name, and desired color
        if (List.Characters[SelectCharacterIndex].Locked) //If the character is locked do this...
        {
            CharacterSplash.sprite = List.LockedSprite;
            characterName.text = "???";

            ConfirmButton.interactable = false;

            CharacterStats.text = $"Health: ??? \n" +
                $"Attack: ??? \n" +
                $"Defense: ???";
        }
        else
        {
            CharacterSplash.sprite = List.Characters[SelectCharacterIndex].Splash;
            characterName.text = List.Characters[SelectCharacterIndex].CharacterName;

            ConfirmButton.interactable = true;

            CharacterStats.text = $"Health: {List.Characters[SelectCharacterIndex].Health} \n" +
                $"Attack: {List.Characters[SelectCharacterIndex].Attack} \n" +
                $"Defense: {List.Characters[SelectCharacterIndex].Defense}";
        }

        SelectedColor = Color.white;

    }

    public void Left() //The button to go left
    {
        if (ReadyToGo)
        {
            List.Characters[SelectedCharacter].Picked = false;//Put it back in the list of choices
        }

        CheckReady.sprite = NotReady;
        ReadyToGo = false;

        ClickSource.PlayOneShot(Click);
        SelectCharacterIndex--;
        if (SelectCharacterIndex < 0) SelectCharacterIndex = List.Characters.Count - 1;

        //Check to make sure the character hasn't already been picked.
        while (List.Characters[SelectCharacterIndex].Picked)
        {
            SelectCharacterIndex--;
            if (SelectCharacterIndex < 0) SelectCharacterIndex = List.Characters.Count - 1;
        }

        UpdateCharacterSelectionUI();
    }
    public void Right() //The button to go right
    {
        if (ReadyToGo)
        {
            List.Characters[SelectedCharacter].Picked = false;//Put it back in the list of choices
        }

        CheckReady.sprite = NotReady;
        ReadyToGo = false;

        ClickSource.PlayOneShot(Click);
        SelectCharacterIndex++;
        if (SelectCharacterIndex == List.Characters.Count) SelectCharacterIndex = 0;

        //Check to make sure the character hasn't already been picked.
        while (List.Characters[SelectCharacterIndex].Picked)
        {
            SelectCharacterIndex++;
            if (SelectCharacterIndex == List.Characters.Count) SelectCharacterIndex = 0;
        }

        UpdateCharacterSelectionUI();
    }

    public void ConfirmSelection()
    {
        List.Characters[SelectCharacterIndex].Picked = true; //Find the position in the list and mark picked as true

        ConfirmSource.PlayOneShot(Confirm); //Sound

        SelectedCharacter = SelectCharacterIndex;
        print(List.Characters[SelectedCharacter].CharacterName + " has been chosen!"); //Debug info

        CheckReady.sprite = Ready;
        ReadyToGo = true;

        //start the effect
        startBlinking = true;
        StartCoroutine(SpriteBlinkingEffect());

        Invoke("StopBlinking", .3f); //How long the blinking happens for
    }

    //since we need a way to stop the blinking after a certain amount of time we need to use an Invoke
    private void StopBlinking()
    {
        startBlinking = false;
    }

    //When startBlinking is true blink
    IEnumerator SpriteBlinkingEffect()
    {
        float delay = .06f; //delay for the flashing

        while (startBlinking)
        {
            SelectedColor = Color.black; //the color used to switch out

            yield return new WaitForSeconds(delay);

            SelectedColor = Color.white;

            yield return new WaitForSeconds(delay);
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        UpdateCharacterSelectionUI();
        CheckReady.sprite = NotReady;
        ReadyToGo = false;

    }

    // Update is called once per frame
    void Update()
    {
        if (startBlinking)
        {
            backgroundColor.color = Color.Lerp(backgroundColor.color, SelectedColor, Time.deltaTime * 50);
        }
        else
        {
            backgroundColor.color = Color.Lerp(backgroundColor.color, SelectedColor, Time.deltaTime * BackgroundTransitionSpeed);

        }

        //Debug.Log(SelectCharacterIndex);

        if (List.Characters[SelectCharacterIndex].Picked && !ReadyToGo) //Selection if all or one are on the same character.
        {
            SelectCharacterIndex++;
            if (SelectCharacterIndex == List.Characters.Count) SelectCharacterIndex = 0;
            UpdateCharacterSelectionUI();
        }

    }
}


//TODO - The color mechanic is outdated and needs cleaned up. Cleaned~