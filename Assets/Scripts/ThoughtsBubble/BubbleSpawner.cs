using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ThoughtBubbleMiniGame
{
    public class BubbleSpawner : Singleton<BubbleSpawner>
    {
        [System.Serializable]
        private struct ValueRange 
        {
            public float min;
            public float max;
        }

        [SerializeField] private ValueRange xRange;
        [SerializeField] private ValueRange yRange;
        [SerializeField] private float zValue = 10.0f;

        public ThoughtBubble SpawnRandon(ThougthBubbleData[] thoughtBubbles)
        {
            ThougthBubbleData randomBubbleData = GetRandomBubble(thoughtBubbles);

            if(randomBubbleData == null)
            {
                Debug.LogWarning("Error on getting random bubble");
                return null;
            }

            float randomX = Random.Range(xRange.min, xRange.max);
            float randomY = Random.Range(yRange.min, yRange.max);

            Vector3 position = new Vector3(randomX, randomY, zValue);
            return SpawnAtPosition(randomBubbleData, position);
        }

        public ThoughtBubble SpawnAtPosition(ThougthBubbleData bubbleData, Vector3 position)
        {
            ThoughtBubble bubbleInstance = Instantiate(bubbleData.prefab, position, Quaternion.identity);
            SetCustomValue(bubbleInstance, bubbleData);
            bubbleInstance.Expand();

            return bubbleInstance;
        }

        private ThougthBubbleData GetRandomBubble(ThougthBubbleData[] thoughtBubbles)
        {
            float randomValue = Random.Range(0.0f, 1.0f);
            float currentValue = 0.0f;

            foreach(ThougthBubbleData bubbleData in thoughtBubbles)
            {
                currentValue += bubbleData.probability;       
                if (randomValue < currentValue)
                    return bubbleData;
            }

            return null;
        }

        private void SetCustomValue(ThoughtBubble bubble, ThougthBubbleData bubbleData)
        {
            if (!bubbleData.maxScale.useDefault)
                bubble.SetMaxScale(bubbleData.maxScale.customValue);

            if (!bubbleData.explodeTime.useDefault)
                bubble.SetExplodeTime(bubbleData.explodeTime.customValue);

            if (!bubbleData.pointsOnTouch.useDefault)
                bubble.SetPointsOnTouch(bubbleData.pointsOnTouch.customValue);

            if (!bubbleData.pointsOnExplode.useDefault)
                bubble.SetPointsOnExplode(bubbleData.pointsOnExplode.customValue);
        }
    }

}