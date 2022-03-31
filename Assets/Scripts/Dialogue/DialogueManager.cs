using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Ink.Runtime;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class DialogueManager : MonoBehaviour
{

    [Header("Dialogue UI")]
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private TextMeshProUGUI characterName;
    [SerializeField] private TextMeshProUGUI dialogueText;

    [Header("Dialogue Events")]
    [SerializeField] private GameEvent dialogueStartedEvent;
    [SerializeField] private GameEvent dialogEndedEvent;


    // INK TAGS
    private const string NAME_TAG = "name";

    public static DialogueManager instance;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("Found more than one Dialogue Manager in the scene: " + SceneManager.GetActiveScene().name);
        }
        instance = this;
    }

    private void Start()
    {
        dialoguePanel.SetActive(false);
    }

    public void EnterDialogueMode(TextAsset inkJSON)
    {
        Story story = new Story(inkJSON.text);
        dialoguePanel.SetActive(true);

        dialogueStartedEvent?.Invoke();

        StartCoroutine(ShowDialogue(story));
    }

    private IEnumerator ExitDialogueMode()
    {
        yield return new WaitForSeconds(0.2f);

        dialoguePanel.SetActive(false);
        dialogueText.text = "";

        dialogEndedEvent?.Invoke();
    }

    private IEnumerator ShowDialogue(Story story)
    {
        while(story.canContinue)
        {
            dialogueText.text = story.Continue().Trim();
            HandleTags(story.currentTags);

            yield return new WaitForSeconds(0.2f);
            
            while(Input.touchCount == 0)
            {
                yield return null;
            }
        }
        
        StartCoroutine(ExitDialogueMode());
    }


    private void HandleTags(List<string> lineTags)
    {
        // loop through each tag and handle it accordingly
        foreach (string tag in lineTags)
        {
            // parse the tag
            string[] splitTag = tag.Split(':');
            if (splitTag.Length != 2)
            {
                Debug.LogError("Tag could not be appropriately parsed: " + tag);
            }
            string tagKey = splitTag[0].Trim();
            string tagValue = splitTag[1].Trim();

            // handle the tag
            switch (tagKey)
            {
                case NAME_TAG:
                    characterName.text = tagValue;
                    break;
                default:
                    Debug.LogWarning("Tag came in but is not currently being handled: " + tag);
                    break;
            }
        }
    }


}
