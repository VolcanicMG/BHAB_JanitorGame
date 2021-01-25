using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTriggerBox : MonoBehaviour
{
    public MGEnemyMove[] triggerEnemies;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        foreach (MGEnemyMove x in triggerEnemies)
        {
            x.enabled = true;
        }
        Destroy(gameObject);
    }
}
