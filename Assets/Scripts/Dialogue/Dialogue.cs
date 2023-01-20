using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ink.Runtime;
using System.Globalization;


public struct DialogueChunkData 
{
    public string chunkText;
    public float typingDelay;
    public bool waitInput;
    public bool playVoiceSound;

    public DialogueChunkData(string chunkText)
    {
        this.chunkText = chunkText;
        this.typingDelay = -1f;
        this.waitInput = false;
        this.playVoiceSound = true;
    }
}

public struct DialogueLineData 
{
    public string actor;
    public List<DialogueChunkData> lineTextChunks;
}

public class Dialogue 
{

    private Story _inkStory;
    private DialogueLineData _currentLine;
    private TextAsset _inkTextAsset;
    private bool _partialDialogue;
    private int _startIndex;
    private int _finalIndex;
    private int _currentIndex;
    private DialogueVariables _dialogueVariables;

    // INK TAGS
    private const string NAME_TAG = "name";
    private const string ACTOR_TAG = "actor";
    private const string TYPING_DELAY = "typing_delay";
    private const string CONTINUE_LINE = "continue_line";
    private const string MUTED = "mute_line";


    public Dialogue(TextAsset inkAsset)
    {
        InitializeInkDialogue(inkAsset);
        _partialDialogue = false;
    }

    public Dialogue(TextAsset inkAsset, int startIndex, int finalIndex)
    {
        InitializeInkDialogue(inkAsset);
        _partialDialogue = true;
        _startIndex = startIndex;
        _finalIndex = finalIndex;

        SetCurrentLineByIndex(startIndex);
    }

    public DialogueLineData GetNextLine()
    {
        DialogueLineData thisLineData = new DialogueLineData();
        thisLineData.lineTextChunks = new List<DialogueChunkData>();

        //Debug.Log("get line at index: " + _currentIndex);
        if (_partialDialogue && _currentIndex > _finalIndex)
        {
            //Debug.Log("reached final index: " + _finalIndex);
            _currentLine = thisLineData;
            return thisLineData;
        }
           

        bool lineCompleted;
        do
        {
            if (!_inkStory.canContinue)
                break;

            string dialogueText = _inkStory.Continue().Trim();
            if(dialogueText != "")
            {
                if(thisLineData.actor == "" || thisLineData.actor == null)
                {
                    string[] splitDialogue = dialogueText.Split(':');
                    //Debug.Log(splitDialogue);
                    if(splitDialogue.Length > 1)
                    {
                        thisLineData.actor = splitDialogue[0];
                    }

                    string auxDialogueText = "";
                    for(int i = 1; i < splitDialogue.Length; i++)
                    {
                        if (i > 1) auxDialogueText += ":";
                        auxDialogueText += splitDialogue[i];
                    }

                    dialogueText = auxDialogueText.Trim();
                }

                DialogueChunkData newChunkData = new DialogueChunkData(dialogueText);
                thisLineData.lineTextChunks.Add(newChunkData);
            }

            bool continueLine = HandleTags(_inkStory.currentTags, ref thisLineData);

            lineCompleted = !continueLine && dialogueText != "";

        } while (!lineCompleted);

        _currentIndex++;
        _currentLine = thisLineData;

        return thisLineData;
    }

    public void SetDialogueVariablesInstance(DialogueVariables dialogueVariables)
    {
        Debug.Log("setting dialogueVariable instance");
        _dialogueVariables = dialogueVariables;

        if(_inkStory != null)
            _dialogueVariables.StartListening(_inkStory);
    }

    public DialogueLineData GetCurrentLine()
    {
        return _currentLine;
    }
    public bool CanContinueDialogue()
    {
        return _inkStory.canContinue;
    }

    public void SetCurrentLineByIndex(int index)
    {
        _currentIndex = 0;

        _inkStory = new Story(_inkTextAsset.text);
        for(int i = 0; i < index; i++)
        {
            if (!_inkStory.canContinue)
                return;

            GetNextLine();
        }
    }

    private bool HandleTags(List<string> lineTags, ref DialogueLineData dialogueLineData)
    {
        bool continueLine = false;

        int textChunksCount = dialogueLineData.lineTextChunks.Count;
        DialogueChunkData lastInsertedChunk = dialogueLineData.lineTextChunks[textChunksCount - 1];

        // loop through each tag and handle it accordingly
        foreach (string tag in lineTags)
        {
            // parse the tag
            string[] splitTag = tag.Split(':');
            string tagKey = splitTag[0].Trim().ToLower();
            string tagValue = splitTag.Length > 1 ? splitTag[1].Trim() : "";

            // handle the tag
            switch (tagKey)
            {
                //case NAME_TAG:
                //    dialogueLineData.actorName = tagValue;
                //    break;
                //
                //case ACTOR_TAG:
                //    dialogueLineData.actorId = tagValue;
                //    break;

                case TYPING_DELAY:
                    float typingDelay;
                    if (float.TryParse(tagValue, NumberStyles.Float, CultureInfo.InvariantCulture, out typingDelay))
                    {     
                        lastInsertedChunk.typingDelay = typingDelay;
                        dialogueLineData.lineTextChunks[textChunksCount - 1] = lastInsertedChunk;
                    }
                    else
                    {
                        Debug.LogWarning("Failed to parse typing delay: " + tagValue);
                    }
                    break;
                    ;
                case CONTINUE_LINE:
                    continueLine = true;
                    break;

                case MUTED:
                    lastInsertedChunk.playVoiceSound = false;
                    dialogueLineData.lineTextChunks[textChunksCount - 1] = lastInsertedChunk;
                    break;

                default:
                    Debug.LogWarning("Tag came in but is not currently being handled: " + tag);
                    break;
            }
        }

        return continueLine;
    }

    private void InitializeInkDialogue(TextAsset inkAsset)
    {
        _inkStory = new Story(inkAsset.text);
        _inkTextAsset = inkAsset;
    }

    public void FinishInkDialogue()
    {
        if(_dialogueVariables != null)
            _dialogueVariables.StopListening(_inkStory);
    }
}
