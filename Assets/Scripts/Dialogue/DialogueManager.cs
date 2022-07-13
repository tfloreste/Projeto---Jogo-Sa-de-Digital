using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using TMPro;
using Ink.Runtime;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class DialogueManager : Singleton<DialogueManager>
{
    [Header("Params")]
    [SerializeField] private float standardTypingDelay = 0.05f;

    [Header("Data")]
    [SerializeField] private DialogueActorProvider actorProvider = null;

    [Header("Dialogue UI")]
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private Animator dialoguePanelAnimator;
    [SerializeField] private TextMeshProUGUI characterName;
    [SerializeField] private TextMeshProUGUI dialogueText;

    [Header("Dialogue Events")]
    [SerializeField] private GameEvent dialogueStartedEvent;
    [SerializeField] private GameEvent dialogEndedEvent;

    private float typingDelay;
    private bool isNewSentenceLine = true;
    private Dialogue currentDialogue = null;
    private bool isTyping = false;
    private string currentActorName = null;
    private AudioClip currentActorVoice = null;
    private AudioSource audioSource = null;

    private enum DialogueState { OPEN, CLOSED };
    private DialogueState currentDialogueState;
    private bool actorChanged = false;


    // INK TAGS
    private const string NAME_TAG = "name";
    private const string TYPING_DELAY = "typing_delay";
    private const string CONTINUE_LINE = "continue_line";

    private void Start()
    {
        dialoguePanel.SetActive(false);
        typingDelay = standardTypingDelay;
        audioSource = GetComponent<AudioSource>();

        currentDialogueState = DialogueState.CLOSED;
    }

    //public void EnterDialogueMode(TextAsset inkJSON)
    //{
    //    Story story = new Story(inkJSON.text);
    //    dialoguePanel.SetActive(true);
    //    dialogueText.text = "";
    //
    //    dialogueStartedEvent?.Invoke();
    //
    //    StartCoroutine(ShowDialogue(story));
    //}

    private IEnumerator ExitDialogueMode()
    {
        yield return InputDelay();
    
        //dialoguePanel.SetActive(false);
        //dialogueText.text = "";
        currentDialogue = null;
        if (currentDialogueState == DialogueState.OPEN)
            CloseDialogue(null);
    
        dialogEndedEvent?.Invoke();
    }

    //private IEnumerator ShowDialogue(Story story)
    //{
    //    while(story.canContinue)
    //    {
    //        string nextSentence = story.Continue().TrimEnd();
    //
    //        ClearTagsEffects();
    //        HandleTags(story.currentTags);
    //
    //        yield return InputDelay();
    //        yield return TypeSentence(nextSentence);
    //
    //        yield return InputDelay();
    //        yield return WaitForInput();
    //    }
    //    
    //    StartCoroutine(ExitDialogueMode());
    //}

    private bool PlayerInputDetected() => Input.touchCount > 0;

    private IEnumerator WaitForInput()
    {
        while (!PlayerInputDetected())
        {
            yield return null;
        }
    }

    private IEnumerator InputDelay()
    {
        yield return new WaitForSeconds(0.2f);
    }

    private void SetActorName(DialogueLineData lineData)
    {
        string newActorName = "";
        currentActorVoice = null;
        if (lineData.actor != "" && lineData.actor != null && actorProvider != null)
        {
            DialogueActor actor = actorProvider.GetActor(lineData.actor);
            if(actor != null)
            {
                newActorName = actor.actorName;
                currentActorVoice = actor.voiceSoundEffect;
            }
        } 

        if(newActorName == "")
        {
            newActorName = lineData.actor;
        }

        actorChanged = (newActorName != currentActorName);
        if(actorChanged)
        {
            currentActorName = newActorName;
        }

        characterName.text = newActorName;

    }

    public bool IsTypingDialogue()
    {
        return this.isTyping;
    }

    public void InitDialogue(Dialogue dialogue)
    {
        currentDialogue = dialogue;

        //dialoguePanel.SetActive(true);
        //dialogueText.text = "";

        dialogueStartedEvent?.Invoke();
    }

    public bool SetDialogueOnTime(double elapsedTime, bool ignoreTypingDelay)
    {
        if (currentDialogue == null)
            return false;

        DialogueLineData lineData = currentDialogue.GetCurrentLine();
        if (lineData.lineTextChunks.Count == 0)
            return true;

        float currentDialogueTime = 0.0f;
        string currentText = "";
        int numVisibleChars = 0;

        foreach(var lineChunk in lineData.lineTextChunks)
        {
            if (currentText != "")
                currentText += " ";

            currentText += lineChunk.chunkText;
            float chunkTypingDelay = standardTypingDelay;
            if (!ignoreTypingDelay && lineChunk.typingDelay > 0)
                chunkTypingDelay = lineChunk.typingDelay;

            foreach (char letter in lineChunk.chunkText.ToCharArray())
            {
                numVisibleChars++;
                currentDialogueTime += chunkTypingDelay;

                if (currentDialogueTime >= elapsedTime)
                    break;
            }

            if (currentDialogueTime >= elapsedTime)
                break;
        }

        dialogueText.text = currentText;
        dialogueText.maxVisibleCharacters = numVisibleChars;
        SetActorName(lineData);

        if (currentDialogueState == DialogueState.CLOSED 
            || (actorChanged && Mathf.Approximately((float)elapsedTime, 0.0f)))
        {
            StartCoroutine(PerformDialogueUITransition());
        }

        return currentText.Length <= numVisibleChars; // Retornar true se o dialogo estiver completo
    }

    public void SetDialogueProgress(double progress, bool ignoreTypingDelay)
    {
        if (currentDialogue == null)
            return;

        DialogueLineData lineData = currentDialogue.GetCurrentLine();
        if (lineData.lineTextChunks.Count == 0)
            return;

        if (progress >= 1.0f)
            progress = 1.0f;
        else if (progress <= 0)
            return;

        float totalTime = 0.0f;
        foreach (var lineChunk in lineData.lineTextChunks)
        {
            float delay = standardTypingDelay;
            if (!ignoreTypingDelay && lineChunk.typingDelay > 0)
                delay = lineChunk.typingDelay;
            
            totalTime += lineChunk.chunkText.Length  * delay;
        }

        double elapsedTime = totalTime * progress;
        SetDialogueOnTime(elapsedTime, ignoreTypingDelay);

    }

    public bool PrepareNextLine()
    {
        if (currentDialogue == null)
            return false;

        if (!currentDialogue.CanContinueDialogue())
            return false;

        DialogueLineData lineData = currentDialogue.GetNextLine();
        if (lineData.lineTextChunks.Count == 0)
            return false;

        return true;
    }

    public bool AdvanceDialogue()
    {
        if (currentDialogue == null)
            return false;

        if (!PrepareNextLine())
        {
            StartCoroutine(ExitDialogueMode());
            return false;
        }

        if (!currentDialogue.CanContinueDialogue())
        {
            StartCoroutine(ExitDialogueMode());
            return false;
        }

        DialogueLineData lineData = currentDialogue.GetCurrentLine();
        StartCoroutine(ShowLine(lineData));
        return true;
    }

    private IEnumerator ShowLine(DialogueLineData lineData)
    {
        this.isTyping = true;
        
        SetActorName(lineData);
        if (actorChanged || currentDialogueState == DialogueState.CLOSED)
            yield return PerformDialogueUITransition();
  
        this.isNewSentenceLine = true;
        foreach (DialogueChunkData lineChunk in lineData.lineTextChunks)
        {
            this.typingDelay = lineChunk.typingDelay > 0 ? lineChunk.typingDelay : standardTypingDelay;
            yield return InputDelay();
            yield return StartCoroutine(TypeSentence(lineChunk.chunkText));
            isNewSentenceLine = false;
        }

        yield return InputDelay();
        this.isTyping = false;
    }


    private IEnumerator TypeSentence(string sentence)
    {
        Debug.Log("Starting typeSentence with delay: " + typingDelay);

        if (this.isNewSentenceLine)
            dialogueText.text = "";

        int prevCharCount = dialogueText.text.Length;

        dialogueText.text += sentence;
        dialogueText.maxVisibleCharacters = prevCharCount;

        foreach(char letter in sentence.ToCharArray())
        {
            if (PlayerInputDetected())
            {
                dialogueText.maxVisibleCharacters = dialogueText.text.Length;
                break;
            }

            dialogueText.maxVisibleCharacters++;
            yield return new WaitForSeconds(this.typingDelay);
        }

    }


    public void OpenDialogue(Action callbackFunc)
    {
        dialoguePanelAnimator.SetBool("isOpen", true);
        currentDialogueState = DialogueState.OPEN;

        if (callbackFunc != null)
            callbackFunc();
    }

    public IEnumerator OpenDialogue()
    {
        dialoguePanel.SetActive(true);
        dialoguePanelAnimator.SetBool("isOpen", true);
        currentDialogueState = DialogueState.OPEN;

        yield return new WaitForSeconds(0.15f);
    }

    public void CloseDialogue(Action callbackFunc)
    {   
        // Condição para evitar erros
        if (!dialoguePanel.activeSelf)
            dialoguePanel.SetActive(true);

        dialoguePanelAnimator.SetBool("isOpen", false);
        currentDialogueState = DialogueState.CLOSED;

        if (callbackFunc != null)
            callbackFunc();
    }

    public IEnumerator CloseDialogue()
    {
        dialoguePanelAnimator.SetBool("isOpen", false);
        currentDialogueState = DialogueState.CLOSED;

        yield return new WaitForSeconds(0.15f);
        dialoguePanel.SetActive(false);
    }


    public IEnumerator PerformDialogueUITransition()
    {
        if (currentDialogueState == DialogueState.OPEN)
            yield return CloseDialogue();

        yield return OpenDialogue();
    }

    public void PlayActorVoice()
    {
        if (currentActorVoice == null || audioSource == null)
            return;

        audioSource.clip = currentActorVoice;
        audioSource.Play();
    }

}
