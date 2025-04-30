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

    //该控件被创建时调用该函数
    public override void OnPlayableCreate (Playable playable)
    {
        
    }

    //该剧本第一个Clip开始时调用
    public override void OnGraphStart(Playable playable)
    {
        //为通过TimeLine调整挂载脚本的数值,要先向上搜索找到脚本本体
        pd = (PlayableDirector)playable.GetGraph().GetResolver();
        foreach (var track in pd.playableAsset.outputs)
        {
            if (track.streamName == "AttackerTrack" || track.streamName == "VictimTrack" || track.streamName == "Player Script")
            {
                ActorManager am = (ActorManager)pd.GetGenericBinding(track.sourceObject);
                am.LockActorController(true); //为实现处决者的无敌状态，让处决时一直处于lock状态
            }
            if(track.streamName == "VictimTrack")
            {
                ActorManager am = (ActorManager)pd.GetGenericBinding(track.sourceObject);
                am.bm.fsm.ChangeHP(30);
            }
        }
    }

    //该剧本最后一个Clip结束时调用
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

    //该track中每个Clip开始时调用
    public override void OnBehaviourPlay(Playable playable, FrameData info)
    {
        
    }

    //该track中每个Clip结束时调用
    public override void OnBehaviourPause(Playable playable, FrameData info)
    {
        
    }

    //该track中每个Clip运行时每帧调用
    public override void PrepareFrame(Playable playable, FrameData info)
    {
        
    }
}
