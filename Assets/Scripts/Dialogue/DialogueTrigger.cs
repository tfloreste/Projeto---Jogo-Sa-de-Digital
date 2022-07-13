using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    [SerializeField] TextAsset inkDialogue;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            Debug.Log("Player entered trigger");
            //DialogueManager.Instance.EnterDialogueMode(inkDialogue);
        }
    }
}
