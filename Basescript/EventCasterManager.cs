using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//为区分背刺检测和事件检测，使用em和im
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
