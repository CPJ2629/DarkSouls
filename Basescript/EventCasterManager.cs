using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Ϊ���ֱ��̼����¼���⣬ʹ��em��im
public class EventCasterManager : IActorManagerInterface
{
    public string eventName;
    public bool active = true;
    public GameObject model;
    void Start()
    {
        if (am == null)
        {
            am = GetComponent<ActorManager>();
        }
    }
     
    void Update()
    {
        
    }
}
