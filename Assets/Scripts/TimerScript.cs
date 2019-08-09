using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimerScript : MonoBehaviour
{
    private float maxTime;
    private float timeLeft;
    public float getTimeLeft()
    {
        return timeLeft;
    }
    private bool countingDown;

    public void restartTimer()
    {
        timeLeft = maxTime;
        countingDown = true;
    }

    public void stopTimer()
    {
        countingDown = false;
    }

    // Start is called before the first frame update
    void Start()
    {
        //timeLeft = battleManagerScript.TURN_TIME;
    }

    public void Setup(float TURN_TIME)
    {
        maxTime = TURN_TIME;
    }

    // Update is called once per frame
    void Update()
    {
        if (countingDown)
        {
            if (timeLeft > 0.0f)
            {
                timeLeft -= Time.deltaTime;
                int secondsLeft = (int)timeLeft;
                GetComponent<Text>().text = "Time left: " + secondsLeft.ToString();

            }

            else
            {
                timeLeft = 0.0f;
                countingDown = false;
            }
        }
    }
}
