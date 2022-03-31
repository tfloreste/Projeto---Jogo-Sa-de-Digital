using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using Ink.Runtime;
using UnityEngine.EventSystems;


public class DialogueManager : MonoBehaviour
{
    public static DialogueManager instance = null;

    [Header("Dialogue UI")]
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private TextMeshProUGUI characterName;
    [SerializeField] private TextMeshProUGUI dialogueText;

    [Header("Dialogue Events")]
    [SerializeField] private GameEvent dialogueStartedEvent;
    [SerializeField] private GameEvent dialogEndedEvent;

    //[Header("Choices UI")]
    //[SerializeField] private GameObject[] choices;
    //private TextMeshProUGUI[] choicesText;


    private void Awake()
    {
        if(instance != null)
        {
            Debug.LogError("Mais de um DialogueManager criado na cena: " + SceneManager.GetActiveScene().name);
            //Destroy(this);
        }

        instance = this;
    }

    private void Start()
    {
        dialoguePanel.SetActive(false);
    }

    public void EnterDialogueMode(Dialogue dialogue)
    {
        dialoguePanel.SetActive(true);
        dialogueStartedEvent?.Invoke();

        StartCoroutine(ShowDialogue(dialogue));
    }

    private IEnumerator ShowDialogue(Dialogue dialogue)
    {
        Debug.Log("ShowDialogue fired");
        dialogue.InitDialogue();

        while (dialogue.CanContinue())
        {
            DialogueLine dialogueLineData = dialogue.GetCurrentLineData();
            DialogueCharacter dialogueCharacterData = dialogueLineData.GetCharacterData();

            dialogueText.text = dialogueLineData.GetLineText();
            characterName.text = dialogueCharacterData.characterName;

            yield return new WaitForSeconds(0.2f);
            while(Input.touchCount == 0)
            {
                yield return null;
            }

            //yield return Input.GetKeyDown(KeyCode.Space);
        }

        yield return new WaitForSeconds(0.2f);
        Debug.Log("ShowDialogue ended");

        ExitDialogueMode();
    }


    private void ExitDialogueMode()
    {
        
        dialoguePanel.SetActive(false);
        dialogueText.text = "";

        dialogEndedEvent?.Invoke();
    }

}
