using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public enum DayTime {
    DAY,
    EVENING,
    NIGHT
};

public class ScreenEffect : Singleton<ScreenEffect>, IDataPersistence
{
    public UnityAction onFadeCompleted;

    [SerializeField] private GameObject fadeEffect;
    [SerializeField] private GameObject eveningFilter;
    [SerializeField] private GameObject nightFilter;

    private Animator fadeAnimator;
    private float fadeInDuration = 1.5f;
    private float fadeOutDuration = 1.5f;
    private DayTime currentDayTime = DayTime.DAY;
    
    public float FadeOutDuration { get => fadeOutDuration; private set => fadeOutDuration = value; }
    public float FadeInDuration { get => fadeInDuration; private set => fadeInDuration = value; }

    private void OnEnable()
    {
        if(fadeEffect)
            fadeAnimator = fadeEffect.GetComponent<Animator>();
       
        SaveAnimationClipsDuration();
    }

    public void FadeIn(bool keepActive)
    {
        if (!fadeEffect || !fadeAnimator)
            return;

        StartCoroutine(FadeCoroutine("Base Layer.Fade In", FadeInDuration, keepActive));
    }

    public void FadeOut(bool keepActive)
    {
        if (!fadeEffect || !fadeAnimator)
            return;

        StartCoroutine(FadeCoroutine("Base Layer.Fade Out", FadeOutDuration, keepActive));
    }

    public bool IsScreenFaded()
    {
        return fadeEffect != null && fadeEffect.activeSelf;
    }

    public void SetDayFilter()
    {
        SetScreenFilter(false, false);
        currentDayTime = DayTime.DAY;
    }

    public void SetEveningFilter()
    {
        SetScreenFilter(true, false);
        currentDayTime = DayTime.EVENING;
    }

    public void SetNightFilter()
    {
        SetScreenFilter(false, true);
        currentDayTime = DayTime.NIGHT;
    }

    private void SetDayTime(DayTime dayTime)
    {
        switch(dayTime)
        {
            case DayTime.DAY:
                SetDayFilter();
                break;
            case DayTime.EVENING:
                SetEveningFilter();
                break;
            case DayTime.NIGHT:
                SetNightFilter();
                break;
        }
    }

    public void SetScreenFilter(bool eveningFilterActive, bool nightFilterActive)
    {
        if(eveningFilter)
            eveningFilter.SetActive(eveningFilterActive);

        if(nightFilter)
            nightFilter.SetActive(nightFilterActive);
    }

    private IEnumerator FadeCoroutine(string clipName, float clipDuration, bool keepActive)
    {
        if(!fadeEffect.activeSelf)
            fadeEffect.SetActive(true);

        fadeAnimator.Play(clipName);
        yield return new WaitForSeconds(clipDuration);

        if(!keepActive)
            fadeEffect.SetActive(false);

        onFadeCompleted?.Invoke();
    }

    private void SaveAnimationClipsDuration()
    {
        if (!fadeAnimator)
            return;

        AnimationClip[] clips = fadeAnimator.runtimeAnimatorController.animationClips;
        foreach (AnimationClip clip in clips)
        {
            switch (clip.name)
            {
                case "Fade In":
                    FadeInDuration = clip.length;
                    break;
                case "Fade Out":
                    FadeOutDuration = clip.length;
                    break;
            }
        }
    }

    public void LoadData(GameData data)
    {
        SetDayTime(data.dayTime);
    }

    public void SaveData(GameData data)
    {
        data.dayTime = currentDayTime;
    }
}
