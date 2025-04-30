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
        //Ϊʵ�ִ����ͱ���Ч����Ҫ���Զ��л�TimeLine�е�ʵ�壬�˴�ʹ��playableAsset����stringƥ�����ʵ��
    }
}
