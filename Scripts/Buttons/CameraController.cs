using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GameObject followTarget;
    private Vector3 targetPos;
    private Vector3 lastPos;
    public float moveSpeed;
    public bool isMoving;
    public bool isControlled;
    public bool isControllable;
    private bool isFollowing;
    

    private float headingX;
    private float headingY;

    // Start is called before the first frame update
    void Start()
    {
        //isFollowing = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isControllable || isFollowing || !isControlled || GetComponentInChildren<BattleSceneManager>().battleSceneActive)
        {
            float RoundX = Mathf.Round(followTarget.transform.position.x * 512);
            float RoundY = Mathf.Round(followTarget.transform.position.y * 512);

            targetPos = new Vector3(RoundX * (1 / 512f), RoundY * (1 / 512f), transform.position.z);
            transform.position = Vector3.Lerp(transform.position, targetPos, moveSpeed * Time.deltaTime);

            if (Vector2.Distance(transform.position, targetPos) > .5f
                || Vector2.Distance(transform.position, targetPos) < -.5f)
            {
                isMoving = true;
            }
            else
            {
                isMoving = false;
                isFollowing = false;
                if (Input.GetKey("w") || Input.GetKey("s") || Input.GetKey("a") || Input.GetKey("d"))
                    isControlled = true;
            }
        }
        else {

            if (isControlled)
            {
                if (Input.GetKey("w")) headingY = 1;
                else if (Input.GetKey("s")) headingY = -1;
                else headingY = 0;

                if (Input.GetKey("d")) headingX = 1;
                else if (Input.GetKey("a")) headingX = -1;
                else headingX = 0;

                targetPos = new Vector3(transform.position.x + headingX, transform.position.y + headingY, transform.position.z);
                transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
            }
        }
        
    }

    public void FollowTarget(GameObject newTarget)
    {
        followTarget = newTarget;
        isFollowing = true;
        isControlled = false;
    }

    public void SetControllable(bool x)
    {
        if (x)
        {
            isControllable = true;
        }
        else
        {
            isControllable = false;
            isControlled = false;
        }
    }

}
