using UnityEngine;
using UnityEngine.AddressableAssets;

public class SkillAnimationManager : MonoBehaviour
{
    Vector3 playPos;

    public void playSlash(Vector3 pos)
    {
        Addressables.LoadAssetAsync<GameObject>("SlashAni").Completed += SlashLoadDone;
        playPos = pos;
    }

    private void SlashLoadDone(UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle<GameObject> obj)
    {
        GameObject ani = Instantiate(obj.Result, playPos, Quaternion.identity);
    }
}
