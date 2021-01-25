using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;


public class AnimationAutoDestroy : MonoBehaviour
{
    public float delay = 0f;
    
    void Start()
    {
        if (GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).length >= GetComponent<AudioSource>().clip.length)
        {
            Destroy(gameObject, GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).length);
        }
        else
        {
            Destroy(GetComponent<SpriteRenderer>(), GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).length);
            Destroy(gameObject, GetComponent<AudioSource>().clip.length + delay);
        }
    }
}