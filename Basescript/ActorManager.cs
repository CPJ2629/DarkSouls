using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class ActorManager : MonoBehaviour
{
    public ActorController ac;
    public BattleManager bm;
    public WeaponManager wm;
    public StateManager sm;
    public DirectorManager dm;
    public InteractionManager im;

    void Awake()
    {
        ac = GetComponent<ActorController>();
        GameObject sensor = transform.Find("sensor").gameObject; //寻找子控件
        GameObject model = ac.model;
        bm = Bind<BattleManager>(sensor);
        wm = Bind<WeaponManager>(model);
        sm = Bind<StateManager>(gameObject);
        dm = Bind<DirectorManager>(gameObject);
        im = Bind<InteractionManager>(sensor);
        ac.onAction += DoAction;
    }


    void Update()
    {
        Vector3 c = im.overlapEcastm[0].am.transform.position - this.transform.position;
        Vector3 a = this.ac.model.transform.forward;
        float angle = Vector3.Angle(a, c);
        if (angle <= 30 && im.isCol) im.interaction = true;
        if (Input.GetKeyDown(KeyCode.H) && im.isCol) ac.OnAction();
    }

    public void DoAction()
    {
        Vector3 c = im.overlapEcastm[0].am.transform.position - this.transform.position;
        Vector3 a = this.ac.model.transform.forward;
        float angle = Vector3.Angle(a,c);
        if (angle > 30) return;

        if (im.overlapEcastm.Count != 0)
        {
            if (im.overlapEcastm[0].eventName == "frontStab" && im.overlapEcastm[0].active)
            {
                dm.PlayFrontStab("Stab", this, im.overlapEcastm[0].am);
            }

            else if (im.overlapEcastm[0].eventName == "openBox" && im.overlapEcastm[0].active)
            {
                dm.PlayOpenBox("openBox", this, im.overlapEcastm[0].am);
                im.overlapEcastm[0].active = false;
            }

            else if (im.overlapEcastm[0].eventName == "leverUp" && im.overlapEcastm[0].active)
            {
                dm.PlayLeverUp("leverUp", this, im.overlapEcastm[0].am);
                im.overlapEcastm[0].active = false;
            }


        }
    }

    private T Bind<T>(GameObject go) where T: IActorManagerInterface //泛型方法,用于函数传入一个类型的类
    {
        T tempClass = go.GetComponent<T>();
        if(tempClass == null) tempClass = go.AddComponent<T>();
        tempClass.am = this;
        return tempClass;
    }

    public void SetIsCounterBack(bool val)
    {
        sm.isCounterBackEnable = val;
    }

    public void TryDoDamage(WeaponController targetWC,bool attackValid,bool counterValid)
    {
        if (sm.isCounterBackSuccess)
        {
            if(counterValid) targetWC.wm.am.Stun();
        }

        else if(sm.isCounterBackFailure)
        {
            if(attackValid) HitOrDie(targetWC,false);
        }

        else if (sm.isImmortal) return;

        else if (sm.isDefense)
        {
            Block();
        }
        else { 
          if(attackValid) HitOrDie(targetWC,true);
        } 
    }

    public void Block()
    {
        ac.IssueTrigger("block");
    }
    
    public void Stun()
    {
        bm.fsm.TransformState(State.Stun);
    }

    public void Hit()
    {
        ac.IssueTrigger("hit");
    }

    public void Die()
    {
        ac.animator.Play("die");
        ac.animator.StopPlayback();

        ac.playerInput.inputEnable = false;
        if (ac.cam.lockState) ac.cam.Lock(); //先取消锁定
        ac.cam.enabled = false;
    }

    public void HitOrDie(WeaponController targetWC,bool doHitAnimation)
    {
        if (sm.isDie) return;
        if (sm.HP <= 0) sm.am.Die();
        else
        {
            sm.ChangeHP(-1f * targetWC.GetATK());
            if (sm.HP > 0)
            {
                if (doHitAnimation) Hit();
            }
            else sm.am.Die();
        }
    }

    public void LockActorController(bool val)
    {
        ac.SetBool("lock", val);
    }

    public void LockLever()
    {
        Transform mesh0Transform = transform.Find("o7201out/mesh0");
        //mesh0Transform.Rotate(60, 0, 0);
    }
}
