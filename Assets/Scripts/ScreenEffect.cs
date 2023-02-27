using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

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
    [SerializeField] private bool handleFadeInOnSceneStart = false;
    [SerializeField] private float autoFadeInTimeSeconds = 0.2f;

    private enum FadeState
    {
        FADED_OUT,
        FADED_IN,
        NONE
    };

    private Animator fadeAnimator;
    private float fadeInDuration = 1.5f;
    private float fadeOutDuration = 1.5f;
    private DayTime currentDayTime = DayTime.DAY;
    private FadeState currentFadeState = FadeState.NONE;
    private Coroutine currentFadeCoroutine;
    private PlayerSwipeController playerController;
    
    public float FadeOutDuration { get => fadeOutDuration; private set => fadeOutDuration = value; }
    public float FadeInDuration { get => fadeInDuration; private set => fadeInDuration = value; }

    private void Start()
    {
        if (fadeEffect)
            fadeAnimator = fadeEffect.GetComponent<Animator>();

        ForceFadeOutState();

        if (handleFadeInOnSceneStart)
        {
            playerController = FindObjectOfType<PlayerSwipeController>();
            if(playerController)
                playerController.BlockMovement();

            if(DataPersistenceManager.Instance.LoadedMode != GameLoadedMode.GALLERY)
                Invoke("AutoFadeInCall", autoFadeInTimeSeconds);
        }

        SaveAnimationClipsDuration();
    }

    private void AutoFadeInCall()
    {
        if(playerController)
            playerController.UnblockMovement();

        FadeIn(false);
    }

    public void FadeIn(bool keepActive)
    {
        if (!fadeEffect || !fadeAnimator)
            return;

        UpdateCurrentFadeState();
        if (currentFadeState == FadeState.FADED_IN)
            return;

        if (currentFadeCoroutine != null)
            StopCoroutine(currentFadeCoroutine);

        currentFadeCoroutine = StartCoroutine(FadeCoroutine("Base Layer.Fade In", FadeInDuration, keepActive));
        currentFadeState = FadeState.FADED_IN;
    }

    public void FadeOut(bool keepActive)
    {
        if (!fadeEffect || !fadeAnimator)
            return;

        UpdateCurrentFadeState();
        if (currentFadeState == FadeState.FADED_OUT)
            return;

        if (currentFadeCoroutine != null)
            StopCoroutine(currentFadeCoroutine);

        currentFadeCoroutine = StartCoroutine(FadeCoroutine("Base Layer.Fade Out", FadeOutDuration, keepActive));
        currentFadeState = FadeState.FADED_OUT;
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
    private void ForceFadeOutState()
    {
        if (!fadeEffect)
            return;

        fadeEffect.SetActive(true);
        Image fadeImg = fadeEffect.gameObject.GetComponent<Image>();
        if (fadeImg)
            fadeImg.color = new Color(fadeImg.color.r, fadeImg.color.g, fadeImg.color.b, 1.0f);
        else
        {
            SpriteRenderer fadeSprite = fadeEffect.gameObject.GetComponent<SpriteRenderer>();
            fadeSprite.color = new Color(fadeSprite.color.r, fadeSprite.color.g, fadeSprite.color.b, 1.0f);
        }

        currentFadeState = FadeState.FADED_OUT;
    }

    private void UpdateCurrentFadeState()
    {
        Image fadeImg = fadeEffect.gameObject.GetComponent<Image>();
        float currentAlpha;
        if (fadeImg)
        {
            currentAlpha = fadeImg.color.a;
        }
        else
        {
            SpriteRenderer fadeSprite = fadeEffect.gameObject.GetComponent<SpriteRenderer>();
            currentAlpha = fadeSprite.color.a;
        }

        if (Mathf.Approximately(currentAlpha, 1.0f))
            currentFadeState = FadeState.FADED_OUT;
        else if (Mathf.Approximately(currentAlpha, 0.0f))
            currentFadeState = FadeState.FADED_IN;
        else
            currentFadeState = FadeState.NONE;
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
