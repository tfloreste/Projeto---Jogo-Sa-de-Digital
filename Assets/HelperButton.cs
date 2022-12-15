using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelperButton : MonoBehaviour
{
    [SerializeField] private TextAsset inkHelperJson;
    public void ShowHelpText()
    {
        Debug.Log("ShowHelpText fired");
        Debug.Log("isInDialogueMode: " + DialogueManager.Instance.isInDialogueMode);
        if (DialogueManager.Instance.isInDialogueMode)
            return;

        Dialogue dialogue = new Dialogue(inkHelperJson);
        DialogueManager.Instance.InitDialogue(dialogue);
        DialogueManager.Instance.StartDialogue();
    }
}
