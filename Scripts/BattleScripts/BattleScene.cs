using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

//The actual battle itself. Controls all functions of the battle
public class BattleScene : MonoBehaviour
{
    private BattlerSlot[] playerSlots; //Array containing player team
    private BattlerSlot[] enemySlots; //Array containing enemy team

    private BattleUIHandler UIHandler; //Script which controls UI functions
    private GameObject pointer;


    private UnityAction skillButtonEvent; //Event which runs when a skill button is hit
    private UnityAction runButtonEvent; //Event which runs when a skill button is hit
    private UnityAction defendButtonEvent; //Event which runs when a skill button is hit
    private UnityAction switchButtonEvent; //Event which runs when a skill button is hit
    private SkillDatabase skillDB; //Database of all skills
    private BattlerSlot currentSlot; //The ID (0-3) of the current slot.
    private int skillSlot; //References a spot in the equippedSkillIDs array in CharacterPacket
    

    private int selectedAllyPos; //The currently selected player slot
    private int selectedEnemyPos; //The currently selected enemy slot

    //Loads data from BattleSceneManager, then fills battle with empty slots.
    public void Initialize(GameObject storage, GameObject backgrounds)
    {
        //Add Skill Database
        skillDB = new SkillDatabase();
        skillDB.Initialize(this);
        skillDB.LoadData(99);

        //Load in battle background.
        GameObject battleBackground = Instantiate(backgrounds.transform.Find("TiledConcreteWall").gameObject, transform.position, Quaternion.identity, transform);
        battleBackground.name = ("Background");
        battleBackground.SetActive(false);
        
        //Load in Battle UI
        GameObject ui = Instantiate(storage.transform.Find("BattleUIHolder").gameObject, transform.position, Quaternion.identity, transform);
        UIHandler = ui.GetComponent<BattleUIHandler>();
        ui.name = "BattleUIHolder";
        ui.transform.localPosition = new Vector3(0, 0, -9);

        //Load pointer
        pointer = Instantiate(storage.transform.Find("Pointer").gameObject, transform.position, Quaternion.identity, transform);
        pointer.name = "Pointer";
        pointer.SetActive(true);

        //Adds events to skill buttons
        skillButtonEvent += RunSkill;
        runButtonEvent += RunAway;
        defendButtonEvent += ActionEnd;
        switchButtonEvent += ActionEnd;
        foreach (Button x in UIHandler.skillbuttons)
        {
           x.onClick.AddListener(skillButtonEvent);
        }
        UIHandler.runButton.onClick.AddListener(runButtonEvent);
        UIHandler.defendButton.onClick.AddListener(defendButtonEvent);
        UIHandler.switchButton.onClick.AddListener(switchButtonEvent);

        //Fill all slots with dummies
        playerSlots = new BattlerSlot[] { 
            gameObject.AddComponent<BattlerSlot>().Initialize(storage, 1, true), 
            gameObject.AddComponent<BattlerSlot>().Initialize(storage, 2, true), 
            gameObject.AddComponent<BattlerSlot>().Initialize(storage, 3, true), 
            gameObject.AddComponent<BattlerSlot>().Initialize(storage, 4, true) };
        enemySlots = new BattlerSlot[] { 
            gameObject.AddComponent<BattlerSlot>().Initialize(storage, 1, false), 
            gameObject.AddComponent<BattlerSlot>().Initialize(storage, 2, false), 
            gameObject.AddComponent<BattlerSlot>().Initialize(storage, 3, false), 
            gameObject.AddComponent<BattlerSlot>().Initialize(storage, 4, false) };

        //Add OnSelect function to each battlerslot.
        foreach (BattlerSlot x in playerSlots)
        {
            x.OnSelect += Player_OnSelection;
        }
        foreach (BattlerSlot x in enemySlots)
        {
            x.OnSelect += Enemy_OnSelection;
        }
    }


    //Places battlers into the battle instance.
    public bool CreateBattle(GameObject attacker, GameObject target, int distance)
    {
        addNewBattler(attacker, distance);
        addNewBattler(target, 0);


        StartCoroutine(DelayAction.ExecuteWhenTrue(() => { return attacker.GetComponent<CharacterPacket>().inBattle && target.GetComponent<CharacterPacket>().inBattle; }, () => {
            if (attacker.GetComponent<CharacterPacket>().isPlayer)
                TitleSkillButtons(attacker.GetComponent<CharacterPacket>());
            else
                TitleSkillButtons(target.GetComponent<CharacterPacket>());
        }));
        

        //Make background and ui visible
        transform.Find("Background").gameObject.SetActive(true);
        return true;
    }


    //Adds a new battler into the battle instance. Tries to place them in the nearest slot, prioritizing the rear.
    public void addNewBattler(GameObject newBattler, int distance)
    {
        if (newBattler.GetComponent<CharacterPacket>().isPlayer)
        {
            for (int c = 0; c < 4; c++)
            {
                int x = distance;

                try {
                    if (!playerSlots[x + c].isFull)
                    {
                        playerSlots[x + c].addBattler(newBattler);
                        break;
                    }
                }
                catch (System.Exception){}

                try {
                    if (!playerSlots[x - c].isFull)
                    {
                        playerSlots[x - c].addBattler(newBattler);
                        break;
                    }
                }
                catch (System.Exception){}
            }
        }
        else
        {
            for (int c = 0; c < 4; c++)
            {
                int x = distance;

                try {
                    if (!enemySlots[x + c].isFull)
                    {
                        enemySlots[x + c].addBattler(newBattler);
                        break;
                    }
                }
                catch (System.Exception){}

                try {
                    if (!enemySlots[x - c].isFull)
                    {
                        enemySlots[x - c].addBattler(newBattler);
                        break;
                    }
                }
                catch (System.Exception){}
            }
        }
        
        LoadData(newBattler); //Loads skill animations for new battler.
    }

    public bool hasEmptySlot(GameObject battler)
    {
        if (battler.GetComponent<CharacterPacket>().isPlayer)
        {
            foreach (BattlerSlot x in playerSlots)
            {
                if (!x.isFull) { return true; }
            }
        }
        else
        {
            foreach (BattlerSlot x in enemySlots)
            {
                if (!x.isFull) { return true; }
            }
        }
        return false;
    }
    
    public void removeBattler(GameObject battler, int slot) //Removes a battler and replaces them with a dummy slot.
    {
        if (battler.GetComponent<CharacterPacket>().isPlayer)
        {
            playerSlots[slot].removeBattler();
        }
        else
        {
            enemySlots[slot].removeBattler();
        }
    }


    public void StartTurn(CharacterPacket pack) //Starts turn processing
    {
        if (pack.isPlayer)
        {
            print("Your turn");
            currentSlot = playerSlots[pack.slotID - 1]; //Stores which unit is currently acting.
            TitleSkillButtons(currentSlot.pack); //Titles and activates skill buttons.
        }
        else
        {
            print(pack.title + " attacks!");
            currentSlot = enemySlots[pack.slotID - 1]; //Stores which unit is currently acting.
            
            StartCoroutine(DelayAction.ExecuteAfterTime(.5f, () => { RunSkill(Random.Range(0, pack.equippedSkillIDs.Length)); })); //After a short delay, uses a generic attack.
        }

        pointer.transform.parent = currentSlot.battleGraphic.transform;
        pointer.transform.localPosition = new Vector3(0, 0.8f, 0);
        pointer.transform.parent = transform;
    }

    public void StartTurn(CharacterPacket pack, float delay) //Starts turn processing
    {
        if (pack.isPlayer)
        {
            //print("Your turn");
            currentSlot = playerSlots[pack.slotID - 1]; //Stores which unit is currently acting.
            TitleSkillButtons(currentSlot.pack); //Titles and activates skill buttons.
        }
        else
        {
            //print(pack.title + " attacks!");
            currentSlot = enemySlots[pack.slotID - 1]; //Stores which unit is currently acting.

            StartCoroutine(DelayAction.ExecuteAfterTime(.5f, () => { RunSkill(Random.Range(0, pack.equippedSkillIDs.Length)); })); //After a short delay, uses a generic attack.
        }

        pointer.transform.parent = currentSlot.battleGraphic.transform;
        pointer.transform.localPosition = new Vector3(0, 0.8f, 0);
        pointer.transform.parent = transform;
    }

    private void TitleSkillButtons(CharacterPacket pack) //Titles and activates skill buttons
    {
        int num = 0;
        //print("Activating Buttons");
        foreach (int x in pack.equippedSkillIDs)
        {
            UIHandler.skillbuttons[num].GetComponent<Image>().sprite = skillDB.GetIcon(x);
            UIHandler.skillbuttons[num].enabled = true;
            num++;
            if (num >= 4) break;
        }
    }

    private void ActionEnd() //End of turn processing for battle
    {
        //Remove dead battlers
        foreach (BattlerSlot x in playerSlots)
        {
            if (x.isFull && (x.pack.hp <= 0)) x.KillBattler();
        }
        foreach (BattlerSlot x in enemySlots)
        {
            if (x.isFull && (x.pack.hp <= 0))
            {
                GameObject ani = Instantiate(skillDB.PlayAnimation(99)[0], x.battleGraphic.position, Quaternion.identity);
                ani.transform.parent = transform;
                StartCoroutine(DelayAction.ExecuteAfterTime(.2f, () => {
                    x.KillBattler();
                }));
            }
        }

        StartCoroutine(DelayAction.ExecuteAfterTime(skillDB.AniLength(currentSlot.pack.equippedSkillIDs[skillSlot]), () => {
            EndTurn();
        })); //Waits for animation to finish, then ends turn.
    }

    public void EndTurn() //Checks to see if either team is empty
    {
        bool playerExists = false;
        bool enemyExists = false;
        foreach (BattlerSlot x in playerSlots)
        {
            if (x.isFull) { playerExists = true; break; }
        }
        foreach (BattlerSlot x in enemySlots)
        {
            if (x.isFull) { enemyExists = true; break; }
        }

        if (playerExists && enemyExists) //If both teams contain units, ends turn
        {
            print("Process Turn End");
            GetComponentInParent<BattleSceneManager>().ProcessTurnEnd();
        }
        else
        {
            //If either team is empty, removes all battlers and ends battle.
            foreach (BattlerSlot x in playerSlots)
            {
                if (x.isFull) x.removeBattler();
            }
            foreach (BattlerSlot x in enemySlots)
            {
                if (x.isFull) x.removeBattler();
            }

            GetComponentInParent<BattleSceneManager>().EndBattle();
        }
    }

    public void RunAway()
    {
        currentSlot.removeBattler();

        bool playerExists = false;
        bool enemyExists = false;
        foreach (BattlerSlot x in playerSlots)
        {
            if (x.isFull) { playerExists = true; break; }
        }
        foreach (BattlerSlot x in enemySlots)
        {
            if (x.isFull) { enemyExists = true; break; }
        }

        if (playerExists && enemyExists) //If both teams contain units, ends turn
        {
            GetComponentInParent<BattleSceneManager>().RunAway(false);
        }
        else
        {
            //If either team is empty, removes all battlers and ends battle.
            foreach (BattlerSlot x in playerSlots)
            {
                if (x.isFull) x.removeBattler();
            }
            foreach (BattlerSlot x in enemySlots)
            {
                if (x.isFull) x.removeBattler();
            }

            GetComponentInParent<BattleSceneManager>().RunAway(true);
        }
        
    }




    //SKILLS==============================================================================================================================================================
    //SKILLS                                                                                                                                                             =
    //SKILLS==============================================================================================================================================================

    //Skill functions may eventually be moved.
    
    private void RunSkill() //Runs a skill based on skill button press
    {
        foreach (Button x in UIHandler.skillbuttons)
        {
            x.enabled = false; //Disables all buttons
        }
        skillSlot = UIHandler.buttonPressed; //Gets which button was pressed
        skillDB.RunSkill(currentSlot.pack.equippedSkillIDs[skillSlot]); //Runs skill contained in CharacterPacket
    }

    private void RunSkill(int ID) //Runs a skill based on an int. Currently only used for enemies.
    {
        foreach (Button x in UIHandler.skillbuttons)
        {
            x.enabled = false; //Disables all buttons
        }
        skillSlot = ID;
        skillDB.RunSkill(currentSlot.pack.equippedSkillIDs[skillSlot]); //Runs skill contained in CharacterPacket
    }

    private void LoadData(GameObject battler) //Loads animations for each of a character's skill upon them being added to battle.
    {
        foreach (int x in battler.GetComponent<CharacterPacket>().equippedSkillIDs)
        {
            skillDB.LoadData(x);
        }
    }

    private void PlayAnimation(bool isPlayer, int targetPos) //Plays animations centered on BattlerSlot at targetPos
    {
        if (isPlayer)
        {
            foreach (GameObject x in skillDB.PlayAnimation(currentSlot.pack.equippedSkillIDs[skillSlot]))
            {
                GameObject ani = Instantiate(x, enemySlots[targetPos].battleGraphic.position, Quaternion.identity);
                ani.transform.parent = transform; //Sets parent to this so animation is centered on camera
            }
        }
        else
        {
            foreach (GameObject x in skillDB.PlayAnimation(currentSlot.pack.equippedSkillIDs[skillSlot]))
            {
                GameObject ani = Instantiate(x, playerSlots[targetPos].battleGraphic.position, Quaternion.identity);
                ani.transform.parent = transform; //Sets parent to this so animation is centered on camera
            }
        }
    }

    private void DamageCalc(BattlerSlot attacker, BattlerSlot target, int power) //Calculates damage
    {
        target.pack.hp = target.pack.hp - ((attacker.pack.atk * power / 100) - target.pack.def);
    }

    private void UpdateHealthBar(bool isPlayer, int slotID)
    {
        if (isPlayer)
        {
            playerSlots[slotID].healthBar.GetComponent<HealthBar>().UpdateHealthbar(playerSlots[slotID].pack.hp, playerSlots[slotID].pack.mhp);
        }
        else
        {
            enemySlots[slotID].healthBar.GetComponent<HealthBar>().UpdateHealthbar(enemySlots[slotID].pack.hp, enemySlots[slotID].pack.mhp);
        }
    }

    private int findFront(bool isPlayer) //Finds the foremost filled slot
    {
        if (isPlayer)
        {
            for (int i = 0; i < playerSlots.Length; i++)
            {
                if (playerSlots[i].isFull) return i;
            }
        }
        else
        {
            for (int i = 0; i < enemySlots.Length; i++)
            {
                if (enemySlots[i].isFull) return i;
            }
        }

        return -1;
    }

    private int findRear(bool isPlayer) //Finds the rearmost filled slot
    {
        if (isPlayer)
        {
            for (int i = 3; i >= 0; i--)
            {
                if (playerSlots[i].isFull) return i;
            }
        }
        else
        {
            for (int i = 3; i >= 0; i--)
            {
                if (enemySlots[i].isFull) return i;
            }
        }

        return -1;
    }

    private int findRandom(bool isPlayer) //Finds the rearmost filled slot
    {
        if (isPlayer)
        {
            for (int x = 0; x <= 9999; x++)
            {
                int i = Random.Range(0, 4);
                if (playerSlots[i].isFull) return i;
            }
        }
        else
        {
            for (int x = 0; x <= 9999; x++)
            {
                int i = Random.Range(0, 4);
                if (enemySlots[i].isFull) return i;
            }
        }

        return -1;
    }

    //GENERIC ATTACK FUNCTIONS: These are simply functions that will be run by the skills themselves.

    public void AttackFront(bool isPlayer, int power) //Attacks opposing unit in front.
    {
        if (isPlayer)
        {
            int targetPos = findFront(false); //Finds enemy unit in front
            PlayAnimation(isPlayer, targetPos); //Plays animation
            DamageCalc(currentSlot, enemySlots[targetPos], power); //Calculates damage
            UpdateHealthBar(false, targetPos); //Updates healthbar
            ActionEnd(); //End of action processing
        }
        else
        {
            int targetPos = findFront(true);
            PlayAnimation(isPlayer, targetPos);
            DamageCalc(currentSlot, playerSlots[targetPos], power);
            UpdateHealthBar(true, targetPos);
            ActionEnd();
        }
    }

    public void AttackRear(bool isPlayer, int power) //Attacks opposing unit in rear.
    {
        if (isPlayer)
        {
            int targetPos = findRear(false);
            PlayAnimation(isPlayer, targetPos);
            DamageCalc(currentSlot, enemySlots[targetPos], power);
            UpdateHealthBar(false, targetPos);
            ActionEnd();
        }
        else
        {
            int targetPos = findRear(true);
            PlayAnimation(isPlayer, targetPos);
            DamageCalc(currentSlot, playerSlots[targetPos], power);
            UpdateHealthBar(true, targetPos);
            ActionEnd();
        }
    }

    public void AttackSelected(int power) //Attacks selected enemy unit
    {
        int targetPos = selectedEnemyPos;
        if (!enemySlots[targetPos].isFull) targetPos = findFront(false);
        PlayAnimation(true, targetPos);
        DamageCalc(currentSlot, enemySlots[targetPos], power);
        UpdateHealthBar(false, targetPos);
        ActionEnd();
    }

    public void AttackAll(bool isPlayer, int power) //Attacks all opposing units.
    {
        if (isPlayer)
        {
            for (int i = 0; i < enemySlots.Length; i++)
            {
                if (enemySlots[i].isFull)
                {
                    PlayAnimation(isPlayer, i);
                    DamageCalc(currentSlot, enemySlots[i], power);
                    UpdateHealthBar(false, i);
                }
            }
        }
        else
        {
            for (int i = 0; i < playerSlots.Length; i++)
            {
                if (playerSlots[i].isFull)
                {
                    PlayAnimation(isPlayer, i);
                    DamageCalc(currentSlot, playerSlots[i], power);
                    UpdateHealthBar(true, i);
                }
            }
        }
        ActionEnd();
    }

    public void AttackRandom(bool isPlayer, int power) //Attacks opposing unit in front.
    {
        if (isPlayer)
        {
            int targetPos = findRandom(false); //Finds enemy unit in front
            PlayAnimation(isPlayer, targetPos); //Plays animation
            DamageCalc(currentSlot, enemySlots[targetPos], power); //Calculates damage
            UpdateHealthBar(false, targetPos); //Updates healthbar
            ActionEnd(); //End of action processing
        }
        else
        {
            int targetPos = findRandom(true);
            PlayAnimation(isPlayer, targetPos);
            DamageCalc(currentSlot, playerSlots[targetPos], power);
            UpdateHealthBar(true, targetPos);
            ActionEnd();
        }
    }





    //Called from BattlerFunctions. Selects a character after the player clicks on them, then deselects all others.
    private void Player_OnSelection(object sender, System.EventArgs e)
    {
        BattlerSlot y = (BattlerSlot)sender;

        //Unhighlights all slots
        foreach (BattlerSlot x in playerSlots)
        {
            if (x.isFull)
            {
                x.transform.Find(x.pack.title + x.slotID).Find("SelectionCircle").gameObject.SetActive(false);
            }
        }

        //Highlights selected slot
        SelectBattler(y);
        selectedAllyPos = y.pack.slotID - 1;
    }

    //Called from BattlerFunctions. Selects a character after the player clicks on them, then deselects all others.
    private void Enemy_OnSelection(object sender, System.EventArgs e)
    {
        BattlerSlot y = (BattlerSlot)sender;

        //Unhighlights all slots
        foreach (BattlerSlot x in enemySlots)
        {
            if (x.isFull)
            {
                x.transform.Find(x.pack.title + x.slotID).Find("SelectionCircle").gameObject.SetActive(false);
            }
        }

        //Highlights selected slot
        SelectBattler(y);
        selectedEnemyPos = y.pack.slotID - 1;
    }

    //Highlights selected slot and calls appropriate functions
    private void SelectBattler(BattlerSlot battler)
    {
        battler.transform.Find(battler.pack.title + battler.slotID).Find("SelectionCircle").gameObject.SetActive(true);
    }
}
