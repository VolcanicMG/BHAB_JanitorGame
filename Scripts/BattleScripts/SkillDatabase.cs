using UnityEngine;

public class SkillDatabase
{
    private Skill[] skillArray;

    public void Initialize(BattleScene BS)
    {
        int skillNum = 100;

        skillArray = new Skill[skillNum];

        for (int i = 0; i < skillNum; i++)
        {
            skillArray[i] = new Skill();
        }
        /*  EXAMPLE SKILL:
         *  skillArray[Skill ID].Initialize( //Each character stores an int which corresponds with the skill id.
         *  Skill Name: "Skill Name",
         *  Skill Description: "Skill Description",
         *  Cooldown: 0,
         *  Animations: "Loads animations designated for the skill through addressable assets",
         *  () => { //Calls a function from BattleScene
         *  BS.CallSkillFunction(
         *  isPlayer, //True for player skill, false for enemy skill
         *  Skill Power: 100 //Skill Damage multiplier
         *  );});
         */

        skillArray[0/*Skill ID*/].Initialize("Greased Lightning"/*Name*/, ""/*Desc*/, 0/*CD*/, "IconMop"/*icon*/, new string[]{"SlashAni"}/*Animation*/,
            () => { BS.AttackFront(true/*isPlayer*/, 200/*Power*/);/*Function*/ });

        skillArray[1].Initialize("Cleanhazzard", "", 0, "IconFireball", new string[] { "SlashHitEffect" },
            () => { BS.AttackRear(true, 200); });

        skillArray[2].Initialize("Wax On, Wax Off", "", 0, "IconTarget", new string[] { "JabHitEffect" },
            () => { BS.AttackSelected(175); });

        skillArray[3].Initialize("Whirling Mop", "", 0, "IconJab", new string[] { "SlashAni" },
            () => { BS.AttackAll(true, 150); });

        skillArray[4].Initialize("Dino Gun", "", 0, "IconTarget", new string[] { "JabHitEffect" },
            () => { BS.AttackRandom(false, 150); });

        skillArray[5].Initialize("Dino Chomp", "", 0, "IconFireball", new string[] { "BiteEffect" },
            () => { BS.AttackFront(false, 200); });

        skillArray[6].Initialize("Dino Crisis", "", 0, "IconTarget", new string[] { "JabHitEffect" },
            () => { BS.AttackAll(false, 125); });

        skillArray[7].Initialize("Bleach Fire", "", 0, "IconBleachGun", new string[] { "DoubleSlashAni" },
            () => { BS.AttackSelected(200); });

        skillArray[8].Initialize("Road Roller", "", 0, "IconBuffer", new string[] { "BufferHitEffect" },
            () => { BS.AttackFront(true, 250); });

        skillArray[9].Initialize("Extendo Arm", "", 0, "IconExtendo", new string[] { "ExtendoHitEffect" },
            () => { BS.AttackAll(true, 175); });






        skillArray[99].Initialize("Explosion", "", 0, "IconFireball", new string[] { "Explosion" },
            () => { BS.AttackFront(true, 0); });

        //TEMP
        foreach (Skill s in skillArray)
        {
            if (s.name != null)
            {
                s.LoadData();
            }
        }
    }

    public void RunSkill(int skillID)
    {
        skillArray[skillID].executeSkill();
    }

    public string GetSkillName(int skillID)
    {
        return skillArray[skillID].name;
    }

    public void LoadData(int skillID)
    {
        skillArray[skillID].LoadData();
    }

    public GameObject[] PlayAnimation(int skillID)
    {
        return skillArray[skillID].storedAnimations;
    }

    public Sprite GetIcon(int skillID)
    {
        return skillArray[skillID].iconSprite;
    }

    public float AniLength(int skillID)
    {
        return skillArray[skillID].aniLength;
    }

    public bool LoadDone(int[] skillIDs)
    {
        int cntr = 0;
        foreach (int i in skillIDs)
        {
            if (skillArray[i].loadState >= 2)
                cntr++;
        }

        if (cntr == skillIDs.Length)
            return true;
        else
            return false;
    }
    
}
