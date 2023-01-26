using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace ThoughtBubbleMiniGame
{
    public class ScoreManager : Singleton<ScoreManager>
    {
        public const int maxPoints = 100;
        public const int minPoints = 0;
        public const int pointsRange = maxPoints - minPoints;

        public int score { get; private set; }
        public event Action<int, int> onScoreChanged;

        private void Awake() => score = 0;

        public void IncreaseScore(int increaseAmount)
        {
            int prevScore = score;
            score = Mathf.Max(Mathf.Min(score + increaseAmount, maxPoints), minPoints);

            onScoreChanged?.Invoke(prevScore, score);
        }

        public void DecreaseScore(int decreaseAmount)
        {
            int prevScore = score;
            score = Mathf.Max(Mathf.Min(score - decreaseAmount, maxPoints), minPoints);

            onScoreChanged?.Invoke(prevScore, score);
        }

        public void SetScore(int newScore)
        {
            if (score == newScore)
                return;

            int prevScore = score;
            score = newScore;

            onScoreChanged?.Invoke(prevScore, score);
        }
    }
}
