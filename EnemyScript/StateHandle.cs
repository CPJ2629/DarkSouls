using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Drawing;
using System.Net;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class IdleState :IState
{
    private EnemyFSM fsm;
    private Parameter parameter;
    private float timer = 0f;

    public IdleState(EnemyFSM fsm)
    {
        this.fsm = fsm;
        this.parameter = fsm.parameter;
    }
    public void OnEnter()
    {
        parameter.animator.Play("idle");
    }

    public void OnUpdate()
    {
        timer += Time.deltaTime;

        if (parameter.getHit) fsm.TransformState(State.Hit);

        if (parameter.target != null)
        {
            fsm.TransformState(State.React);
        }


        if (timer >= parameter.idletTime)
        {
            fsm.TransformState(State.Patrol);
        }
    }

    public void OnExit()
    {
        timer = 0;
    }
}

public class PatrolState : IState
{
    private EnemyFSM fsm;
    private Parameter parameter;
    private int patrolPosition=0;

    public PatrolState(EnemyFSM fsm)
    {
        this.fsm = fsm;
        this.parameter = fsm.parameter;
    }
    public void OnEnter()
    {
        parameter.animator.Play("walk");
    }

    public void OnUpdate()
    {
        if (parameter.getHit) fsm.TransformState(State.Hit);

        fsm.FlipTo(parameter.patrolPoints[patrolPosition]);

        parameter.EnemyHandle.position = Vector3.MoveTowards(parameter.EnemyHandle.position, parameter.patrolPoints[patrolPosition].position, 
            parameter.moveSpeed * Time.deltaTime);


        if (parameter.target != null)
        {
            fsm.TransformState(State.React);
        }

        if (Vector3.Distance(parameter.EnemyHandle.position, parameter.patrolPoints[patrolPosition].position) < 2.2f) fsm.TransformState(State.Idle);
    }

    public void OnExit()
    {
        patrolPosition++;

        if(patrolPosition>=parameter.patrolPoints.Length) patrolPosition = 0;
    }
}

public class ChaseState : IState
{
    private EnemyFSM fsm;
    private Parameter parameter;
    private AnimatorStateInfo info;
    public ChaseState(EnemyFSM fsm)
    {
        this.fsm = fsm;
        this.parameter = fsm.parameter;
    }
    public void OnEnter()
    {
        parameter.animator.Play("run");
    }

    public void OnUpdate()
    {

        if (parameter.getHit) fsm.TransformState(State.Hit);

        fsm.FlipTo(parameter.target);

        info = parameter.animator.GetCurrentAnimatorStateInfo(0);

        if (parameter.target)
        {
            parameter.EnemyHandle.position = Vector3.MoveTowards(parameter.EnemyHandle.position, parameter.target.position,
                parameter.chaseSpeed * Time.deltaTime);
        }

        if (parameter.target == null) //¶ªÊ§Íæ¼Ò
        {
            fsm.TransformState(State.Idle);
        }

        if (Physics.OverlapSphere(parameter.attackPoint.position, parameter.attackArea, parameter.targetLayer)[0]!=null) //¹¥»÷·¶Î§ÅÐ¶¨
        {
            fsm.TransformState(State.Attack);
        }
    }

    public void OnExit()
    {

    }

    /*public IEnumerator FollowPath()
    {
        print("inFollowPath");
        parameter.isMoving = true;
        foreach (Vector3 point in Instance.path)
        {
            print(point);
            while (Vector3.Distance(parameter.EnemyHandle.transform.position, point) > 0.1f)
            {
                parameter.EnemyHandle.transform.position = Vector3.MoveTowards(parameter.EnemyHandle.transform.position, 
                    point, parameter.chaseSpeed * Time.deltaTime);
                yield return null;
            }
        }
        parameter.isMoving = false;
    }*/


}

public class ReactState : IState
{
    private EnemyFSM fsm;
    private Parameter parameter;
    private AnimatorStateInfo info;

    public ReactState(EnemyFSM fsm)
    {
        this.fsm = fsm;
        this.parameter = fsm.parameter;
    }
    public void OnEnter()
    {
        fsm.FlipTo(parameter.target);
        parameter.animator.Play("react");
    }

    public void OnUpdate()
    {
        if (parameter.getHit) fsm.TransformState(State.Hit);

        info = parameter.animator.GetCurrentAnimatorStateInfo(0);

        if (info.normalizedTime >= 0.95f) fsm.TransformState(State.Chase);
    }

    public void OnExit()
    {

    }
}

public class AttackState : IState
{
    private EnemyFSM fsm;
    private Parameter parameter;
    private AnimatorStateInfo info;
    private float timer;

    public AttackState(EnemyFSM fsm)
    {
        this.fsm = fsm;
        this.parameter = fsm.parameter;
    }
    public void OnEnter()
    {
        int randomIndex = Random.Range(0, 30);
        if (randomIndex <= 10) parameter.animator.Play("attack1");
        else if (randomIndex > 10 && randomIndex <= 20) parameter.animator.Play("attack2");
        else if (randomIndex > 20 && randomIndex <= 30) parameter.animator.Play("attack3");
        timer = 0f;
    }

    public void OnUpdate()
    {
        if (parameter.getHit) fsm.TransformState(State.Hit);
        if (parameter.isCounterBack) fsm.TransformState(State.Stun);

        info = parameter.animator.GetCurrentAnimatorStateInfo(0);

        if (info.normalizedTime >= 0.95f)
        {
            if (parameter.target == null) //¹¥»÷·¶Î§ÅÐ¶¨
            {
                parameter.target = GameObject.FindWithTag("Player").transform;
                fsm.TransformState(State.Chase);
            }
            else 
            {
                fsm.TransformState(State.Defense);
            }

        }

    }

    public void OnExit()
    {

    }
}

public class DefenseState : IState
{
    private EnemyFSM fsm;
    private Parameter parameter;
    private AnimatorStateInfo info;

    public DefenseState(EnemyFSM fsm)
    {
        this.fsm = fsm;
        this.parameter = fsm.parameter;
    }
    public void OnEnter()
    {
        parameter.animator.Play("defense");
    }

    public void OnUpdate()
    {
        Vector3 counterDir = parameter.model.transform.position - parameter.player.transform.position;
        float counterAngle1 = Vector3.Angle(parameter.player.transform.forward, counterDir);
        float counterAngle2 = Vector3.Angle(parameter.player.transform.forward, parameter.model.transform.forward);
        bool counterVaild = (counterAngle1 < 30 && Mathf.Abs(counterAngle2 - 180) < 30);
        if(!counterVaild && parameter.getHit) fsm.TransformState(State.Hit);

        info = parameter.animator.GetCurrentAnimatorStateInfo(0);
        if (info.normalizedTime >= 0.95f) fsm.TransformState(State.Defensing);
    }

    public void OnExit()
    {
        
    }
}

public class DefenseWalkState : IState
{
    private EnemyFSM fsm;
    private Parameter parameter;
    private AnimatorStateInfo info;

    public DefenseWalkState(EnemyFSM fsm)
    {
        this.fsm = fsm;
        this.parameter = fsm.parameter;
    }
    public void OnEnter()
    {
        fsm.FlipTo(parameter.target);
        parameter.animator.Play("defensingwalk");
    }

    public void OnUpdate()
    {
        fsm.FlipTo(parameter.target);
        Vector3 direction = (parameter.EnemyHandle.transform.forward - parameter.EnemyHandle.transform.right).normalized;
        parameter.EnemyHandle.transform.Translate(direction * 2f * Time.deltaTime, Space.World);

        info = parameter.animator.GetCurrentAnimatorStateInfo(0);
        if (info.normalizedTime >= 0.95f) fsm.TransformState(State.Chase);
    }

    public void OnExit()
    {

    }
}

public class StunState : IState
{
    private EnemyFSM fsm;
    private Parameter parameter;
    private AnimatorStateInfo info;

    public StunState(EnemyFSM fsm)
    {
        this.fsm = fsm;
        this.parameter = fsm.parameter;
    }
    public void OnEnter()
    {
        parameter.animator.Play("hit");
        parameter.caster1.SetActive(true);
    }

    public void OnUpdate()
    {
        info = parameter.animator.GetCurrentAnimatorStateInfo(0);
        if (info.normalizedTime >= 0.95f) fsm.TransformState(State.Chase);
    }

    public void OnExit()
    {
        parameter.caster1.SetActive(false);
        GameManager.Instance.UpdateInteration(false);
    }
}

public class HitState : IState
{
    private EnemyFSM fsm;
    private Parameter parameter;
    private AnimatorStateInfo info;

    public HitState(EnemyFSM fsm)
    {
        this.fsm = fsm;
        this.parameter = fsm.parameter;
    }
    public void OnEnter()
    {
        parameter.animator.Play("hit");
    }

    public void OnUpdate()
    {
        info = parameter.animator.GetCurrentAnimatorStateInfo(0);

        if (info.normalizedTime >= 0.95f)
        {
            if (parameter.HP <= 0f) fsm.TransformState(State.Dead);
            else
            {
                parameter.target = GameObject.FindWithTag("Player").transform;
                fsm.TransformState(State.Chase);
            }
        }
    }

    public void OnExit()
    {
        parameter.getHit = false;
    }

}

public class DeadState : IState
{
    private EnemyFSM fsm;
    private Parameter parameter;
    private float timer = 0f;

    public DeadState(EnemyFSM fsm)
    {
        this.fsm = fsm;
        this.parameter = fsm.parameter;
    }
    public void OnEnter()
    {
        parameter.animator.Play("dead");
    }

    public void OnUpdate()
    {
        
    }

    public void OnExit()
    {

    }
}

