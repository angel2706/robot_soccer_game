using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class PlayerScript : MonoBehaviour
{
    /*
     * Player defaults
     * 
     * Speed = 300
     * Rotation Sensitivity = 5000
     * Stationary Bonus = 5     * 
     * Cooldown time = 8
     */


    // Rigidbody Component
    private Rigidbody rb;

    // ball
    public GameObject Prefab;
    private GameObject ball;

    // Player Speed
    public float Speed = 300.0f;

    // for movement
    private float movementInput;
    private Vector3 movement;

    // for rotation
    private float turningInput;
    private Vector3 playerRotation;
    public float rotationSensitivity = 5000;
    public float stationaryBonus = 5;
    public bool bonusActive = false;

    // for wheels
    private Transform rightWheel;
    private Transform leftWheel;
    private bool rightBackwards = false;
    private bool leftBackwards = false;

    // for boosts
    private bool boostAvailable = true;
    public float cooldownTime = 8;
    private float timer;

    // Boost UI
    public TMP_Text BoostTitle;
    public TMP_Text Boost;

    private enum BoostState
    {
        Boost,
        Waiting,
        Ready
    }


    void Start()
    {
        // the player root which has the rigidbody
        rb = GetComponent<Rigidbody>();

        Cursor.lockState = CursorLockMode.Locked;
        //Cursor.visible = false;

        // get the wheel objects
        rightWheel = transform.GetChild(0).GetChild(0);
        leftWheel = transform.GetChild(0).GetChild(1);

        // timer to zero
        timer = 0.0f;

        // Boost UI
        BoostAvailable();

    }

    void Update()
    {
        // move on horizontal plane with mouse - no up or downs
        movementInput = Input.GetAxis("Vertical");
        turningInput = Input.GetAxis("Horizontal");

        // handles boosts
        if ((Input.GetKey(KeyCode.LeftShift) || Input.GetMouseButtonDown(1)) && boostAvailable)
        {
            boostPlayer();
        }

        // countsdown, resets if time runs past 0
        if (timer > 0.0f)
        {
            // countdown
            timer -= Time.deltaTime;

            // Updates a boost countdown timer in rough seconds
            Boost.text = timer.ToString("N0") + " s";

            // on time up
            if (timer < 0.0f)
            {
                timer = 0.0f;
                BoostAvailable();
            }
        }

        // handles wheels
        if (Input.GetKey(KeyCode.A))
        {
            // left wheel direction backwards
            leftBackwards = true;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            // right wheel direction backwards
            rightBackwards = true;
        }
        else
        {
            leftBackwards = false;
            rightBackwards = false;
        }       

        // input to reset ball
        if (Input.GetKey(KeyCode.R))
        {
            ResetBall();
        }

        // incase the ball somehow makes it out of the arena
        CheckOutOfBounds();
    }

    void FixedUpdate()
    {
        movePlayer();
        rotatePlayer();
        rotateWheels();
    }

    void movePlayer()
    {
        movement = rb.transform.forward * movementInput; 
        rb.AddForce(movement.normalized * Speed, ForceMode.Force);      
    }

    void rotatePlayer()
    {
        // This isnt working, unsure why but velocity takes forever to settle to zero, despite the sleep threshhold being adjusted
        if (rb.velocity.magnitude == 0)
        {
            bonusActive = true;
            playerRotation = new Vector3(0, turningInput * Time.deltaTime * stationaryBonus, 0);
            rb.AddRelativeTorque(playerRotation * rotationSensitivity, ForceMode.Acceleration);
        }
        // TODO: Check or Fix, bonus activates, but does it work?
        else
        {
            bonusActive = false;
            playerRotation = new Vector3(0, turningInput * Time.deltaTime, 0);
            rb.AddRelativeTorque(playerRotation * rotationSensitivity, ForceMode.Acceleration);
        }
    }

    void rotateWheels()
    {
        // deals with wheels while moving
        if (leftBackwards)
        {
            leftWheel.Rotate(Vector3.up, rb.velocity.magnitude, Space.Self);
        }
        else
        {
            leftWheel.Rotate(Vector3.up, -rb.velocity.magnitude, Space.Self);
        }

        if (rightBackwards)
        {
            rightWheel.Rotate(Vector3.up, rb.velocity.magnitude, Space.Self);
        }
        else
        {
            rightWheel.Rotate(Vector3.up, -rb.velocity.magnitude, Space.Self);
        }

        // TODO: test or fix
        // dont think this is quite working? or just too small to notice
        // ensures wheels turn while not moving but turning
        if (movementInput == 0 && turningInput != 0)
        {
            if (leftBackwards)
            {
                leftWheel.Rotate(Vector3.up, rb.angularVelocity.magnitude *5, Space.Self);
            }
            else
            {
                leftWheel.Rotate(Vector3.up, -rb.angularVelocity.magnitude*5, Space.Self);
            }

            if (rightBackwards)
            {
                rightWheel.Rotate(Vector3.up, rb.angularVelocity.magnitude*5, Space.Self);
            }
            else
            {
                rightWheel.Rotate(Vector3.up, -rb.angularVelocity.magnitude*5, Space.Self);
            }
        }
    }

    void boostPlayer()
    {
        boostAvailable = false;
        BoostTitle.text = BoostState.Waiting.ToString();

        timer = cooldownTime;
        Boost.text = timer.ToString();

        Debug.Log("Pressed right click or shift.");
        rb.AddForce(movement.normalized * Speed * 1.04f, ForceMode.Impulse);
    }

    void FindBall()
    {
        ball = GameObject.FindGameObjectWithTag("Ball");
    }

    void NewBall()
    {
        Destroy(ball);
        Instantiate(Prefab, new Vector3(0, 15, 0), Quaternion.identity);
    }

    void ResetBall()
    {
        FindBall();
        NewBall();
    }

    void BoostAvailable()
    {
        boostAvailable = true;
        BoostTitle.text = BoostState.Boost.ToString();
        Boost.text = BoostState.Ready.ToString();
    }

    void CheckOutOfBounds()
    {
        FindBall();

        if (ball.transform.position.y < 0 || ball.transform.position.y > 20)
        {
            NewBall();
        }
    }
}
