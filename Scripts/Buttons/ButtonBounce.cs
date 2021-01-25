using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonBounce : MonoBehaviour
{

    public float amplitude = 0.5f;
    public float frequency = 1f;

    Vector2 Pos;

    private void Start()
    {
        Pos.y = transform.position.y;
        Pos.x = transform.position.x;
    }

    void Update()
    {
        // Float up/down with a Sin()
        Pos.y += (Mathf.Sin(Time.fixedTime * Mathf.PI * frequency) * amplitude);
        //Pos.x = 24.965f;

        transform.position = Pos;
    }
}
