using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace ThoughtBubbleMiniGame
{
    public class RandomLevelStep : LevelStep
    {
        [SerializeField] protected ThougthBubbleData[] thoughtBubbles;
        [SerializeField] protected float spawnTimer = 0.5f;
        [SerializeField] protected float levelTimeSeconds = 30.0f;
        [SerializeField] protected bool useTimer = true;
        [SerializeField] protected Stopwatch stopwatch;

        public float remainingTime { get; private set; }

        private void Start()
        {
            if (!stopwatch)
                stopwatch = FindObjectOfType<Stopwatch>();
        }


        protected override IEnumerator PlayLevelStep()
        {
            ThougthBubbleData.SetThoughtBubblesProbabilities(thoughtBubbles);

            if (useTimer)
                StartTimer();


            onLevelStart?.Invoke(this);
            currentState = State.STARTED;

            while (true)
            {
                if (currentState != State.STARTED)
                    yield break;

                BubbleSpawner.Instance.SpawnRandon(thoughtBubbles);
                yield return new WaitForSeconds(spawnTimer);
            }
        }

        protected void StartTimer()
        {
            remainingTime = levelTimeSeconds;
            stopwatch.onStowatchStopped.AddListener(TimeOver);
            stopwatch.StartCounting(levelTimeSeconds);
            onLevelEnd.AddListener(arg => stopwatch.StopCounting());
        }

        protected void TimeOver()
        {
            stopwatch.onStowatchStopped.RemoveListener(TimeOver);

            if (currentState != State.STARTED)
                return;

            if (ScoreManager.Instance.score >= minPointsToWin)
                WonLevelStep();
            else
                LoseLevelStep();

        }

    }
}