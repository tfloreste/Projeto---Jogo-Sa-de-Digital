using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    private TextAsset _inkDialogue;
    private int _startIndex = 0;
    private int _finalIndex = 9999;
    private List<BoolVariable> conditions;

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

    public void StartDialogue(TextAsset inkDialogue)
    {
        Dialogue dialogue = new Dialogue(inkDialogue);
        DialogueManager.Instance.InitDialogue(dialogue);
        DialogueManager.Instance.StartDialogue();
    }

    public void StartDialogueIfConditionsMet()
    {
        if (!AreConditionsMet())
            return;

        StartDialogue();
    }

    public void StartDialogueIfConditionsMet(TextAsset inkDialogue)
    {
        if (!AreConditionsMet())
            return;

        StartDialogue(inkDialogue);
    }

    public void ClearConditions()
    {
        if (conditions == null)
            conditions = new List<BoolVariable>();

        conditions.Clear();
    }

    public void AddCondition(BoolVariable condition)
    {
        if (conditions == null)
            conditions = new List<BoolVariable>();

        conditions.Add(condition);
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

    private bool AreConditionsMet()
    {
        if (conditions == null)
            return true;

        foreach (BoolVariable condition in conditions)
        {
            if (!condition.Value)
                return false;
        }

        return true;
    }
}
