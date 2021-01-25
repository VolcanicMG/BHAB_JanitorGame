using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterList : MonoBehaviour
{
    internal bool FirstTick;

    internal static List<CharacterSelectObject> PickedCharacters = new List<CharacterSelectObject>(); //The characters that where selected from the boxes

    [Header("Character Specials")]
    [SerializeField]
    public Sprite LockedSprite;

    [Header("Character List")]
    [SerializeField]
    public List<CharacterSelectObject> Characters = new List<CharacterSelectObject>(); //Might move this over to a global script so it can be instantiated once'

    [Header("Check Boxes")]
    [SerializeField]
    private CharacterSlectionBox1 Box1;

    [SerializeField]
    private CharacterSlectionBox2 Box2;

    [SerializeField]
    private CharacterSlectionBox3 Box3;

    [SerializeField]
    private CharacterSlectionBox4 Box4;

    [System.Serializable]
    public class CharacterSelectObject
    {
        [Header("Character UI")]
        public Sprite Splash;
        public string CharacterName;
        //public Color CharacterColor;
        public Sprite CharacterPortrait;

        public bool Locked;

        [Header("Character Stats")]
        public float Health;
        public float Attack;
        public float Defense;
        public int range;

        internal bool Picked;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if ((Box1.ReadyToGo && Box2.ReadyToGo && Box3.ReadyToGo && Box4.ReadyToGo) && !FirstTick)
        {
            FirstTick = true;

            //Add the characters that were selected to the list
            PickedCharacters.Add(Characters[Box1.SelectedCharacter]);
            PickedCharacters.Add(Characters[Box2.SelectedCharacter]);
            PickedCharacters.Add(Characters[Box3.SelectedCharacter]);
            PickedCharacters.Add(Characters[Box4.SelectedCharacter]);

            //foreach(int Character in PickedCharacters)
            //{
            //    print(Character);
            //}
        }
    }
}
