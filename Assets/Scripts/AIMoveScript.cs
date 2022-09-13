using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIMoveScript : MonoBehaviour
{
    // Dont delete for my sanity
    /*
     * Red Goalie defaults 
     * 
     * xMin = -5.5
     * xMax = 5.5
     * 
     * zMin = 16.5
     * zMax = 19.2
     */

    /*
     * Blue Goalie defaults 
     * 
     * xMin = -5.5
     * xMax = 5.5
     * 
     * zMin = -19.2
     * zMax = -16.5
     */

    /*
     * Shared
     * 
     * Speed = 10
     */

    // ball information
    private GameObject ball;
    private float xPos;
    private float zPos;

    // target position
    public Transform targetPosition;

    // goalie movement limitations ! LEAVE PUBLIC
    public float xMin;
    public float xMax;

    public float zMin;
    public float zMax;

    // goalie info
    private Rigidbody rb;
    public float speed = 10.0f;

    // for wheels
    private Transform rightWheel;
    private Transform leftWheel;
    private bool rightBackwards = false;
    private bool leftBackwards = false;

    // step info
    private float step = 0;

    // check cooldowns
    private bool checkBall = true;
    private float cooldownTime = 0.75f;
    private float timer;


    // Start is called before the first frame update
    void Start()
    {
        // get player rigidbody
        rb = GetComponent<Rigidbody>();

        // set with a tiny delay
        timer = cooldownTime;

        // get the wheel objects - could use tags instead?
        rightWheel = transform.GetChild(0).GetChild(0);
        leftWheel = transform.GetChild(0).GetChild(1);
    }

    // Update is called once per frame
    void Update()
    {
        // detect ball - check delays?
        if (timer > 0.0f)
        {
            //countdown
            timer -= Time.deltaTime;

            if (timer < 0.0f)
            {
                timer = 0.0f;
                checkBall = true;
            }
        }

        // irregularly checks the ball - to make it easier for the player
        if (checkBall)
        {
            // ensures wheels stop rotating
            step = 0;

            detectBall();

            // look at new position
            rb.transform.LookAt(targetPosition);            

            checkBall = false;
        }

        // block from source: https://docs.unity3d.com/ScriptReference/Vector3.MoveTowards.html
        if (!(Vector3.Distance(transform.position, targetPosition.position) < 0.001f))
        {
            step = speed * Time.deltaTime;
        }
        transform.position = Vector3.MoveTowards(transform.position, targetPosition.position, step);

    }

    void FixedUpdate()
    {
        // handles wheel movement - refactor if theres time
        rotateWheels();
    }

    void detectBall()
    {
        timer = cooldownTime;
        ball = GameObject.FindGameObjectWithTag("Ball");

        // get current x and z pos of ball
        // clamp x and z positions to defined limits
        zPos = Mathf.Clamp(ball.transform.position.z, zMin, zMax);
        xPos = Mathf.Clamp(ball.transform.position.x, xMin, xMax);

        //creates new target position for AI
        targetPosition.position = new Vector3(xPos, 1, zPos);

    }

    void rotateWheels()
    {
        // deals with wheels while moving
        // increased by a multiplier since AI speed is quite slow - results are a tad janky
        if (leftBackwards)
        {
            leftWheel.Rotate(Vector3.up, step * 100, Space.Self);
        }
        else
        {
            leftWheel.Rotate(Vector3.up, -step * 100, Space.Self);
        }

        if (rightBackwards)
        {
            rightWheel.Rotate(Vector3.up, step * 100, Space.Self);
        }
        else
        {
            rightWheel.Rotate(Vector3.up, -step * 100, Space.Self);
        }
    }

}
