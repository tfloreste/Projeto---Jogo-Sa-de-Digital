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

    public DialogueChunkData(string chunkText)
    {
        this.chunkText = chunkText;
        this.typingDelay = -1f;
        this.waitInput = false;
    }
}

public struct DialogueLineData 
{
    public string actorName;
    public List<DialogueChunkData> lineTextChunks;
}

public class Dialogue 
{

    private Story _inkStory;
    private DialogueLineData _currentLine;
    private TextAsset _inkTextAsset;

    // INK TAGS
    private const string NAME_TAG = "name";
    private const string TYPING_DELAY = "typing_delay";
    private const string CONTINUE_LINE = "continue_line";


    public Dialogue(TextAsset inkAsset)
    {
        _inkStory = new Story(inkAsset.text);
        _inkTextAsset = inkAsset;
    }

    public DialogueLineData GetNextLine()
    {
        DialogueLineData thisLineData = new DialogueLineData();
        thisLineData.lineTextChunks = new List<DialogueChunkData>();

        bool lineCompleted;
        do
        {
            if (!_inkStory.canContinue)
                break;

            string dialogueText = _inkStory.Continue().TrimEnd();
            if(dialogueText != "")
            {
                Debug.Log("Creating new chunkText with dialog: " + dialogueText);
                DialogueChunkData newChunkData = new DialogueChunkData(dialogueText);
                thisLineData.lineTextChunks.Add(newChunkData);
            }

            bool continueLine = HandleTags(_inkStory.currentTags, ref thisLineData);

            lineCompleted = !continueLine && dialogueText != "";

        } while (!lineCompleted);

        _currentLine = thisLineData;

        return thisLineData;
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
                    dialogueLineData.actorName = tagValue;
                    break;

                case TYPING_DELAY:
                    float typingDelay;
                    if (float.TryParse(tagValue, NumberStyles.Float, CultureInfo.InvariantCulture, out typingDelay))
                    {
                        int textChunksCount = dialogueLineData.lineTextChunks.Count;
                        DialogueChunkData lastInsertedChunk = dialogueLineData.lineTextChunks[textChunksCount - 1];
                        lastInsertedChunk.typingDelay = typingDelay;
                        
                        dialogueLineData.lineTextChunks[textChunksCount - 1] = lastInsertedChunk;

                    }
                    else
                    {
                        Debug.LogWarning("Failed to parse typing delay: " + tagValue);
                    }
                    break;

                case CONTINUE_LINE:
                    continueLine = true;
                    break;

                default:
                    Debug.LogWarning("Tag came in but is not currently being handled: " + tag);
                    break;
            }
        }

        return continueLine;
    }
}
