using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    [SerializeField] private TextAsset _inkDialogue;
    [SerializeField] private int _startIndex = 0;
    [SerializeField] private int _finalIndex = 9999;

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
