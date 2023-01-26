using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ThoughtBubbleMiniGame
{
    public class PointScaleUI : MonoBehaviour
    {
        private enum PointsState
        {
            WINNING,
            LOSING,
            WARNING
        }

        [SerializeField] private Image pointsFiller;
        [SerializeField] private Sprite winningUISprite;
        [SerializeField] private Sprite losingUISprite;
        [SerializeField] private Sprite warningUISprite;
        [SerializeField] private float winningMinPercentage;
        [SerializeField] private float losingMinPercentage;
        [SerializeField] private float changePointsAnimationTime = 0.5f;

        private float currentUIPoints;
        private PointsState currentPointState;
        private Queue<int> changePointsQueue;

        private int maxPoints = 100;

        private void Awake()
        {
            changePointsQueue = new Queue<int>();
        }

        private void Start()
        {
            currentUIPoints = ScoreManager.Instance.score;
            pointsFiller.fillAmount = currentUIPoints / ScoreManager.pointsRange;
            currentPointState = GetCurrentState();
            UpdatePointState(currentPointState);

            ScoreManager.Instance.onScoreChanged +=
                (int prevScore, int newScore) => changePointsQueue.Enqueue(newScore);

            StartCoroutine(ObserveChangePointsQueue());
        }

        private IEnumerator ObserveChangePointsQueue()
        {
            while(true)
            {
                if (changePointsQueue.Count > 0)
                    yield return ChangePointsCoroutine(changePointsQueue.Dequeue());
                else
                    yield return new WaitForSeconds(0.1f);

                while (changePointsQueue.Count > 2)
                    changePointsQueue.Dequeue();
            }
        }

        private IEnumerator ChangePointsCoroutine(int realPoints)
        {
            float startingUIPoints = currentUIPoints;
            float currentAnimTime = 0.0f;
            float pointsChangePerSecond = (realPoints - currentUIPoints) / changePointsAnimationTime;


            while (currentAnimTime < changePointsAnimationTime)
            {
                yield return new WaitForEndOfFrame();

                currentAnimTime += Time.deltaTime;
                currentUIPoints += pointsChangePerSecond * Time.deltaTime;

                if (
                    (startingUIPoints < realPoints && currentUIPoints > realPoints) ||
                    (startingUIPoints > realPoints && currentUIPoints < realPoints)
                 )
                {
                    currentUIPoints = realPoints;
                }

                pointsFiller.fillAmount = currentUIPoints / ScoreManager.pointsRange;

                if (currentPointState != GetCurrentState())
                {
                    UpdatePointState(GetCurrentState());
                }

            }
        }

        private PointsState GetCurrentState()
        {
            if (pointsFiller.fillAmount >= winningMinPercentage/100.0f)
                return PointsState.WINNING;

            if (pointsFiller.fillAmount >= losingMinPercentage/100.0f)
                return PointsState.LOSING;

            return PointsState.WARNING;
        }

        private void UpdatePointState(PointsState state)
        {
            switch (state)
            {
                case PointsState.WINNING:
                    currentPointState = PointsState.WINNING;
                    pointsFiller.sprite = winningUISprite;
                    break;

                case PointsState.LOSING:
                    currentPointState = PointsState.LOSING;
                    pointsFiller.sprite = losingUISprite;
                    break;

                case PointsState.WARNING:
                default:
                    currentPointState = PointsState.WARNING;
                    pointsFiller.sprite = warningUISprite;
                    break;
            }

        }

    }
}