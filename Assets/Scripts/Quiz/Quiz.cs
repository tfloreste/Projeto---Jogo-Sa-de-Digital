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
    [SerializeField] private Button[] alternativeButtons;

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
    private TextMeshProUGUI[] buttonsTexts;
    private Animator quizPanelAnimator;

    private void Start()
    {
        buttonsTexts = new TextMeshProUGUI[alternativeButtons.Length];
        for (int i = 0; i < alternativeButtons.Length; i++)
        {
            buttonsTexts[i] = alternativeButtons[i].transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        }

        quizPanelAnimator = quizPanel.GetComponent<Animator>();
        quizPanel.SetActive(false);
        
        //StartQuiz();
    }

    public void StartQuiz()
    {
        Debug.Log("Quiz started");

        if(!quizPanel.activeSelf)
            quizPanel.SetActive(true);

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
            alternativeButtons[i].interactable = true;
            alternativeButtons[i].onClick.RemoveAllListeners();

            if (i < currentQuestion.alternatives.Length)
            {
                int selectedIndex = i;
                alternativeButtons[i].onClick.AddListener(() => { ChooseAnswer(selectedIndex); });
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
        Debug.Log("Choose answer at index " + index);
        if(questions[currentQuestionIndex].correctAnswerIndex == index)
        {
            Debug.Log("Correct answer");
            if(correctAnswerChosenClip && SFXManager.GetInstance())
            {
                Debug.Log("Playing clip");
                SFXManager.GetInstance().PlayClip(correctAnswerChosenClip);
            }
        }
        else
        {
            Debug.Log("Wrong asnwer");
            if (wrongAnswerChosenClip && SFXManager.GetInstance())
            {
                Debug.Log("Playing clip");
                SFXManager.GetInstance().PlayClip(wrongAnswerChosenClip);
            }
            if (chooseUntilGetTheRightAnswer)
                alternativeButtons[index].interactable = false;

            return;
        }

        if (currentQuestionIndex < questions.Length - 1)
            ShowNextQuestion();
        else
            CloseQuiz();
    }

}
