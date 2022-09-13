using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class GoalScript : MonoBehaviour
{
    private GameObject Ball;
    public GameObject Prefab;

    public TMP_Text ScoreTracker;
    public int _score; 

    public int Score
    {
        get { return _score; }
        set 
        {
            // sharing score between goals in the text
            int prev = Convert.ToInt32(ScoreTracker.text);
            // increment
            _score = 1 + prev;            
            // set in ui
            ScoreTracker.text = _score.ToString();
        }
    }
      

    public void OnTriggerEnter(Collider collider)
    {
        // looks for ball
        Ball = GameObject.FindGameObjectWithTag("Ball");

        // then checks collision details
        if (collider.gameObject.tag == "Ball")
        {
            // bye bye ball
            Destroy(collider.gameObject);
            IncreaseScore();
            ResetBall();
        }        
    }

    void ResetBall()
    {
        // create new ball 
        Instantiate(Prefab, new Vector3(0, 15, 0), Quaternion.identity);
    }

    void IncreaseScore()
    {
        Score++;
    }

}
