using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Events;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(AudioSource))]
public class SystemMessageUI : MonoBehaviour
{
    public struct SystemMessageConfig {
        public AudioClip audioToPlay;
        public bool enableSkipingMessage;
        public float typingDelay;
    }

    [SerializeField] private TextMeshProUGUI systemTMPText;
    [SerializeField] private GameObject canContinueIndicator;

    [Header("Audio")]
    [SerializeField] private AudioClip gotItemAudioClip;
    [SerializeField] private AudioClip openUIClip;
    [SerializeField] private AudioClip closeUIClip;


    public event UnityAction onSystemMessageShown;
    public event UnityAction onSystemMessageClosed;

    private Animator systemMessageUIAnimator;
    private AudioSource audioSource;
    private SystemMessageConfig currentConfig;

    private float openUIAnimationDuration = 0.5f;
    private float closeUIAnimationDuration = 0.5f;

    private bool audioEffectFinished;

    private void Start()
    {
        systemMessageUIAnimator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        systemTMPText.text = "";

        // Setando as configurações das mensagens para 
        // o tipo de mensagem de "item" de forma hardcoded.
        //
        // Caso outro tipo de System Message seja implementado,
        // talvez o ideal seria fazer ScriptableObjects com as diferentes
        // configurações, e usar algo como é feito no script DialogueActorProvider
        // para obter a configuração correta de acordo com o tipo da mensagem.
        //
        // A opção enableSkipingMessage por enquanto não está implementada
        // mas há uma implementação equivalente no DialogueManager
        // dentro do método TypeSentece (ver a subscrição do método
        // "CompleteDialogueMessageOnTouch")
        currentConfig = new SystemMessageConfig();
        currentConfig.enableSkipingMessage = false; 
        currentConfig.typingDelay = 0.1f;
        currentConfig.audioToPlay = gotItemAudioClip;


        canContinueIndicator.SetActive(false);
        onSystemMessageShown += () => canContinueIndicator.SetActive(true);
        onSystemMessageClosed += () => canContinueIndicator.SetActive(false);
    }

    public void ShowSystemMessage(string message)
    {
        systemTMPText.text = "";
        StartCoroutine(ShowSystemMessageCO(message));
    }

    private IEnumerator ShowSystemMessageCO(string message)
    {
        systemMessageUIAnimator.SetTrigger("Open");

        if (openUIClip)
            SFXManager.GetInstance().PlayClip(openUIClip);

        yield return new WaitForSeconds(openUIAnimationDuration);

        if (currentConfig.audioToPlay)
        {
            StartCoroutine(PlayAudioEffect());
        }

        yield return TypeMessage(message);

        if (currentConfig.audioToPlay && !audioEffectFinished)
            yield return new WaitUntil(() => audioEffectFinished);

        onSystemMessageShown?.Invoke();

        InputManager.OnTouchStart += CloseSystemMessageOnInput;
    }

    private IEnumerator PlayAudioEffect()
    {
        audioEffectFinished = false;
        audioSource.PlayOneShot(currentConfig.audioToPlay);
        yield return new WaitForSeconds(currentConfig.audioToPlay.length);

        audioEffectFinished = true;
    }

    private void CloseSystemMessageOnInput()
    {
        InputManager.OnTouchStart -= CloseSystemMessageOnInput;
        StartCoroutine(CloseSystemMessageCO());
    }

    private IEnumerator CloseSystemMessageCO()
    {
        systemMessageUIAnimator.SetTrigger("Close");

        if (closeUIClip)
            SFXManager.GetInstance().PlayClip(closeUIClip);

        yield return new WaitForSeconds(closeUIAnimationDuration);

        onSystemMessageClosed?.Invoke();
    }

    private IEnumerator TypeMessage(string message)
    {
        systemTMPText.text = message;
        systemTMPText.maxVisibleCharacters = 0;

        bool processingTag = false;
        foreach (char letter in message.ToCharArray())
        {
            // Abertura de tag
            if (letter == '<')
            {
                processingTag = true;
            }

            // Se estiver processando uma tag, pula para a proxima letra
            if (processingTag)
            {
                // Ultimo char da tag
                if (letter == '>')
                    processingTag = false;

                continue;
            }

            systemTMPText.maxVisibleCharacters++;
            yield return new WaitForSeconds(currentConfig.typingDelay);
        }
    }

}
