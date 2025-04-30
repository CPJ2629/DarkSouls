using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//为ActorManager提供数据

public class StateManager : IActorManagerInterface
{
    
    public float HP = 1000f;
    public float HPMAX = 1000f;
    public float ATK = 10.0f;

    public bool isGround;
    public bool isJump;
    public bool isFall;
    public bool isRoll;
    public bool isStepBack;
    public bool isAttack;
    public bool isHit;
    public bool isDie;
    public bool isBlock;
    public bool isDefense;
    public bool isCounterBack;
    public bool isCounterBackEnable;

    public bool isAllowDefense;
    public bool isImmortal;
    public bool isCounterBackSuccess;
    public bool isCounterBackFailure;


    private void Start()
    {
        HP = HPMAX;
        am = GetComponentInParent<ActorManager>();
    }

    void Update()
    {
        isGround = am.ac.CheckState("ground");
        isJump = am.ac.CheckState("jump");
        isFall = am.ac.CheckState("fall");
        isRoll = am.ac.CheckState("roll");
        isStepBack = am.ac.CheckState("stepback");
        isAttack = am.ac.CheckStateTag("attackR") || am.ac.CheckStateTag("attackL");
        isHit = am.ac.CheckState("hit");
        isDie = am.ac.CheckState("die");
        isBlock = am.ac.CheckState("block");
        isCounterBack = am.ac.CheckState("counterBack"); //此处分为盾反成功和失败

        isAllowDefense = isGround || isBlock;
        isDefense = isAllowDefense && (am.ac.CheckState("defenseL", "defense"));
        isCounterBackSuccess = isCounterBackEnable; //通过动画播放进度来判断是否盾反成功
        isCounterBackFailure = isCounterBack && !isCounterBackEnable; //盾反有效时间外则为失败

        isImmortal = isRoll || isStepBack; //翻滚后撤无敌帧
    }

    public void ChangeHP(float val)
    {
        HP += val;
        HP = Mathf.Clamp(HP, 0, HPMAX);
    }

}
