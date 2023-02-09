using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace ThoughtBubbleMiniGame
{
    public class Tutorial : MonoBehaviour, IDataPersistence
    {
        [SerializeField] private TextMeshProUGUI tutorialText;
        [SerializeField] private GameLevel tutorialLevel;
        [SerializeField] private BoolVariable tutorialCompletedCondition;
        [SerializeField] private string[] stepFailedTexts;
        [SerializeField] private string[] stepCompletedTexts;
        [SerializeField] private float timeBetweenTexts = 2.0f;
        [SerializeField] private float fadeAnimationTime = 1.0f;
        [SerializeField] private ScreenEffect screenEffect;

        private TMPFaderEffect tutorialTextFadeEffect;

        int currentStepIndex = -1;
        bool dataLoaded = false;

        private void Start()
        {
            tutorialTextFadeEffect = tutorialText.gameObject.GetComponent<TMPFaderEffect>();
            tutorialText.gameObject.SetActive(false);

            StartCoroutine(WaitForDataLoading());
        }

        private IEnumerator WaitForDataLoading()
        {
            yield return new WaitUntil(() => dataLoaded);

            if (tutorialCompletedCondition.Value)
                this.gameObject.SetActive(false);
            else
                StartCoroutine(StartTutorial());
        }

        private IEnumerator StartTutorial()
        {
            screenEffect.FadeIn(false);
            yield return new WaitForSeconds(screenEffect.FadeInDuration);

            currentStepIndex = -1;
            ShowNextTutorial();
        }

        private void ShowNextTutorial()
        {
            currentStepIndex++;
            if(currentStepIndex >= tutorialLevel.GetLevelStepCount())
            {
                EndTutorial();
                return;
            }

            LevelStep nextLevelStep = tutorialLevel.GetNextLevelStep();
            nextLevelStep.onLevelStepWon.AddListener(OnSucess);
            nextLevelStep.onLevelStepLose.AddListener(OnFail);

            tutorialLevel.PlayNextLevelStep();
        }

        private void OnFail()
        {
            StartCoroutine(PerformFailedCoroutine());
        }

        private void OnSucess()
        {
            Debug.Log("OnSucess fired");
            StartCoroutine(PerformSuccessCoroutine());
        }

        private IEnumerator PerformFailedCoroutine()
        {
            if (currentStepIndex < stepFailedTexts.Length && stepFailedTexts[currentStepIndex].Trim() != "")
            {
                Color color = tutorialText.color;
                tutorialText.color = new Color(color.r, color.g, color.b, 0.0f);
                tutorialText.text = stepFailedTexts[currentStepIndex].Trim();

                if (!tutorialText.gameObject.activeSelf)
                    tutorialText.gameObject.SetActive(true);

                yield return tutorialTextFadeEffect.FadeIn(fadeAnimationTime);
                yield return new WaitForSeconds(timeBetweenTexts);
                yield return tutorialTextFadeEffect.FadeOut(fadeAnimationTime);
            }

            tutorialLevel.PlayCurrentLevel();
        }

        private IEnumerator PerformSuccessCoroutine()
        {
            if (currentStepIndex < stepCompletedTexts.Length && stepCompletedTexts[currentStepIndex].Trim() != "")
            {
                Color color = tutorialText.color;
                tutorialText.color = new Color(color.r, color.g, color.b, 0.0f);
                tutorialText.text = stepCompletedTexts[currentStepIndex].Trim();

                if (!tutorialText.gameObject.activeSelf)
                    tutorialText.gameObject.SetActive(true);

                yield return tutorialTextFadeEffect.FadeIn(fadeAnimationTime);
                yield return new WaitForSeconds(timeBetweenTexts);
                yield return tutorialTextFadeEffect.FadeOut(fadeAnimationTime);
            }
            
            ShowNextTutorial();
        }

        private void EndTutorial()
        {
            tutorialCompletedCondition.Value = true;
            this.gameObject.SetActive(false);
            //DataPersistenceManager.instance.SaveGame();
        }

        public void LoadData(GameData data)
        {

            if (!gameObject.activeSelf)
                return;

            if (tutorialCompletedCondition != null)
            {
                tutorialCompletedCondition.Value = false;

                if (data.conditions.ContainsKey(tutorialCompletedCondition.name))
                    tutorialCompletedCondition.Value = data.conditions[tutorialCompletedCondition.name];
            }

            dataLoaded = true;
        }

        public void SaveData(GameData data)
        {
            if (data.conditions.ContainsKey(tutorialCompletedCondition.name))
            {
                data.conditions[tutorialCompletedCondition.name] = tutorialCompletedCondition.Value;
            }
            else
            {
                data.conditions.Add(tutorialCompletedCondition.name, tutorialCompletedCondition.Value);
            }
        }
    }
}