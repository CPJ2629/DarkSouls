using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

[RequireComponent(typeof(CapsuleCollider))]
public class BattleManager : IActorManagerInterface
{
    public CapsuleCollider defCol; //Ϊ�����Զ����col��,col���ݲ�����Ԥ��ֵ,�ڴ��ֶ�����ֵ
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

    //�ܵ�����--���CapsuleCollider�������ߣ���������ʵ��,�Դ�����ȡ��������Ϣ
    private void OnTriggerEnter(Collider col)
    {
        WeaponController targetWC = col.GetComponentInParent<WeaponController>();

        attacker = targetWC.wm.am.ac.model;
        receiver = am.ac.model;

        //Ϊ���⹥������ֵĽǶ����У��˴����㹥���ߺͱ����������������ߺ͹�������ǰ�������ɵĽǣ���С��45���򹥻���Ч
        Vector3 attackingDir = receiver.transform.position - attacker.transform.position; //��������
        float attackingAngle1 = Vector3.Angle(attacker.transform.forward, attackingDir);

        //�ܷ�ͬ��
        Vector3 counterDir = attacker.transform.position - receiver.transform.position;
        float counterAngle1 = Vector3.Angle(receiver.transform.forward, counterDir);
        float counterAngle2 = Vector3.Angle(receiver.transform.forward, attacker.transform.forward); //�����Ƿ������

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
