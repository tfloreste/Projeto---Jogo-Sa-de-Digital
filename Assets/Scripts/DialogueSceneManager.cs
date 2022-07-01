using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ink.Runtime;

public class DialogueSceneManager : MonoBehaviour
{
    public TextAsset dialogueToPlay = null;

    private bool dialogueStarted = false;
    private bool isDialogueFinished = false;

    void Start()
    {
        if (dialogueToPlay != null)
        {
            Invoke("StartSceneDialog", 0.5f);
        } 
    }

    void StartSceneDialog()
    {
        Dialogue dialogue = new Dialogue(dialogueToPlay);
        DialogueManager.Instance.InitDialogue(dialogue);
        DialogueManager.Instance.AdvanceDialogue();
        dialogueStarted = true;
        //DialogueManager.instance.EnterDialogueMode(dialogToPlay);
    }


    private void Update()
    {
        if(dialogueStarted && !isDialogueFinished && !DialogueManager.Instance.IsTypingDialogue() && Input.touchCount > 0)
        {
            isDialogueFinished = !DialogueManager.Instance.AdvanceDialogue();
            Debug.Log("Trying to show next line. Dialogue Finished: " + (isDialogueFinished ? "True" : "False"));
        }
    }

}
