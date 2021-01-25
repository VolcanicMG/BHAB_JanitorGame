using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class _Vector2
{
    //this_ is the stationary position while to is the variable position
    public static float AngleTo(this Vector3 this_, Vector3 to) //this_ is flashlight position - to is mouse position
    {
        //find direction by subtracing world posisition
        Vector2 direction = to - this_;
        //call on a math library to get radian and convert radian to degree
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        //add 360 to a negative value
        if (angle < 0f) angle += 360f;
        //return the ultimate angle
        return angle;
    }


}

public class FlashlightMovement : MonoBehaviour
{
    //Globals*----------------------*

    //This is a "placeholder", we can use this later for .range and .spotlight angle
    private UnityEngine.Experimental.Rendering.Universal.Light2D light;

    //Flash light radius - For later use
    private float FlashlightOuterRadius;

    private float FlashlightInnerRadius;

    //Get the outer angle for the flashlight.
    private float FlashlightOuterAngle;

    private float FlashlightInnerAngle;

    private float FlashlightIntensity = 0.8f;

    //Stops tracking mouse when its not their turn.
    public bool followMouse = false;

    //diffrent lens
    [Header("**IMPORTANT** Can only select one at a time.")]
    public bool normalLens;
    public bool focusedLen;
    public bool lantern;


    //Set the position of the object
    [Tooltip("Center of the angle(Zero starts here)")]
    public Transform Center;

    //Using the main camera for the world to screen point
    public Camera MainCam;

    private Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        //used to the the light compenent
        light = GetComponent<UnityEngine.Experimental.Rendering.Universal.Light2D>();
        animator = transform.parent.GetComponentInChildren<Animator>();

    }

    // Update is called once per frame
    void Update()
    {
        if (followMouse)
        {
            //We use this later for battery life and to adjust the intensity.
            light.pointLightOuterRadius = FlashlightOuterRadius;
            light.pointLightOuterAngle = FlashlightOuterAngle;
            light.intensity = FlashlightIntensity;
            light.pointLightInnerAngle = FlashlightInnerAngle;
            light.pointLightInnerRadius = FlashlightInnerRadius;

            //Get the mouse location and the direction(Might not need to get direction if its going to be in an area)
            Vector2 mousePosition = Input.mousePosition;

            //Get the players position related to the camera and screen point
            Vector3 playerPosition = MainCam.WorldToScreenPoint(Center.transform.position);

            //info is sent the _Vector2 class to calculate the mouse position relative to the players position in order to get the angle.
            float angle = playerPosition.AngleTo(mousePosition);


            //different Debugs to figure out the angle and mouse position
            //Debug.Log(angle.ToString());
            //Debug.Log(Input.mousePosition);
            //Debug.Log(mousePosition.ToString());

            //Right
            if (angle <= 45 || angle > 315)
            {
                if (animator.GetBool("faceLeft")) animator.SetBool("faceLeft", false);
                transform.rotation = Quaternion.Euler(0, 0, 0);
                transform.Rotate(0, 0, 270);
                //Debug.Log("Right");
            }
            //Down
            else if (angle <= 135)
            {
                transform.rotation = Quaternion.Euler(0, 0, 0);
                transform.Rotate(0, 0, 0);
                //Debug.Log("up");
            }
            //Left
            else if (angle <= 225)
            {
                if (!animator.GetBool("faceLeft")) animator.SetBool("faceLeft", true);
                transform.rotation = Quaternion.Euler(0, 0, 0);
                transform.Rotate(0, 0, 90);
                //Debug.Log("left");
            }
            //Up
            else if (angle <= 315)
            {
                transform.rotation = Quaternion.Euler(0, 0, 0);
                transform.Rotate(0, 0, 180);
                //Debug.Log("down");
            }

            //normal flashlight
            if (normalLens)
            {
                FlashlightOuterRadius = 4.3f;
                FlashlightOuterAngle = 84.0f;
                FlashlightInnerAngle = 34.33f;
                FlashlightInnerRadius = 1.45f;
                //Debug.Log(FlashlightOuterRadius + " " + FlashlightOuterAngle);

            }

            //focused lens
            if (focusedLen)
            {
                FlashlightOuterRadius = 6.75f;
                FlashlightOuterAngle = 34.9f;
                FlashlightInnerAngle = 16.3f;
                FlashlightInnerRadius = 1.0f;
                //Debug.Log(FlashlightOuterRadius + " " + FlashlightOuterAngle);

            }

            //lantern
            if (lantern)
            {
                FlashlightOuterRadius = 2.15f;
                FlashlightOuterAngle = 1300.0f;
                FlashlightInnerAngle = 0.0f;
                FlashlightInnerRadius = 1.1f;
                //Debug.Log(FlashlightOuterRadius + " " + FlashlightOuterAngle);

            }
        }

        ////Move the light right - Input used for later use if needed
        //if (Input.GetAxisRaw("Horizontal") > 0.5f)
        //{
        //    transform.rotation = Quaternion.Euler(0, 0, 0);
        //    transform.Rotate(0, 0, -90);
        //}

        ////move the light left
        //if(Input.GetAxisRaw("Horizontal") < -0.5f)
        //{
        //    transform.rotation = Quaternion.Euler(0, 0, 0);
        //    transform.Rotate(0, 0, 90);
        //}

        ////move to the top
        //if (Input.GetAxisRaw("Vertical") > 0.5f )
        //{
        //    transform.rotation = Quaternion.Euler(0, 0, 0);
        //    transform.Rotate(0, 0, 0);
        //}

        ////move to the bottom
        //if(Input.GetAxisRaw("Vertical") < -0.5f)
        //{
        //    transform.rotation = Quaternion.Euler(0, 0, 0);
        //    transform.Rotate(0, 0, 180);
        //}
    }
}
