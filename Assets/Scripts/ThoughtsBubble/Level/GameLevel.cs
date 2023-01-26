using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace ThoughtBubbleMiniGame
{
    public class GameLevel : MonoBehaviour
    {
        [SerializeField] private LevelStep[] levelSteps;
        [SerializeField] private bool showTotalLevelTime = true;
        [SerializeField] private int startingScore = 50;
        [SerializeField] private bool autoStartNextLevel = true;
        [SerializeField] private bool endLevelOnLose = true;
        [SerializeField] private Stopwatch stopwatch;

        public UnityEvent<GameLevel> onLevelStarted;
        public UnityEvent<GameLevel> onLevelFinished;

        public int currentStepIndex { get; private set; } = -1;

        private void Start()
        {
            if (!stopwatch)
                stopwatch = FindObjectOfType<Stopwatch>();
        }

        public void Play()
        {
            onLevelStarted?.Invoke(this);
            currentStepIndex = -1;
            PlayNextLevelStep();
        }

        public void PlayNextLevelStep()
        {
            Debug.Log("PlayNextLevelStep fired");
            currentStepIndex++;
            
            if(levelSteps.Length <= currentStepIndex)
            {
                EndLevel();
                return;
            }

            if (currentStepIndex == 0)
                PrepareLevel();

            if (autoStartNextLevel)
                levelSteps[currentStepIndex].onLevelEnd.AddListener(OnLevelStepEnd);

            PlayCurrentLevel();
        }

        public void PlayCurrentLevel()
        {
            levelSteps[currentStepIndex].StartLevelStep();
        }

        public void OnLevelStepEnd(LevelStep levelStep)
        {
            if (endLevelOnLose && levelStep.currentState == LevelStep.State.LOSED)
                EndLevel();
            else if (autoStartNextLevel)
                PlayNextLevelStep();
        }

        public void EndLevel()
        {
            Debug.Log("level ended");
            onLevelFinished?.Invoke(this);
        }

        public LevelStep GetCurrentLevelStep()
        {
            if (currentStepIndex > levelSteps.Length - 1)
                return null;

            return levelSteps[currentStepIndex];
        }

        public LevelStep GetNextLevelStep()
        {
            if (currentStepIndex + 1 > levelSteps.Length - 1)
                return null;

            return levelSteps[currentStepIndex+1];
        }

        public int GetLevelStepCount()
        {
            return levelSteps.Length;
        }

        private void PrepareLevel()
        {
            if (levelSteps[0].StartFromPreviousLevelPoints)
                ScoreManager.Instance.SetScore(startingScore);
        }
    }
}