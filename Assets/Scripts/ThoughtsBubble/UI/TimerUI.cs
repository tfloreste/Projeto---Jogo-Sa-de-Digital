using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TimerUI : MonoBehaviour
{
    [SerializeField] private Animator timerAnimator;
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private Stopwatch stopwatch;

    private bool timeStarted = false;
    private int currentMinutes;
    private int currentSeconds;

    // Start is called before the first frame update
    void Start()
    {
        ResetTime();
        stopwatch.onStowatchStarted.AddListener(TimerStarted);
        stopwatch.onStowatchStopped.AddListener(TimeStopped);
    }

    // Update is called once per frame
    void Update()
    {
        if (!timeStarted)
            return;

        UpdateTimer(stopwatch.Timer);
    }

    private void TimerStarted(float timer)
    {
        timeStarted = true;
        UpdateTimer(timer);

        if (timer > 0.0f)
            ShowTimer();
    }

    private void TimeStopped()
    {
        timeStarted = false;
        ResetTime();
        HideTimer();
    }

    private void UpdateTimer(float timer)
    {
        int minutes = (int)(timer / 60);
        int seconds = (int)Mathf.Ceil(timer % 60);
        
        if(seconds == 60)
        {
            minutes++;
            seconds -= 60;
        }

        if(minutes != currentMinutes || seconds != currentSeconds)
            timerText.text = string.Format("{0:00} : {1:00}", minutes, seconds);
    }

    private void ResetTime()
    {
        timerText.text = "00 : 00";
    }

    private void ShowTimer()
    {
        timerAnimator.SetTrigger("Open");
    }

    private void HideTimer()
    {
        timerAnimator.SetTrigger("Close");
    }
}
