using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class MyButton
{
    public bool isPressing = false; //������ѹ��ť
    public bool onPressed = false;  //���°�ť��һ��
    public bool onReleased = false; //�ſ���ť
    public bool isExtending = false; //��չʱ��,����ʵ�ֹ���,��ⰴť���ֺ�����˫��
    public bool isDelaying = false; //long press,�ڰ�ѹ��ť��ȴ�һ��ʱ��,�������ְ�һ�ºͳ�����ѹ

    private bool curState = false;
    private bool lastState = false;

    private float extendDuration = 0.2f;
    private float delayingDuration = 0.2f;

    private MyTimer exTimer = new MyTimer(); //���˫��
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
