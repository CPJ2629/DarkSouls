using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[Serializable]
public class MySuperPlayableBehaviour : PlayableBehaviour
{
    public Camera myCamera;
    public float myFloat;
    private PlayableDirector pd;

    //�ÿؼ�������ʱ���øú���
    public override void OnPlayableCreate (Playable playable)
    {
        
    }

    //�þ籾��һ��Clip��ʼʱ����
    public override void OnGraphStart(Playable playable)
    {
        //Ϊͨ��TimeLine�������ؽű�����ֵ,Ҫ�����������ҵ��ű�����
        pd = (PlayableDirector)playable.GetGraph().GetResolver();
        foreach (var track in pd.playableAsset.outputs)
        {
            if (track.streamName == "AttackerTrack" || track.streamName == "VictimTrack" || track.streamName == "Player Script")
            {
                ActorManager am = (ActorManager)pd.GetGenericBinding(track.sourceObject);
                am.LockActorController(true); //Ϊʵ�ִ����ߵ��޵�״̬���ô���ʱһֱ����lock״̬
            }
            if(track.streamName == "VictimTrack")
            {
                ActorManager am = (ActorManager)pd.GetGenericBinding(track.sourceObject);
                am.bm.fsm.ChangeHP(30);
            }
        }
    }

    //�þ籾���һ��Clip����ʱ����
    public override void OnGraphStop(Playable playable)
    {
        foreach (var track in pd.playableAsset.outputs)
        {
            if (track.streamName == "AttackerTrack" || track.streamName == "VictimTrack" || track.streamName == "Player Script")
            {
                ActorManager am = (ActorManager)pd.GetGenericBinding(track.sourceObject);
                am.LockActorController(false); 
            }

            if(track.streamName =="Lever Script")
            {
                ActorManager am = (ActorManager)pd.GetGenericBinding(track.sourceObject);
                am.LockLever();
            }
            
        }
    }

    //��track��ÿ��Clip��ʼʱ����
    public override void OnBehaviourPlay(Playable playable, FrameData info)
    {
        
    }

    //��track��ÿ��Clip����ʱ����
    public override void OnBehaviourPause(Playable playable, FrameData info)
    {
        
    }

    //��track��ÿ��Clip����ʱÿ֡����
    public override void PrepareFrame(Playable playable, FrameData info)
    {
        
    }
}
