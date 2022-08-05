using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    private TextAsset _inkDialogue;
    private int _startIndex = 0;
    private int _finalIndex = 9999;

    /**
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            Debug.Log("Player entered trigger");
            //DialogueManager.Instance.EnterDialogueMode(inkDialogue);
        }
    }
    */

   /* private void Start()
    {
        DialogueManager.Instance.MoveDialoguePainelUp();
        StartDialogue();
    }*/

    public void StartDialogue()
    {
        Debug.Log("startDialogue with startIndex = " + _startIndex + " and finalIndex = " + _finalIndex);
        Dialogue dialogue = new Dialogue(_inkDialogue, _startIndex, _finalIndex);
        DialogueManager.Instance.InitDialogue(dialogue);
        DialogueManager.Instance.StartDialogue();
    }

    public void SetStartIndex(int index)
    {
        _startIndex = index;
    }

    public void SetFinalIndex(int index)
    {
        _finalIndex = index;
    }

    public void SetInkDialogue(TextAsset inkDialogue)
    {
        _inkDialogue = inkDialogue;
    }
}
