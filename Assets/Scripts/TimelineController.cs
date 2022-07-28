using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[RequireComponent(typeof(PlayableDirector))]
public class TimelineController : MonoBehaviour
{
    [SerializeField] private Animator[] _animatorsToControlUntilEnd;
    //[SerializeField] private Animator[] _animatorsToControlUntilPause;

    private PlayableDirector _director = null;
    /*private Dictionary<string, Animator> _animatorsDictionary = null; */
    private Dictionary<string, RuntimeAnimatorController> _animatorControllerDictionary = null;

    private void Start()
    {
        _director = GetComponent<PlayableDirector>();
        //Invoke("StartTimeline", 1f); // Temporário
        StartTimeline();
    }

    public void StartTimeline()
    {
        //_animatorsDictionary = new Dictionary<string, Animator>();
        _animatorControllerDictionary = new Dictionary<string, RuntimeAnimatorController>();
        SaveAnimatorsControllers(_animatorsToControlUntilEnd);
        //SaveAnimatorsControllers(_animatorsToControlUntilPause);
        // AddAnimatorsToCache();

        _director.stopped += RestoreAnimatorsOnStop;
        //_director.paused += RestoreAnimatorsOnPause;

        _director.Play();
    }

    public void ResumeTimeline()
    {
        if (IsFinished() || !IsPaused())
            return;

        //SaveAnimatorsControllers(_animatorsToControlUntilPause);
        //_director.paused += RestoreAnimatorsOnPause;

        _director.Play();
    }

    public bool IsFinished() => _director.duration < _director.time;


    public bool IsPaused() => _director.state == PlayState.Paused;

    private void SaveAnimatorsControllers(Animator[] animatorList)
    {
        foreach (Animator anim in animatorList)
        {
            AddAnimatorToDictionary(anim);
            anim.runtimeAnimatorController = null;
        }
    }

    private void RestoreAnimatorsOnStop(PlayableDirector director)
    {
        foreach(Animator anim in _animatorsToControlUntilEnd)
        {
            RestoreAnimator(anim);
        }

        _director.stopped -= RestoreAnimatorsOnStop;
    }

    /*private void RestoreAnimatorsOnPause(PlayableDirector director)
    {
        Debug.Log("RestoreAnimatorsOnPause fired");
        foreach (Animator anim in _animatorsToControlUntilPause)
        {
            RestoreAnimator(anim);
            //anim.Play("Base Layer.DialogueClosedFrame");
        }

        _director.paused -= RestoreAnimatorsOnPause;
    }*/

    private void RestoreAnimator(Animator anim)
    {
        string gameObjectName = anim.gameObject.name;
        RuntimeAnimatorController animController = _animatorControllerDictionary[gameObjectName];

        anim.runtimeAnimatorController = animController;
    }

    private void AddAnimatorToDictionary(Animator anim)
    {
        string gameObjectName = anim.gameObject.name;

        if (_animatorControllerDictionary.ContainsKey(gameObjectName))
            return;

        _animatorControllerDictionary.Add(gameObjectName, anim.runtimeAnimatorController);
    }

}
