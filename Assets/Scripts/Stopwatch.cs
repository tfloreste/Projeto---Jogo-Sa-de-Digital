using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Stopwatch : MonoBehaviour
{
    [HideInInspector] private float timer;
    public float Timer { get => timer; set => timer = Mathf.Max(value, 0.0f); }
    public UnityEvent<float> onStowatchStarted;
    public UnityEvent onStowatchStopped;

    private bool timeRunning = false;

    private void Update()
    {
        if (!timeRunning)
            return;

        Timer -= Time.deltaTime;
        if(Mathf.Approximately(Timer, 0.0f))
        {
            StopCounting();
        }
    }

    public void StartCounting(float time)
    {
        Timer = time;
        StartCounting();
    }

    public void StartCounting()
    {
        timeRunning = true;
        onStowatchStarted?.Invoke(Timer);
    }

    public void StopCounting()
    {
        timeRunning = false;
        onStowatchStopped?.Invoke();
    }
}
