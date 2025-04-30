using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class TesterDirector : MonoBehaviour
{
    public PlayableDirector pd;
    public Animator attacker;
    public Animator victim;



    void Start()
    {
        pd = GetComponent<PlayableDirector>();
    }

    void Update()
    {
        if (Input.GetKeyDown("h"))
        {
            pd.enabled = true;
            pd.Play();
        foreach (var track in pd.playableAsset.outputs)
        {
            if (track.streamName == "Attacker Animation") pd.SetGenericBinding(track.sourceObject, attacker);
            else if (track.streamName == "Victim Animation") pd.SetGenericBinding(track.sourceObject, victim);
        }
            
        }
        //为实现处决和背刺效果，要能自动切换TimeLine中的实体，此处使用playableAsset进行string匹配查找实现
    }
}
