using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using TMPro;
using Ink.Runtime;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class DialogueManager : Singleton<DialogueManager>, IDataPersistence
{
    [Header("Params")]
    [SerializeField] private float standardTypingDelay = 0.05f;
    [SerializeField] private float timeBetweenVoiceEffect = 0.15f;
    [SerializeField] private float dialoguePainelUpperVerticalPosition = 132.0f;
    [SerializeField] private float dialoguePainelBottomVerticalPosition = -115.0f;

    [Header("Data")]
    [SerializeField] private DialogueActorProvider actorProvider = null;
    [SerializeField] private TextAsset globalDialogueVariableJson = null;

    [Header("Dialogue UI")]
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private Animator dialoguePanelAnimator;
    [SerializeField] private TextMeshProUGUI characterName;
    [SerializeField] private TextMeshProUGUI dialogueText;

    [Header("Dialogue Events")]
    [SerializeField] private GameEvent dialogueStartedEvent;
    [SerializeField] private GameEvent dialogEndedEvent;

    [HideInInspector] public bool isInDialogueMode = false;

    private float typingDelay;
    //private bool isNewSentenceLine = true;
    private Dialogue currentDialogue = null;
    private bool isTyping = false;
    private string currentActorName = null;
    private AudioClip currentActorVoice = null;
    private AudioSource audioSource = null;
    private DialogueVariables dialogueVariables = null;

    private enum DialogueState { OPEN, CLOSED };
    private DialogueState currentDialogueState;
    private bool actorChanged = false;
    private bool completedTypingByTouch = false;

    private void Awake()
    {
        currentDialogueState = DialogueState.CLOSED;
        dialogueVariables = new DialogueVariables(globalDialogueVariableJson);


    }
    private void Start()
    {
        if(dialoguePanel)
        {
            dialoguePanel.SetActive(false);
            typingDelay = standardTypingDelay;
            audioSource = GetComponent<AudioSource>();

            dialoguePanel.transform.localScale = new Vector3(0, 0, 0);
        }
    }

    public void ExitDialogueMode()
    {
        StartCoroutine(ExitDialogueModeCO());
    }

    private IEnumerator ExitDialogueModeCO()
    {
        Debug.Log("Exiting dialogue started");
        if (currentDialogue == null)
            yield break;
        //yield return InputDelay();

        //dialoguePanel.SetActive(false);
        //dialogueText.text = "";
        currentDialogue.FinishInkDialogue();
        currentDialogue = null;

        if (currentDialogueState == DialogueState.OPEN)
            yield return CloseDialogue();

        dialogEndedEvent?.Invoke();
        isInDialogueMode = false;
        Debug.Log("Exiting dialogue ended");
    }

    private bool PlayerInputDetected() => Input.touchCount > 0;

    private IEnumerator WaitForInput()
    {
        while (!PlayerInputDetected())
        {
            yield return null;
        }
    }

    private void SetActorName(DialogueLineData lineData)
    {
        string newActorName = "";
        currentActorVoice = null;
        if (lineData.actor != "" && lineData.actor != null && actorProvider != null)
        {
            DialogueActor actor = actorProvider.GetActor(lineData.actor);
            if (actor != null)
            {
                newActorName = actor.actorName;
                currentActorVoice = actor.voiceSoundEffect;
            }
        }

        if (newActorName == "")
        {
            newActorName = lineData.actor;
        }

        actorChanged = (newActorName != currentActorName);
        if (actorChanged)
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
        Debug.Log("Init dialogue started");
        currentDialogue = dialogue;
        currentDialogue.SetDialogueVariablesInstance(dialogueVariables);

        //dialoguePanel.SetActive(true);
        //dialogueText.text = "";

        dialogueStartedEvent?.Invoke();
        Debug.Log("Init dialogue ended");
    }

    public void StartDialogue()
    {
        if (currentDialogue == null)
            return;

        isInDialogueMode = true;
        AdvanceDialogue();
    }

    private void AdvanceDialogueOnTouch()
    {
        InputManager.OnTouchStart -= AdvanceDialogueOnTouch;
        AdvanceDialogue();
    }

    private void CompleteDialogueMessageOnTouch()
    {
        dialogueText.maxVisibleCharacters = dialogueText.text.Length;
        completedTypingByTouch = true;

        InputManager.OnTouchStart -= CompleteDialogueMessageOnTouch;
        InputManager.OnTouchStart += AdvanceDialogueOnTouch;
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

        foreach (var lineChunk in lineData.lineTextChunks)
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

            totalTime += lineChunk.chunkText.Length * delay;
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
            StartCoroutine(ExitDialogueModeCO());
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

        //this.isNewSentenceLine = true;
        dialogueText.text = "";

        if (actorChanged || currentDialogueState == DialogueState.CLOSED)
            yield return PerformDialogueUITransition();

        foreach (DialogueChunkData lineChunk in lineData.lineTextChunks)
        {
            this.typingDelay = lineChunk.typingDelay > 0 ? lineChunk.typingDelay : standardTypingDelay;
            //yield return InputDelay();
            yield return StartCoroutine(TypeSentence(lineChunk));
            //isNewSentenceLine = false;
        }

        //yield return InputDelay();
        //this.isTyping = false;
    }


    private IEnumerator TypeSentence(DialogueChunkData lineChunk)
    {
        int prevCharCount = dialogueText.text.Length;
        string sentence = lineChunk.chunkText;

        dialogueText.text += sentence;
        dialogueText.maxVisibleCharacters = prevCharCount;

        completedTypingByTouch = false;
        InputManager.OnTouchStart += CompleteDialogueMessageOnTouch;

        float voiceEffectTimer = -1f;

        bool processingTag = false;
        foreach (char letter in sentence.ToCharArray())
        {
            // Abertura de tag
            if(letter == '<')
            {
                processingTag = true;
            }

            // Se estiver processando uma tag, pula para a proxima letra
            if (processingTag)
            {
                // Ultimo char da tag
                if (letter == '>')
                    processingTag = false;

                continue;
            }

            float timer = 0.0f;
            while (true)
            {
                if (dialogueText.maxVisibleCharacters == dialogueText.text.Length)
                    break;

                if (lineChunk.playVoiceSound) 
                {
                    bool canPlayVoice = (voiceEffectTimer < 0 || voiceEffectTimer >= timeBetweenVoiceEffect);
                    if(canPlayVoice)
                    {
                        PlayActorVoice();
                        voiceEffectTimer = 0.0f;
                    }
                }

                yield return null;

                timer += Time.deltaTime;

                if (lineChunk.playVoiceSound)
                    voiceEffectTimer += Time.deltaTime;

                if (timer >= this.typingDelay)
                    break;
            }

            if (dialogueText.maxVisibleCharacters == dialogueText.text.Length)
                break;

            dialogueText.maxVisibleCharacters++;
        }

        if (!completedTypingByTouch)
        {
            InputManager.OnTouchStart -= CompleteDialogueMessageOnTouch;
            InputManager.OnTouchStart += AdvanceDialogueOnTouch;
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
        //dialoguePanel.SetActive(false);
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

    public void MoveDialoguePainelUp()
    {
        ChangeDialoguePanelVerticalPosition(dialoguePainelUpperVerticalPosition);
    }

    public void MoveDialoguePainelDown()
    {
        ChangeDialoguePanelVerticalPosition(dialoguePainelBottomVerticalPosition);
    }


    public void ChangeDialoguePanelVerticalPosition(float newVerticalPosition)
    {

        Vector3 currentPosition = dialoguePanel.transform.localPosition;
        Vector3 newPosition = new Vector3(currentPosition.x, newVerticalPosition, currentPosition.z);
        dialoguePanel.transform.localPosition = newPosition;
    }

    public void LoadData(GameData data)
    {
        Debug.Log("DialogueManager loaddata fired");
        if (dialogueVariables != null && data.dialogueVariablesJsonState != null && data.dialogueVariablesJsonState != "")
            dialogueVariables.LoadJsonState(data.dialogueVariablesJsonState);

        Debug.Log("DialogueManager loaddata completed");
    }

    public void SaveData(GameData data)
    {
        Debug.Log("DialogueManager saving game");
        if (dialogueVariables != null)
            data.dialogueVariablesJsonState = dialogueVariables.GetJsonState();
    }

    public void SetDialogueVariable<T>(string key, T value)
    {
        Debug.Log("DialogueManager setting ink variable: " + key + ": " + value);
        if (dialogueVariables != null)
            dialogueVariables.SetVariable(key, value);
    }

    public T GetDialogueVariable<T>(string key)
    {
        if (dialogueVariables != null)
            return dialogueVariables.GetVariable<T>(key);

        return default(T);
    }

}
