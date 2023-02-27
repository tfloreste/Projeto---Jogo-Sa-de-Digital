using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace ThoughtBubbleMiniGame
{
    public class SceneLevel : MonoBehaviour, IDataPersistence
    {
        [Header("Debug")]
        [SerializeField] private bool ignoreOtherConditions;
        [SerializeField] private bool ignoreSelfCondition;

        [Header("INK Integration")]
        [SerializeField] private bool savePointsOnInkVariable = false;
        [SerializeField] private string inkVariableName;

        [Header("Required References")]
        [SerializeField] private DialogueTrigger dialogueTrigger;
        [SerializeField] private BoolVariable[] necessaryConditions;
        [SerializeField] private BoolVariable thisCondition;
        [SerializeField] private GameLevel gameLevel;

        [Header("Params")]
        [SerializeField] private TextAsset[] beforeLevelStepDialogue;
        [SerializeField] private TextAsset[] afterLevelStepDialogue;
        [SerializeField] private TextAsset onFailDialogue;
        [SerializeField] private float dialogueDelay = 0.5f;
        [SerializeField] private ScreenEffect screenEffect;

        [Header("Music")]
        [SerializeField] private AudioSource backgroundMusicSource;
        [SerializeField] private AudioClip backgroundClip;
        [SerializeField] private bool changeBackgroundMusic = false;
        [SerializeField] private bool changeVolume = false;
        [SerializeField] private float musicVolume;

        [Header("Scene Gallery")]
        [SerializeField] private bool addSceneToGallery = false;
        [SerializeField] private bool returnToGalleryAfterSceneEnds = true;
        [SerializeField] private int gallerySceneIndex = -1;
        [SerializeField] private string gallerySceneName = "";

        public UnityEvent onSceneFinished;

        private int currentStepIndex = -1;
        private bool currentDialogueEnded = false;
        private GameData gameDataBeforeStarting;

        private void Start()
        {
            if (gameObject.activeSelf && !screenEffect)
                screenEffect = FindObjectOfType<ScreenEffect>();

            if (!ignoreSelfCondition && thisCondition && thisCondition.Value)
                gameObject.SetActive(false);
            else
                StartCoroutine(CheckConditions());
        }

        private void StartSceneLevel()
        {
            gameDataBeforeStarting = DataPersistenceManager.Instance.GetCurrentGameState();
            if (changeBackgroundMusic && backgroundClip && backgroundMusicSource)
            {
                backgroundMusicSource.clip = backgroundClip;

                if (changeVolume)
                    backgroundMusicSource.volume = musicVolume;

                backgroundMusicSource.Play();
            }

            currentStepIndex = -1;
            StartNextLevelStep();
        }

        private void StartNextLevelStep()
        {
            currentStepIndex++;
            if (currentStepIndex >= gameLevel.GetLevelStepCount())
            {
                EndSceneLevel();
                return;
            }
            Debug.Log("Starting level {" + currentStepIndex + "} from " + gameLevel.gameObject.name);

            LevelStep nextLevelStep = gameLevel.GetNextLevelStep();
            nextLevelStep.onLevelStepWon.AddListener(OnSucess);
            nextLevelStep.onLevelStepLose.AddListener(OnFail);

            StartCoroutine(StartLevelStep());
        }

        private IEnumerator StartLevelStep()
        {
            if(screenEffect.IsScreenFaded())
            {
                screenEffect.FadeIn(false);
                yield return new WaitForSeconds(screenEffect.FadeInDuration);
            }

            yield return new WaitForSeconds(dialogueDelay);
            if(currentStepIndex < beforeLevelStepDialogue.Length && beforeLevelStepDialogue[currentStepIndex])
            {
                yield return ShowDialogue(beforeLevelStepDialogue[currentStepIndex]);
            }

            gameLevel.PlayNextLevelStep();
        }

        private void OnFail()
        {
            Debug.Log("OnFail fired for " + gameObject.name + " at index: " + currentStepIndex);
            StartCoroutine(PerformFailedCoroutine());
        }

        private void OnSucess()
        {
            Debug.Log("OnSucess fired for " + gameObject.name + " at index: " + currentStepIndex);
            StartCoroutine(PerformSuccessCoroutine());
        }

        private IEnumerator PerformFailedCoroutine()
        {
            Debug.Log("PerformFailedCoroutine fired from " + gameObject.name);
            if (onFailDialogue)
            {
                yield return ShowDialogue(onFailDialogue);
            }

            EndSceneLevel();
        }

        private IEnumerator PerformSuccessCoroutine()
        {
            if (currentStepIndex < afterLevelStepDialogue.Length && afterLevelStepDialogue[currentStepIndex])
            {
                yield return ShowDialogue(afterLevelStepDialogue[currentStepIndex]);
            }

            StartNextLevelStep();
        }

        private IEnumerator ShowDialogue(TextAsset inkDialogue)
        {
            Debug.Log("showing dialogue with asset: " + inkDialogue.name);
            yield return new WaitForSeconds(dialogueDelay);
            currentDialogueEnded = false;
            dialogueTrigger.StartDialogue(inkDialogue);
            yield return new WaitUntil(() => currentDialogueEnded);
            yield return new WaitForSeconds(dialogueDelay);
        }

        public void OnDialogueEnded() => currentDialogueEnded = true;

        private void EndSceneLevel()
        {
            if (thisCondition)
                thisCondition.Value = true;

            if (DataPersistenceManager.Instance.LoadedMode == GameLoadedMode.GALLERY)
            {
                if (returnToGalleryAfterSceneEnds)
                {
                    Debug.Log("Returning to gallery");
                    SceneChanger.Instance.GoToSceneGallery(true);
                }

                return;
            }

            if (addSceneToGallery)
                SceneGalleryManager.Instance.AddSceneToGallery(gallerySceneIndex, gameDataBeforeStarting, gallerySceneName);

            if (savePointsOnInkVariable && inkVariableName != "")
                DialogueManager.Instance.SetDialogueVariable<int>(inkVariableName, ScoreManager.Instance.score);

            onSceneFinished?.Invoke();
        }

        private bool ConditionsMet()
        {
            if (ignoreOtherConditions)
                return true;

            if (thisCondition && thisCondition.Value)
                return false;

            foreach (BoolVariable condition in necessaryConditions)
            {
                if (!condition.Value)
                    return false;
            }

            return true;
        }

        private IEnumerator CheckConditions()
        {
            while (!ConditionsMet())
            {
                yield return new WaitForSeconds(0.5f);
            }

            StartSceneLevel();
        }
        public void LoadData(GameData data)
        {
            if (!gameObject.activeSelf)
                return;

            if (thisCondition != null)
            {
                thisCondition.Value = false;

                if (data.conditions.ContainsKey(thisCondition.name))
                    thisCondition.Value = data.conditions[thisCondition.name];
            }


            foreach (BoolVariable condition in necessaryConditions)
            {
                condition.Value = false;
                if (data.conditions.ContainsKey(condition.name))
                    condition.Value = data.conditions[condition.name];
            }
        }

        public void SaveData(GameData data)
        {
            if (data.conditions.ContainsKey(thisCondition.name))
            {
                data.conditions[thisCondition.name] = thisCondition.Value;
            }
            else
            {
                data.conditions.Add(thisCondition.name, thisCondition.Value);
            }
        }
    }
}