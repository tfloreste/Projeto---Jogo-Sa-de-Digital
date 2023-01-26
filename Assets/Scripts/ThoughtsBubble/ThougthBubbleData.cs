using UnityEngine;

namespace ThoughtBubbleMiniGame
{
    [System.Serializable]
    public class ThougthBubbleData
    { 
        [System.Serializable]
        public struct ThoughtBubbleProperty<T>
        {
            public bool useDefault;
            public T customValue;
        }

        public ThoughtBubble prefab;
        public float spawnPriority; // usado para calcular probabilidades
        public ThoughtBubbleProperty<float> maxScale;
        public ThoughtBubbleProperty<float> explodeTime;
        public ThoughtBubbleProperty<int> pointsOnTouch;
        public ThoughtBubbleProperty<int> pointsOnExplode;
        public float probability { get; private set; }

        public static void SetThoughtBubblesProbabilities(ThougthBubbleData[] bubblesData)
        {
            float prioritySum = 0.0f;
            foreach (ThougthBubbleData bubbleData in bubblesData)
                prioritySum += Mathf.Max(bubbleData.spawnPriority, 0.0f); // Para evitar prioridades negativas

            for (int i = 0; i < bubblesData.Length; i++)
                bubblesData[i].probability = bubblesData[i].spawnPriority / prioritySum;
        }

    }
}