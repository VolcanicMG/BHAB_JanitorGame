using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class UpdateGUIHealthBars : MonoBehaviour
{
    //Variables:
    //Players
    public GameObject player1;
    public GameObject player2;
    public GameObject player3;
    public GameObject player4;

    //Green Health Bars
    public Transform GreenBar1;
    public Transform GreenBar2;
    public Transform GreenBar3;
    public Transform GreenBar4;

    //Icons
    public GameObject playerIcon1;
    public GameObject playerIcon2;
    public GameObject playerIcon3;
    public GameObject playerIcon4;

    //Players HP Values
    private float MaxHP1;
    private float MaxHP2;
    private float MaxHP3;
    private float MaxHP4;
    private float CurrentHP1;
    private float CurrentHP2;
    private float CurrentHP3;
    private float CurrentHP4;

    // Start is called before the first frame update
    void Start()
    {
        MaxHP1 = player1.GetComponent<CharacterPacket>().mhp;
        MaxHP2 = player2.GetComponent<CharacterPacket>().mhp;
        MaxHP3 = player3.GetComponent<CharacterPacket>().mhp;
        MaxHP4 = player4.GetComponent<CharacterPacket>().mhp;
        CurrentHP1 = MaxHP1;
        CurrentHP2 = MaxHP2;
        CurrentHP3 = MaxHP3;
        CurrentHP4 = MaxHP4;
    }

    // Update is called once per frame
    void Update()
    {
        try { CurrentHP1 = player1.GetComponent<CharacterPacket>().hp; } catch (Exception e) { CurrentHP1 = 0; }
        try { CurrentHP2 = player2.GetComponent<CharacterPacket>().hp; } catch (Exception e) { CurrentHP2 = 0; }
        try { CurrentHP3 = player3.GetComponent<CharacterPacket>().hp; } catch (Exception e) { CurrentHP3 = 0; }
        try { CurrentHP4 = player4.GetComponent<CharacterPacket>().hp; } catch (Exception e) { CurrentHP4 = 0; }

        UpdateHealthbar(CurrentHP1, MaxHP1, GreenBar1, playerIcon1);
        UpdateHealthbar(CurrentHP2, MaxHP2, GreenBar2, playerIcon2);
        UpdateHealthbar(CurrentHP3, MaxHP3, GreenBar3, playerIcon3);
        UpdateHealthbar(CurrentHP4, MaxHP4, GreenBar4, playerIcon4);
        
    }

    //Functions:
    private void UpdateHealthbar(float hp, float mhp, Transform bar, GameObject icon) //Updates the GUI healthbars
    {
        float healthPercent = hp / mhp;
        if (hp <= 0) healthPercent = 0f;

        bar.GetComponent<Image>().fillAmount = healthPercent; //Shrinks healthbar

        if(healthPercent <= 0)
        {
            Destroy(icon); //Removes players icon from hud
        }
    }
}
