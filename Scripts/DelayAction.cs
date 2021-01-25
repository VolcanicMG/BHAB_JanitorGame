using System;
using System.Collections;
using UnityEngine;

public class DelayAction : MonoBehaviour
{
    private static bool loadFinished = true;
    private static Func<bool> loadingAction;

    private void Update()
    {
        if (!loadFinished)
        {
            if (loadingAction())
            {
                loadFinished = true;
            }
        }
    }

    public static IEnumerator ExecuteAfterTime(float time, Action executeAction)
    {
        yield return new WaitForSeconds(time);

        //print("Finished " + executeAction.ToString() + " in " + time);
        executeAction();
    }

    public static IEnumerator ExecuteWhenTrue(Func<bool> LoadingAction, Action executeAction)
    {
        loadFinished = false;
        loadingAction = LoadingAction;

        yield return new WaitUntil(() => loadFinished);

        //print("Loaded " + executeAction.ToString());
        executeAction();
    }
}
