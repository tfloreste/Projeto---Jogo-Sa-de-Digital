using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCInteraction : MonoBehaviour, IInteractable
{

    [SerializeField] TextAsset inkDialogueText;
    [SerializeField] InteractionButton interactionButton;
    
    public void Interact()
    {
        Debug.Log("Npc interact fired");
        Dialogue dialogue = new Dialogue(inkDialogueText);
        DialogueManager.Instance.InitDialogue(dialogue);
        DialogueManager.Instance.StartDialogue();
    }

    public void InInteractionRange()
    {
        interactionButton.Show(this);
    }

    public void OutOfInteractionRange()
    {
        interactionButton.Hide();
    }

}
