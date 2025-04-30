using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using System;


public enum State {Idle,Patrol,React,Chase,Attack,Hit,Dead,Defense,Defensing,Stun }

[Serializable]
public class Parameter 
{
    public GameObject model;
    public GameObject player;
    public GameObject caster1;

    public float HP;
    public float moveSpeed;
    public float chaseSpeed;
    public float idletTime;

    public Transform[] patrolPoints;
    public Transform attackPoint;
    public Transform target;
    public Transform EnemyHandle;

    public LayerMask targetLayer;

    public float attackArea;

    public Animator animator;

    public bool getHit = false;
    public bool isCounterBack = false;
}


public class EnemyFSM : MonoBehaviour
{
    public IState currentState;
    public Dictionary<State,IState> states = new Dictionary<State,IState>();
    public Parameter parameter;
    public SphereCollider col;
    void Start()
    {
        states.Add(State.Idle, new IdleState(this));
        states.Add(State.Patrol, new PatrolState(this));
        states.Add(State.Chase, new ChaseState(this));
        states.Add(State.React, new ReactState(this));
        states.Add(State.Attack, new AttackState(this));
        states.Add(State.Hit, new HitState(this));
        states.Add(State.Dead, new DeadState(this));
        states.Add(State.Defense, new DefenseState(this));
        states.Add(State.Defensing, new DefenseWalkState(this));
        states.Add(State.Stun, new StunState(this));


        parameter.attackPoint = parameter.model.transform;
        parameter.targetLayer = LayerMask.GetMask("Player");
        currentState = states[State.Idle];
        col = GetComponent<SphereCollider>();
    }

    void Update()
    {
        currentState.OnUpdate();
        if (parameter.HP <= 0f) TransformState(State.Dead);
    }

    public void TransformState(State state)
    {
        if (currentState != null) currentState.OnExit();
        currentState = states[state];
        currentState.OnEnter();
    }

    public void FlipTo(Transform target)
    {
        if (target != null)
        {
            parameter.EnemyHandle.LookAt(target);
        }
    }

    private void OnTriggerEnter(Collider col)
    {
        if (col.CompareTag("Player"))
        {
            parameter.target = col.transform;
        }
    }

    private void OnTriggerExit(Collider col)
    {
        if (col.CompareTag("Player")){
            parameter.target = null;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(parameter.attackPoint.position, parameter.attackArea);
    }

    public void ChangeHP(WeaponController targetWC,bool attackValid)
    {
        if (!attackValid) return;
        parameter.HP -= targetWC.GetATK();
    }

    public void ChangeHP(int val)
    {
        parameter.HP -= val;
    }
}
