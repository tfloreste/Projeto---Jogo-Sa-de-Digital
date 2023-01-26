using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ThoughtBubbleMiniGame
{
    public class StreamlinedLevelStep : LevelStep
    {
        [System.Serializable]
        struct StreamlinedBubbleData
        {
            public ThougthBubbleData bubbleData;
            public Vector3 position;
        }

        [System.Serializable]
        struct StreamlinedWave
        {
            public StreamlinedBubbleData[] waveBubbles;
            public float spawnDelay;
            public float endWaveDelay;
        }

        [SerializeField] private StreamlinedWave[] bubbleWaves;


        protected override IEnumerator PlayLevelStep()
        {

            List<ThoughtBubble> instanciedBubbles = new List<ThoughtBubble>();
            onLevelStart?.Invoke(this);
            currentState = State.STARTED;

            //foreach(StreamlinedWave waveInfo in bubbleWaves)
            for (int i = 0; i < bubbleWaves.Length; i++)
            {
                instanciedBubbles.Clear();
                StreamlinedWave waveInfo = bubbleWaves[i];
                foreach (StreamlinedBubbleData streamlinedData in waveInfo.waveBubbles)
                {
                    yield return new WaitForSeconds(waveInfo.spawnDelay);
                    ThoughtBubble instance = BubbleSpawner.Instance.SpawnAtPosition(
                        streamlinedData.bubbleData, streamlinedData.position);

                    instanciedBubbles.Add(instance);
                }

                yield return WaitBubblesBeDestroyed(instanciedBubbles);

                if (i < bubbleWaves.Length - 1)
                    yield return new WaitForSeconds(waveInfo.endWaveDelay);
            }

            EndLevelStep();
        }

        protected IEnumerator WaitBubblesBeDestroyed(List<ThoughtBubble> bubbles)
        {
            while (bubbles.Count > 0)
            {
                bubbles.RemoveAll(instance => !instance);
                yield return new WaitForEndOfFrame();
            }
        }
    }
}