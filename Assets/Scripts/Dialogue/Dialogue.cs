using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Dialogue/Dialogue")]
public class Dialogue : ScriptableObject
{
    [SerializeField] private List<DialogueLine> dialogueLines;

    private int currentLineIndex = 0;

    public void InitDialogue()
    {
        currentLineIndex = 0;
    }

    public bool CanContinue()
    {
        while(currentLineIndex < dialogueLines.Count)
        {
            string lineText = dialogueLines[currentLineIndex].GetLineText();

            Debug.Log("Dialogue CanContinue index " + currentLineIndex + ", lineText: " + lineText);

            // Ignora linhas vazias
            if(lineText != "")
            {
                return true;
            }

            currentLineIndex += 1;
        }

        return false;
    }

    public DialogueLine GetCurrentLineData()
    {
        if (currentLineIndex > dialogueLines.Count - 1)
            currentLineIndex = 0;

        DialogueLine currentLine = dialogueLines[currentLineIndex];
        currentLineIndex += 1;

        return currentLine;
    }
}
