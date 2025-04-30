using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class MyButton
{
    public bool isPressing = false; //持续按压按钮
    public bool onPressed = false;  //按下按钮的一下
    public bool onReleased = false; //放开按钮
    public bool isExtending = false; //扩展时间,用于实现惯性,检测按钮松手后有无双击
    public bool isDelaying = false; //long press,在按压按钮后等待一段时间,用于区分按一下和持续按压

    private bool curState = false;
    private bool lastState = false;

    private float extendDuration = 0.2f;
    private float delayingDuration = 0.2f;

    private MyTimer exTimer = new MyTimer(); //检测双击
    private MyTimer delayTimer = new MyTimer();
    public void tick(bool input)
    {
        exTimer.tick();
        delayTimer.tick();

        curState = input;

        isPressing = curState;

        onPressed = false;
        onReleased = false;
        isExtending = false;
        isDelaying = false;

        if (curState != lastState && curState)
        {
            onPressed = true;
            StartTimer(delayTimer, delayingDuration);
        }
         
        else if (curState != lastState && !curState)
        {
            onReleased = true;
            StartTimer(exTimer, extendDuration);
        }
        lastState = curState;

        isExtending = (exTimer.state == MyTimer.STATE.RUN) ? true : false;
        isDelaying = (delayTimer.state == MyTimer.STATE.RUN) ? true : false;
    }

    private void StartTimer(MyTimer timer,float duration)
    {
        timer.duration = duration;
        timer.Go();
    }
}
