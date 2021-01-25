using System;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class Skill
{
    public string name;
    public string description;
    public int cooldown;

    public string[] skillAnimations;
    public Action executeSkill;
    public string icon;
    public Sprite iconSprite;
    public GameObject[] storedAnimations;
    public float aniLength;
    private int aniNum = 0;

    public int loadState = 0;
    


    public void Initialize(string Name, string Desc, int CD, string Icon, string[] ani, Action XSkill)
    {
        name = Name;
        description = Desc;
        cooldown = CD;
        skillAnimations = ani;
        executeSkill = XSkill;
        icon = Icon;
        storedAnimations = new GameObject[skillAnimations.Length];
        aniLength = 0;
    }

    public void LoadData()
    {
        Addressables.LoadAssetAsync<Sprite>(icon).Completed += onIconLoadComplete;

        if (aniNum < storedAnimations.Length)
        {
            foreach (string x in skillAnimations)
            {
                Addressables.LoadAssetAsync<GameObject>(x).Completed += onAniLoadComplete;
            }
        }
    }

    private void onIconLoadComplete(UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle<Sprite> obj)
    {
        iconSprite = obj.Result;
        loadState++;
    }

    private void onAniLoadComplete(UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle<GameObject> obj)
    {
        if (aniNum < storedAnimations.Length)
        {
            storedAnimations[aniNum] = obj.Result;

            if (obj.Result.GetComponent<Animator>().runtimeAnimatorController.animationClips[0].length >= obj.Result.GetComponent<AudioSource>().clip.length)
            {
                aniLength += obj.Result.GetComponent<Animator>().runtimeAnimatorController.animationClips[0].length + 0.2f;
            }
            else
            {
                aniLength += obj.Result.GetComponent<AudioSource>().clip.length + 0.2f;
            }
        }
        aniNum++;
        loadState++;
    }
}
