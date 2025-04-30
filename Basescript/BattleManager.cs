using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

[RequireComponent(typeof(CapsuleCollider))]
public class BattleManager : IActorManagerInterface
{
    public CapsuleCollider defCol; //为避免自动添加col后,col数据不符合预期值,在此手动设置值
    public EnemyFSM fsm;
    public GameObject attacker;
    public GameObject receiver;

    void Start()
    {
        am = GetComponentInParent<ActorManager>();
        defCol = GetComponent<CapsuleCollider>();
        defCol.isTrigger = true;
    }

    void Update()
    {
        
    }

    //受到攻击--检测CapsuleCollider的所有者，向上搜索实体,以此来获取攻击者信息
    private void OnTriggerEnter(Collider col)
    {
        WeaponController targetWC = col.GetComponentInParent<WeaponController>();

        attacker = targetWC.wm.am.ac.model;
        receiver = am.ac.model;

        //为避免攻击在奇怪的角度命中，此处计算攻击者和被攻击者两点所成线和攻击者正前方线所成的角，若小于45度则攻击有效
        Vector3 attackingDir = receiver.transform.position - attacker.transform.position; //向量计算
        float attackingAngle1 = Vector3.Angle(attacker.transform.forward, attackingDir);

        //盾反同理
        Vector3 counterDir = attacker.transform.position - receiver.transform.position;
        float counterAngle1 = Vector3.Angle(receiver.transform.forward, counterDir);
        float counterAngle2 = Vector3.Angle(receiver.transform.forward, attacker.transform.forward); //两人是否面对面

        bool attackVaild = (attackingAngle1 < 45);
        bool counterVaild = (counterAngle1 < 30 && Mathf.Abs(counterAngle2 - 180) < 30);


        if(counterVaild && am.sm.isCounterBackSuccess)
        {
            am.im.interaction = true;
            fsm.TransformState(State.Stun);
            return;
        }

        if (col.gameObject.CompareTag("Weapon"))
        {
            if (am.ac.model.CompareTag("Player"))
            {
                am.TryDoDamage(targetWC, attackVaild, counterVaild);
            }
            else if (am.ac.model.CompareTag("Enemy"))
            {
                fsm.parameter.getHit = true;
                fsm.ChangeHP(targetWC, attackVaild);
            }
        }
    }
}
