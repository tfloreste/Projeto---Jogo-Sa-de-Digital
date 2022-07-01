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

    // INK TAGS
    private const string NAME_TAG = "name";
    private const string TYPING_DELAY = "typing_delay";
    private const string CONTINUE_LINE = "continue_line";

    private void Start()
    {
        dialoguePanel.SetActive(false);
        typingDelay = standardTypingDelay;
    }

    public void EnterDialogueMode(TextAsset inkJSON)
    {
        Story story = new Story(inkJSON.text);
        dialoguePanel.SetActive(true);
        dialogueText.text = "";

        dialogueStartedEvent?.Invoke();

        StartCoroutine(ShowDialogue(story));
    }

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

    private IEnumerator ExitDialogueMode()
    {
        yield return InputDelay();

        dialoguePanel.SetActive(false);
        dialogueText.text = "";
        currentDialogue = null;

        dialogEndedEvent?.Invoke();
    }

    private IEnumerator ShowDialogue(Story story)
    {
        while(story.canContinue)
        {
            string nextSentence = story.Continue().TrimEnd();

            ClearTagsEffects();
            HandleTags(story.currentTags);

            yield return InputDelay();
            yield return TypeSentence(nextSentence);

            yield return InputDelay();
            yield return WaitForInput();
        }
        
        StartCoroutine(ExitDialogueMode());
    }

    public bool IsTypingDialogue()
    {
        return this.isTyping;
    }

    public void InitDialogue(Dialogue dialogue)
    {
        currentDialogue = dialogue;

        dialoguePanel.SetActive(true);
        dialogueText.text = "";

        dialogueStartedEvent?.Invoke();
    }

    public void SetDialogueOnTime(double elapsedTime, bool ignoreTypingDelay)
    {
        if (currentDialogue == null)
            return;

        DialogueLineData lineData = currentDialogue.GetCurrentLine();
        if (lineData.lineTextChunks.Count == 0)
            return;

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
        characterName.text = lineData.actorName;
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

        Debug.Log("setting dialogue progress: " + progress.ToString());

        float totalTime = 0.0f;
        foreach (var lineChunk in lineData.lineTextChunks)
        {
            float delay = standardTypingDelay;
            if (!ignoreTypingDelay && lineChunk.typingDelay > 0)
                delay = lineChunk.typingDelay;
            
            totalTime += lineChunk.chunkText.Length  * delay;
        }

        double elapsedTime = totalTime * progress;
        Debug.Log("Total Time: " + totalTime.ToString() + " | elapsedTime: " + elapsedTime.ToString());
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

        characterName.text = lineData.actorName;
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

    private void ClearTagsEffects()
    {
        characterName.text = "???";
        isNewSentenceLine = true;
        typingDelay = standardTypingDelay;
    }

    private void HandleTags(List<string> lineTags)
    {
        // loop through each tag and handle it accordingly
        foreach (string tag in lineTags)
        {
            // parse the tag
            string[] splitTag = tag.Split(':');
            if (splitTag.Length != 2)
            {
                Debug.LogError("Tag could not be appropriately parsed: " + tag);
            }
            string tagKey = splitTag[0].Trim().ToLower();
            string tagValue = splitTag[1].Trim();

            // handle the tag
            switch (tagKey)
            {
                case NAME_TAG:
                    characterName.text = tagValue;
                    break;

                case TYPING_DELAY:
                    Debug.Log("Tag TYPING_DELAY detectada com valor: " + tagValue);
                    if(!float.TryParse(tagValue, NumberStyles.Float, CultureInfo.InvariantCulture, out typingDelay))
                    {
                        typingDelay = standardTypingDelay;
                        Debug.LogWarning("Failed to parse typing delay: " + tagValue);
                    }
                    break;

                case CONTINUE_LINE:
                    Debug.Log("Tag CONTINUE_LINE detectada");
                    isNewSentenceLine = false;
                    dialogueText.text += " "; 
                    break;

                default:
                    Debug.LogWarning("Tag came in but is not currently being handled: " + tag);
                    break;
            }
        }
    }

    public void OpenDialogue(Action callbackFunc)
    {
        dialoguePanelAnimator.SetBool("isOpen", true);

        if (callbackFunc != null)
            callbackFunc();
    }

    public void CloseDialogue(Action callbackFunc)
    {
        dialoguePanelAnimator.SetBool("isOpen", false);

        if (callbackFunc != null)
            callbackFunc();
    }

}
