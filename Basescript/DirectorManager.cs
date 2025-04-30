using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;
using UnityEngine.Timeline;
using UnityEngine.UI;

[RequireComponent(typeof(PlayableDirector))]
public class DirectorManager : IActorManagerInterface 
{
    public PlayableDirector pd;
    public TimelineAsset timeLineAsset;
    public Image fadeImage;
    public GameObject change;
    private float fadeDuration = 2f;
    private bool isFading = false;
    void Start()
    {
        pd = GetComponent<PlayableDirector>();
        pd.playOnAwake = false;
    }

    void Update()
    {
         
    }

    void load(string timelineName)
    {
        string folderPath = "Assets/Timeline";
        string[] guids = AssetDatabase.FindAssets(timelineName + " t:TimelineAsset", new[] { folderPath });

        if (guids.Length > 0)
        {
            string path = AssetDatabase.GUIDToAssetPath(guids[0]);
            TimelineAsset timeline = AssetDatabase.LoadAssetAtPath<TimelineAsset>(path);

            if (timeline != null) pd.playableAsset = timeline;
        }
    }

    public void PlayFrontStab(string timelineName,ActorManager attacker,ActorManager victim)
    {
        if (pd.state == PlayState.Playing) return;

        if(timelineName == "Stab")
        {
           load("Stab Timeline");
           foreach (var track in pd.playableAsset.outputs)
           {
               if (track.streamName == "AttackerTrack") pd.SetGenericBinding(track.sourceObject, attacker);
               else if (track.streamName == "VictimTrack") pd.SetGenericBinding(track.sourceObject, victim);
               else if (track.streamName == "Attacker Animation") pd.SetGenericBinding(track.sourceObject, attacker.ac.animator);
               else if (track.streamName == "Victim Animation") pd.SetGenericBinding(track.sourceObject, victim.ac.animator);
           }
            pd.Play();
        }
    }

    public void PlayOpenBox(string timelineName, ActorManager player, ActorManager box)
    {
        if (pd.state == PlayState.Playing) return;
        if (timelineName == "openBox")
        {
            load("openBox");
            foreach (var track in pd.playableAsset.outputs)
            {
                if (track.streamName == "Player Animation") pd.SetGenericBinding(track.sourceObject, player.ac.animator);
                else if (track.streamName == "Box Animation") pd.SetGenericBinding(track.sourceObject, box.ac.animator);
                else if (track.streamName == "Player Script") pd.SetGenericBinding(track.sourceObject, player);
            }
            pd.Play();
            StartCoroutine(WaitForTimelineToFinish());
        }
    }

    public void PlayLeverUp(string timelineName, ActorManager player, ActorManager lever)
    {
        if (pd.state == PlayState.Playing) return;
        if (timelineName == "leverUp")
        {
            load("leverUp");
            foreach (var track in pd.playableAsset.outputs)
            {
                if (track.streamName == "Player Animation") pd.SetGenericBinding(track.sourceObject, player.ac.animator);
                else if (track.streamName == "Lever Animation") pd.SetGenericBinding(track.sourceObject, lever.ac.animator);
                else if (track.streamName == "Player Script") pd.SetGenericBinding(track.sourceObject, player);
                else if (track.streamName == "Lever Script") pd.SetGenericBinding(track.sourceObject, lever);
            }
            pd.Play();
            StartCoroutine(FadeCycle());
        }
    }

    IEnumerator WaitForTimelineToFinish()
    {
        while (pd.state != PlayState.Playing)
        {
            yield return null;
        }

        // 等待时间轴播放完成
        while (pd.state == PlayState.Playing)
        {
            yield return null;
        }
        StartCoroutine(GameManager.Instance.UpdateGetGoods());

    }

    IEnumerator FadeCycle()
    {
        yield return StartCoroutine(Fade(0, 1));
    }

    IEnumerator Fade(float startAlpha, float targetAlpha)
    {
        while (pd.state != PlayState.Playing)
        {
            yield return null;
        }

        // 等待时间轴播放完成
        while (pd.state == PlayState.Playing)
        {
            yield return null;
        }

        change.SetActive(true);
        float elapsedTime = 0f;
        Color color = fadeImage.color;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            color.a = Mathf.Lerp(startAlpha, targetAlpha, 0.01f);
            fadeImage.color = color;
            yield return null;
        }

        color.a = targetAlpha;
        fadeImage.color = color;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
