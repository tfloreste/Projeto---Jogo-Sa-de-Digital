using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SystemMessageUI : MonoBehaviour
{
    public struct SystemMessageConfig {
        public string message;
        public AudioClip audioToPlay;
        public bool enableSkipingMessage;
        public float messageTime;
    }

    [SerializeField] private GameObject systemMessageUIObject;
    [SerializeField] private TextMeshProUGUI systemTMPText;


    // Start is called before the first frame update
    void Start()
    {
        systemMessageUIObject.SetActive(false);
    }

    public void ShowSystemMessage(string message, AudioClip audioToPlay)
    {
        if (audioToPlay)
            SFXManager.GetInstance().PlayClip(audioToPlay);

        ShowSystemMessage(message);
    }

    public void ShowSystemMessage(string message)
    {
        systemTMPText.text = message;
    }
}
