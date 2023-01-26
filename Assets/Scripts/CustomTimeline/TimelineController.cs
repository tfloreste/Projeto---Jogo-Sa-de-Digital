using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[RequireComponent(typeof(PlayableDirector))]
public class TimelineController : MonoBehaviour, IDataPersistence
{

    [Header("Debug")]
    [SerializeField] private bool ignoreOtherConditions;
    [SerializeField] private bool ignoreSelfCondition;

    [Header("Params")]
    [SerializeField] BoolVariable[] necessaryConditions;
    [SerializeField] BoolVariable thisCondition;
    [SerializeField] bool playOnlyOnce = true;
    [SerializeField] private Animator[] _animatorsToControlUntilEnd;
    [SerializeField] private Collider2D[] collidersToDisable;
    [SerializeField] private GameEvent cutsceneStartedEvent;
    [SerializeField] private GameEvent cutsceneEndedEvent;

    [Header("Main Cutscenes Params")]
    [SerializeField] private bool setCutsceneIndexOnInk = false;
    [SerializeField] private int cutsceneIndex = 0;

    [Header("Music")]
    [SerializeField] private AudioSource backgroundMusicSource;
    [SerializeField] private AudioClip timelineBackgroundClip;
    [SerializeField] private bool changeBackgroundMusic = false;
    [SerializeField] private bool changeVolume = false;
    [SerializeField] private float musicVolume;

    private const string lastCutscenePlayedVarName = "last_finished_cutscene";

    //[SerializeField] private Animator[] _animatorsToControlUntilPause;

    private PlayableDirector _director = null;
    /*private Dictionary<string, Animator> _animatorsDictionary = null; */
    private Dictionary<string, RuntimeAnimatorController> _animatorControllerDictionary = null;

    private bool _isPlaying = false;

    private void Start()
    {
        _director = GetComponent<PlayableDirector>();

        if (!ignoreSelfCondition && thisCondition && thisCondition.value)
            gameObject.SetActive(false);
        else
            StartCoroutine(CheckConditions());
        
    }

    private bool ConditionsMet()
    {
        Debug.Log("Checking conditions for " + gameObject.name);
        if (ignoreOtherConditions)
            return true;

        if(thisCondition)
            Debug.Log("thisCondition for " + gameObject.name + " is '" + thisCondition.name + "' with value: " + thisCondition.value);

        if (playOnlyOnce && thisCondition && thisCondition.value)
            return false;

        foreach(BoolVariable condition in necessaryConditions)
        {
            Debug.Log("condition '" + condition.name + "' for " + gameObject.name + " has value: " + condition.value);
            if (!condition.value)
                return false;
        }

        return true;
    }

    public void StartTimeline()
    {
        //_animatorsDictionary = new Dictionary<string, Animator>();
        Debug.Log("Starting timeline from " + gameObject.name);
        _animatorControllerDictionary = new Dictionary<string, RuntimeAnimatorController>();
        
        if(collidersToDisable != null)
        {
            foreach (Collider2D collider in collidersToDisable)
            {
                collider.enabled = false;
            }
        }

        SaveAnimatorsControllers(_animatorsToControlUntilEnd);
        //SaveAnimatorsControllers(_animatorsToControlUntilPause);
        // AddAnimatorsToCache();

        //_director.stopped += RestoreAnimatorsOnStop;
        //_director.stopped += TimelineFinished; // Asume que a timeline só será pausada no final
        //_director.paused += RestoreAnimatorsOnPause;

        if(changeBackgroundMusic && timelineBackgroundClip && backgroundMusicSource)
        {
            backgroundMusicSource.clip = timelineBackgroundClip;

            if (changeVolume)
                backgroundMusicSource.volume = musicVolume;

            backgroundMusicSource.Play();
        }

        _director.Play();
        _isPlaying = true;
        cutsceneStartedEvent?.Invoke();
    }

    public void ResumeTimeline()
    {
        if (!IsPaused() || !ConditionsMet())
            return;

        //SaveAnimatorsControllers(_animatorsToControlUntilPause);
        //_director.paused += RestoreAnimatorsOnPause;

        _director.Play();
    }

    
    public void TimelineFinished()
    {
        if (!_isPlaying)
            return;

        if (collidersToDisable != null)
        {
            foreach (Collider2D collider in collidersToDisable)
                collider.enabled = true;
        }

        RestoreAnimatorsOnStop();

        if (thisCondition)
        {
            thisCondition.value = true;
        }

        if (setCutsceneIndexOnInk && DialogueManager.Instance)
            DialogueManager.Instance.SetDialogueVariable<int>(lastCutscenePlayedVarName, cutsceneIndex);

        DataPersistenceManager.instance.SaveGame();

        _isPlaying = false;
        cutsceneEndedEvent?.Invoke();
    }
   
    public bool IsPaused() => _director.state == PlayState.Paused;

    private void SaveAnimatorsControllers(Animator[] animatorList)
    {
        foreach (Animator anim in animatorList)
        {
            AddAnimatorToDictionary(anim);
            anim.runtimeAnimatorController = null;
        }
    }

    private void RestoreAnimatorsOnStop()
    {
        foreach(Animator anim in _animatorsToControlUntilEnd)
        {
            RestoreAnimator(anim);
        }

        //_director.stopped -= RestoreAnimatorsOnStop;
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


    //--------------------------  IDataPersistance Interface --------------------------------------//
    public void LoadData(GameData data)
    {
        if (!this.gameObject.activeSelf)
            return;

        if (thisCondition != null)
        {
            thisCondition.value = false;

            if (data.conditions.ContainsKey(thisCondition.name))
                thisCondition.value = data.conditions[thisCondition.name];
        }
            

        foreach (BoolVariable condition in necessaryConditions)
        {
            condition.value = false;
            if (data.conditions.ContainsKey(condition.name))
                condition.value = data.conditions[condition.name];
        }
    }

    public void SaveData(GameData data)
    {
        if (!this.gameObject.activeSelf)
            return;

        if (!thisCondition)
            return;

        if (data.conditions.ContainsKey(thisCondition.name))
        {
            data.conditions[thisCondition.name] = thisCondition.value;
        }
        else
        {
            data.conditions.Add(thisCondition.name, thisCondition.value);
        }
    }


    private IEnumerator CheckConditions()
    {
        while(!ConditionsMet())
        {
            yield return new WaitForSeconds(0.5f);
        }

        StartTimeline();
    }
}
