using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Tutorial : MonoBehaviour
{
    private enum TutorialList {
        MovementTutorial,
        HelpButtonTutorial
    };

    // Lista de objetos para serem desabilitados durante o tutorial
    [SerializeField] private GameObject[] objectsToDisable;

    [Header("Debug")]
    [SerializeField] private bool openOnStart = false;

    [Header("Tutorial Title")]
    [SerializeField] private Animator tutorialTitlePanel;
    [SerializeField] private Animator tutorialTitleTextAnimator;

    [Header("Sounds")]
    [SerializeField] private AudioClip tutorialTitleOpenSfx;
    [SerializeField] private AudioClip tutorialTitleCloseSfx;
    [SerializeField] private AudioClip tutorialTextOpenSfx;
    [SerializeField] private AudioClip tutorialTextCloseSfx;
    [SerializeField] private AudioClip tutorialStepCompletedAudio;

    [Header("Base Requiriments")]
    [SerializeField] private Animator tutorialTextBackgroundAnimator;
    [SerializeField] private TextMeshProUGUI tutorialText;
    [SerializeField] private Animator singleTextPanelAnimator;
    [SerializeField] private TextMeshProUGUI singleTextEl;
    [SerializeField] private Animator singleTextElAnimator;
    [SerializeField] private Animator screenBlockerAnimator;
    [SerializeField] private TutorialList[] tutoriaOrder = new TutorialList[2];
    [SerializeField] private BoolVariable tutorialStartCondition;
    [SerializeField] private PlayerSwipeController playerController;

    [Header("Start of tutorial")]
    [SerializeField] [TextArea] private string[] tutorialStartedTexts;

    [Header("End of tutorial")]
    [SerializeField] [TextArea] private string[] tutorialFinishedTexts;

    [Header("Movement Tutorial")]
    [SerializeField] private GameObject movementHandIcon;
    [SerializeField] [TextArea] private string[] beforeMovementIconTexts;
    [SerializeField] private string duringMovementIconText;
    [SerializeField] [TextArea] private string[] afterMovementIconTexts;
    [SerializeField] private Transform playerTransform;
    [SerializeField] private float moveTotalTime = 1.0f;

    [Header("Helper button Tutorial")]
    [SerializeField] private GameObject pointHandIcon;
    [SerializeField] private Button helperButton;
    [SerializeField] [TextArea] private string[] beforePointHandTexts;
    [SerializeField] private string duringPointHandIconText;
    [SerializeField] [TextArea] private string[] afterPointHandIconTexts;


    // Variaveis base
    private bool isDialogueOpened = false;
    private int currentTextIndex = 0;
    private string[] currentTextList;
    private bool currentTextListFinished = false;
    private bool tutorialStarted = false;

    // Tutorial de movimento
    private bool playerControllerEnabledValue = true;
    private float currentPlayerMovingTime = 0.0f;

    private void Start()
    {
        movementHandIcon.SetActive(false);
        pointHandIcon.SetActive(false);

        if (!openOnStart && tutorialStartCondition.Value)
            gameObject.SetActive(false);

        if (openOnStart)
            StartTutorial();
    }

    private void Update()
    {
        if (!tutorialStarted && tutorialStartCondition.Value)
        {
            tutorialStarted = true;
            StartTutorial();
        }
    }

    public void StartTutorial()
    {
        playerController.BlockMovement();
        playerControllerEnabledValue = playerController.enabled;
        helperButton.interactable = false;

        foreach (GameObject gameObject in objectsToDisable)
            gameObject.SetActive(false);

        StartCoroutine(ShowTutorial());
    }

    private IEnumerator ShowTutorial()
    {
        Debug.Log("ShowTutorial fired");
        yield return new WaitForSeconds(1.2f);

        screenBlockerAnimator.SetTrigger("Show");
        yield return ShowTutorialTitle();

        if (tutorialStartedTexts.Length > 0)
            yield return ShowTutorialTextList(tutorialStartedTexts);

        foreach (var tutorial in tutoriaOrder)
        {
            switch (tutorial) 
            {
                case TutorialList.MovementTutorial:
                    yield return ShowMovementTutorial();
                    break;

                case TutorialList.HelpButtonTutorial:
                    yield return ShowHelpButtonTutorial();
                    break;
            }
        }

        if (tutorialFinishedTexts.Length > 0)
            yield return ShowTutorialTextList(tutorialFinishedTexts);

        yield return CloseTutorialTitle();

        TutorialFinished();
    }

    private void TutorialFinished()
    {
        movementHandIcon.SetActive(false);
        pointHandIcon.SetActive(false);
        helperButton.interactable = true;

        foreach (GameObject gameObject in objectsToDisable)
            gameObject.SetActive(true);

        playerController.UnblockMovement();
        gameObject.SetActive(false);
    }

    private IEnumerator ShowMovementTutorial()
    {
        if (beforeMovementIconTexts.Length > 0)
            yield return ShowTutorialTextList(beforeMovementIconTexts);

        movementHandIcon.SetActive(true);

        if (duringMovementIconText != "")
            yield return ShowSingleText(duringMovementIconText);
        
        screenBlockerAnimator.SetTrigger("Hide");
        yield return WaitForPlayerMovement();
        movementHandIcon.SetActive(false);
        screenBlockerAnimator.SetTrigger("Show");
        yield return SingleTextPanelObjectiveCompleted();

        if (afterMovementIconTexts.Length > 0)
            yield return ShowTutorialTextList(afterMovementIconTexts);
    }

    private IEnumerator WaitForPlayerMovement()
    {
        currentPlayerMovingTime = 0.0f;
        Vector3 lastPlayerPosition;

        playerController.UnblockMovement();
        while(currentPlayerMovingTime < moveTotalTime)
        {
            lastPlayerPosition = playerTransform.position;
            
            yield return new WaitForEndOfFrame();

            Vector3 positionDifference = lastPlayerPosition - playerTransform.position;
            if(positionDifference.normalized.magnitude > 0)
            {
                // Player se moveu
                currentPlayerMovingTime += Time.deltaTime;
            }
        }
        playerController.BlockMovement();
    }

    private IEnumerator ShowHelpButtonTutorial()
    {
        if (beforePointHandTexts.Length > 0)
            yield return ShowTutorialTextList(beforePointHandTexts);

        pointHandIcon.SetActive(true);

        if (duringPointHandIconText != "")
            yield return ShowSingleText(duringPointHandIconText);

        helperButton.interactable = true;
        yield return WaitStartOfDialogue();

        screenBlockerAnimator.SetTrigger("Hide");
        if (duringPointHandIconText != "")
            yield return SingleTextPanelObjectiveCompleted();

        pointHandIcon.SetActive(false);
        yield return WaitEndOfDialogue();

        helperButton.interactable = false;

        if (afterPointHandIconTexts.Length > 0)
            yield return ShowTutorialTextList(afterPointHandIconTexts);
    }

    private IEnumerator WaitStartOfDialogue()
    {
        while(!DialogueManager.Instance.isInDialogueMode)
        {
            yield return new WaitForEndOfFrame();
        }
    }

    private IEnumerator WaitEndOfDialogue()
    {
        while (DialogueManager.Instance.isInDialogueMode)
        {
            yield return new WaitForEndOfFrame();
        }
    }

    private IEnumerator OpenTutorialDialogueCO()
    {
        if (tutorialTextOpenSfx)
            SFXManager.GetInstance().PlayClip(tutorialTextOpenSfx);

        tutorialTextBackgroundAnimator.SetTrigger("Open");
        yield return new WaitForSeconds(0.5f);
        isDialogueOpened = true;
    }

    private IEnumerator CloseTutorialDialogueCO()
    {
        tutorialTextBackgroundAnimator.SetTrigger("Close");
        yield return new WaitForSeconds(0.5f);
        isDialogueOpened = false;
    }

    private void SetTutorialDialogueText(string text)
    {
        tutorialText.text = text;
    }

    private IEnumerator ShowSingleText(string text)
    {
        singleTextEl.text = text;
        singleTextPanelAnimator.SetTrigger("Open");
        singleTextElAnimator.SetTrigger("Open");
        yield return new WaitForSeconds(0.5f);


    }

    private IEnumerator SingleTextPanelObjectiveCompleted()
    {
        singleTextPanelAnimator.SetTrigger("Completed");

        if (tutorialStepCompletedAudio)
            SFXManager.GetInstance().PlayClip(tutorialStepCompletedAudio);

        yield return new WaitForSeconds(0.5f);

        yield return CloseSingleText();
    }

    private IEnumerator CloseSingleText()
    {
        singleTextPanelAnimator.SetTrigger("Close");
        singleTextElAnimator.SetTrigger("Close");
        yield return new WaitForSeconds(0.5f);
    }

    private IEnumerator ShowTutorialTextList(string[] textList)
    {
        currentTextIndex = -1;
        currentTextList = textList;
        currentTextListFinished = false;
        StartCoroutine(ShowNextTextLine());

        while (!currentTextListFinished)
            yield return new WaitForSeconds(1.0f);
    }

    private IEnumerator ShowNextTextLine()
    {
        if (isDialogueOpened)
            yield return CloseTutorialDialogueCO();

        currentTextIndex++;
        if (currentTextIndex < currentTextList.Length)
        {
            SetTutorialDialogueText(currentTextList[currentTextIndex]);
            yield return OpenTutorialDialogueCO();
            InputManager.OnTouchStart += TutorialTextReadConfirmed;
        } else
        {
            currentTextListFinished = true;
        }

    }

    private IEnumerator ShowTutorialTitle()
    {
        tutorialTitlePanel.SetTrigger("Open");
        tutorialTitleTextAnimator.SetTrigger("Open");

        if (tutorialTitleOpenSfx)
            SFXManager.GetInstance().PlayClip(tutorialTitleOpenSfx);

        yield return new WaitForSeconds(0.5f);
    }

    private IEnumerator CloseTutorialTitle()
    {
        tutorialTitlePanel.SetTrigger("Close");
        tutorialTitleTextAnimator.SetTrigger("Close");

        if (tutorialTitleOpenSfx)
            SFXManager.GetInstance().PlayClip(tutorialTitleOpenSfx);

        yield return new WaitForSeconds(0.5f);
    }

    private void TutorialTextReadConfirmed()
    {
        InputManager.OnTouchStart -= TutorialTextReadConfirmed;

        if(tutorialTextCloseSfx)
            SFXManager.GetInstance().PlayClip(tutorialTextCloseSfx);

        StartCoroutine(ShowNextTextLine());
    }
}
