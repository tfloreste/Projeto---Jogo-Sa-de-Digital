using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HelperButton : MonoBehaviour
{
    [SerializeField] private TextAsset inkHelperJson;

    private Image btnImage;
    private Text btnText;

    private void Awake()
    {
        Debug.Log("helperBtn start fired");
        btnImage = GetComponent<Image>();
        btnText = transform.GetChild(0).GetComponent<Text>();
    }

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

    public void Hide()
    {
        Color btnColor = btnImage.color;
        Color textColor = btnText.color;

        btnImage.color = new Color(btnColor.r, btnColor.g, btnColor.b, 0.0f);
        btnText.color = new Color(textColor.r, textColor.g, textColor.b, 0.0f);
    }

    public void Show()
    {
        Color btnColor = btnImage.color;
        Color textColor = btnText.color;

        btnImage.color = new Color(btnColor.r, btnColor.g, btnColor.b, 1.0f);
        btnText.color = new Color(textColor.r, textColor.g, textColor.b, 1.0f);

    }
}
