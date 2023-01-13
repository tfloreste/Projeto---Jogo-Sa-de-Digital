using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Quiz : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject quizPanel;
    [SerializeField] private TextMeshProUGUI questionText;
    [SerializeField] private CustomButton[] alternativeButtons;

    [Header("Audio")]
    [SerializeField] private AudioClip correctAnswerChosenClip;
    [SerializeField] private AudioClip wrongAnswerChosenClip;

    [Header("Data")]
    [SerializeField] private QuizQuestion[] questions;

    [Header("Params")]
    [SerializeField] private bool chooseUntilGetTheRightAnswer = false;

    [Header("Quiz Events")]
    [SerializeField] private GameEvent quizStartedEvent;
    [SerializeField] private GameEvent quizEndedEvent;

    private int currentQuestionIndex;
    private Text[] buttonsTexts;
    private Animator quizPanelAnimator;
    private RectTransform quizPanelRectTransform;

    private void Start()
    {
        buttonsTexts = new Text[alternativeButtons.Length];
        for (int i = 0; i < alternativeButtons.Length; i++)
        {
            buttonsTexts[i] = alternativeButtons[i].transform.GetChild(0).GetComponent<Text>();
        }

        quizPanelAnimator = quizPanel.GetComponent<Animator>();
        quizPanelRectTransform = quizPanel.GetComponent<RectTransform>();
        quizPanel.SetActive(false);
        
        //StartQuiz();
    }

    public void StartQuiz()
    {
        if (!quizPanel.activeSelf)
        {
            quizPanelRectTransform.localScale = new Vector3(
                0.0f, 
                0.0f, 
                quizPanelRectTransform.localScale.z
            );

            quizPanel.SetActive(true);
        }
            

        if (quizPanelAnimator)
        {
            quizPanelAnimator.SetTrigger("Open");
        }

        quizStartedEvent?.Invoke();

        currentQuestionIndex = -1;
        ShowNextQuestion();
    }

    public void CloseQuiz()
    {
        if (quizPanelAnimator)
        {
            quizPanelAnimator.SetTrigger("Close");
        }
        else
        {
            quizPanel.SetActive(false);
        }

        quizEndedEvent?.Invoke();
    }

    public void ShowNextQuestion()
    {
        currentQuestionIndex++;
        ShowQuestion(currentQuestionIndex);
    }

    private void ShowQuestion(int index)
    {
        QuizQuestion currentQuestion = questions[index];
        Debug.Log(questions[index].question);
        questionText.text = currentQuestion.question;

        for(int i = 0; i < alternativeButtons.Length; i++)
        {
            alternativeButtons[i].Enable();
            alternativeButtons[i].GetButtonInstance().onClick.RemoveAllListeners();

            if (i < currentQuestion.alternatives.Length)
            {
                int selectedIndex = i;
                alternativeButtons[i].GetButtonInstance().onClick.AddListener(() => { ChooseAnswer(selectedIndex); });
                buttonsTexts[i].text = currentQuestion.alternatives[i];

                if (!alternativeButtons[i].gameObject.activeSelf)
                    alternativeButtons[i].gameObject.SetActive(true);
            } 
            else
            {
                alternativeButtons[i].gameObject.SetActive(false);
            }

        }
    }


    public void ChooseAnswer(int index)
    {
        if(questions[currentQuestionIndex].correctAnswerIndex == index)
        {
            if(correctAnswerChosenClip && SFXManager.GetInstance())
            {
                SFXManager.GetInstance().PlayClip(correctAnswerChosenClip);
            }
        }
        else
        {
            if (wrongAnswerChosenClip && SFXManager.GetInstance())
            {
                SFXManager.GetInstance().PlayClip(wrongAnswerChosenClip);
            }
            if (chooseUntilGetTheRightAnswer)
                alternativeButtons[index].Disable();

            return;
        }

        if (currentQuestionIndex < questions.Length - 1)
            ShowNextQuestion();
        else
            CloseQuiz();
    }

}
