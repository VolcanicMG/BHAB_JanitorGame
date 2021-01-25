using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DinoAI : MonoBehaviour
{
    public GameObject[] players;
    private MGEnemyMove unit;

    public void Initialize(MGEnemyMove Unit)
    {
        unit = Unit;
    }

    public void GetPlayers()
    {
        players = GameObject.FindGameObjectsWithTag("TestPlayer");
    }

    public GameObject FindNearestTarget() 
    {

        GameObject nearest = null;
        float distance = Mathf.Infinity;

        foreach (GameObject obj in players)
        {
            float d = Vector3.Distance(transform.position, obj.transform.position);

            if (d < distance)
            {
                distance = d;
                nearest = obj;
            }
        }

        return nearest;
    }

    public void FindAnyAttackableTargets()
    {
        foreach (GameObject obj in players)
        {
            
        }
    }
}
