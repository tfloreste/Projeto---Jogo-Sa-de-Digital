using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace ThoughtBubbleMiniGame
{
    public abstract class LevelStep : MonoBehaviour
    {
        public enum State
        {
            INITIZALIZED,
            STARTED,
            PAUSED,
            WON,
            LOSED,
            FINISHED
        };

        [SerializeField] protected bool startFromPreviousLevelPoints = false;
        [SerializeField] protected int startingPoints = 30;
        [SerializeField] protected int winningPoints = 100;
        [SerializeField] protected int minPointsToWin = 50;
        [SerializeField] protected int losingPoints = 0;

        public UnityEvent<LevelStep> onLevelStart;
        public UnityEvent<LevelStep> onLevelEnd;
        public UnityEvent onLevelStepWon;
        public UnityEvent onLevelStepLose;

        public bool StartFromPreviousLevelPoints { get; protected set; }
        public State currentState { get; protected set; } = State.INITIZALIZED;


        protected void Update()
        {
            if (currentState != State.STARTED)
                return;

            int currentPoints = ScoreManager.Instance.score;

            if (currentPoints >= winningPoints)
                WonLevelStep();
            else if (currentPoints <= losingPoints)
                LoseLevelStep();
        }

        public void StartLevelStep()
        {
            if (!startFromPreviousLevelPoints)
                ScoreManager.Instance.SetScore(startingPoints);

            StartCoroutine(PlayLevelStep());
        }

        abstract protected IEnumerator PlayLevelStep();


        protected void WonLevelStep()
        {
            currentState = State.WON;
            EndLevelStep();
            onLevelStepWon?.Invoke();
        }

        protected void LoseLevelStep()
        {
            currentState = State.LOSED;
            EndLevelStep();
            onLevelStepLose?.Invoke();
        }

        protected void EndLevelStep()
        {
            if (currentState != State.WON && currentState != State.LOSED)
                currentState = State.FINISHED;

            ThoughtBubble[] bubbles = FindObjectsOfType<ThoughtBubble>();
            foreach (var bubble in bubbles)
                bubble.StopExpandingAndFade();

            onLevelEnd?.Invoke(this);
        }

    }
}