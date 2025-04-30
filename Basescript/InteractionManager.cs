using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.VersionControl;
using UnityEngine;

public class InteractionManager : IActorManagerInterface
{
    private CapsuleCollider interCol;
    public List<EventCasterManager> overlapEcastm = new List<EventCasterManager>();
    public bool isCol = false;
    public bool interaction = false;
    void Start()
    {
        interCol = GetComponent<CapsuleCollider>();
    }

    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider col)
    {
        isCol = true;
        EventCasterManager[] ecastms = col.GetComponents<EventCasterManager>();
        foreach(var e in ecastms)
        {
            if (!overlapEcastm.Contains(e))
            {
                overlapEcastm.Add(e);
            }
        }
        if (interaction) GameManager.Instance.UpdateInteration(true);
    }
    private void OnTriggerStay(Collider col)
    {
        if (interaction) GameManager.Instance.UpdateInteration(true);
    }

    private void OnTriggerExit(Collider col)
    {
        isCol = false;
        EventCasterManager[] ecastms = col.GetComponents<EventCasterManager>();
        foreach (var e in ecastms)
        {
            if (overlapEcastm.Contains(e))
            {
                overlapEcastm.Remove(e);
            }
        }
        GameManager.Instance.UpdateInteration(false);
        interaction = false;
    }
}
