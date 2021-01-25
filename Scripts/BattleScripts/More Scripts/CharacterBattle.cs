using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterBattle : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private State state;
    private Vector3 slideTargetPosition;
    private Action onSlideComplete;
    private bool isPlayerTeam;
    private GameObject selectionCircleGameObject;

    private HealthSystem healthSystem;

    private CharacterPacket characterPacket;

    private enum State
    {
        Idle,
        Sliding, 
        Busy
    }


    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        characterPacket = gameObject.AddComponent<CharacterPacket>();
        characterPacket.createPacket(100, 40, 20);

        healthSystem = new HealthSystem(100);

        selectionCircleGameObject = transform.Find("SelectionCircle").gameObject;
        HideSelectionCircle();
        state = State.Idle;
    }

    private void Start()
    {
        
    }

    public void Setup(bool isPlayerTeam)
    {
        this.isPlayerTeam = isPlayerTeam;
        if (isPlayerTeam)
        {
            //spriteRenderer.sprite = BattleHandler.GetInstance().playerSpritesheet;
        }
        else
        {
            //spriteRenderer.sprite = BattleHandler.GetInstance().enemySpritesheet;
        }
    }

    private void PlayAnimIdle()
    {
        if (isPlayerTeam)
        {

        } else
        {

        }
    }

    void Update()
    {
        switch (state)
        {
            case State.Idle:
                break;
            case State.Busy:
                break;
            case State.Sliding:
                float slideSpeed = 10f;
                transform.position += (slideTargetPosition - GetPosition()) * slideSpeed * Time.deltaTime;

                float reachedDistance = 1f;
                if (Vector3.Distance(GetPosition(), slideTargetPosition) < reachedDistance)
                {
                    transform.position = slideTargetPosition;
                    onSlideComplete();
                }
                break;
        }
    }

    public Vector3 GetPosition()
    {
        return transform.position;
    }


    public void Attack(CharacterBattle targetCharacterBattle, Action onAttackComplete)
    {
        Vector3 slideTargetPosition = targetCharacterBattle.GetPosition() + (GetPosition() + targetCharacterBattle.GetPosition()).normalized * 10f;
        Vector3 startingPosition = GetPosition();

        SlideToPosition(slideTargetPosition, () =>
        {
            state = State.Busy;
            targetCharacterBattle.Damage(20);
            SlideToPosition(startingPosition, () =>
            {
                state = State.Idle;
                onAttackComplete();
            });
        });
    }

    private void SlideToPosition(Vector3 slideTargetPosition, Action onSlideComplete)
    {
        this.slideTargetPosition = slideTargetPosition;
        this.onSlideComplete = onSlideComplete;

        state = State.Sliding;
        
    }

    public void HideSelectionCircle()
    {
        selectionCircleGameObject.SetActive(false);
    }

    public void ShowSelectionCircle()
    {
        selectionCircleGameObject.SetActive(true);
    }

    public CharacterPacket getPacket()
    {
        return characterPacket;
    }

    public void Damage(int damage)
    {
        healthSystem.Damage(damage);
    }

    public bool isDead()
    {
        return healthSystem.isDead();
    }
}

