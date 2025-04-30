using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyTimer
{
    public enum STATE { IDLE,RUN,FINISH };

    public STATE state = STATE.IDLE;
    public float duration;
    private float elapsedTime = 0f;

    public void tick()
    {
        switch (state)
        {
            case STATE.IDLE:
            {

                    break;
            }

            case STATE.RUN:
            {
                elapsedTime += Time.deltaTime;
                if (elapsedTime >= duration) state = STATE.FINISH;
                break;
            }

            case STATE.FINISH:
            {

                    break;
            }

            default:
            {
                Debug.Log("ERROR");
                break;
            }
        }
    }

    public void Go()
    {
        elapsedTime = 0f;
        state = STATE.RUN;
    }
}
