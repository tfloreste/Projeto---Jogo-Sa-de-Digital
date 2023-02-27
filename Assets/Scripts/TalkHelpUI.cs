using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TalkHelpUI : MonoBehaviour
{
    [SerializeField] private Animator talkHelpUIAnimator;

    private int helpRequestCount = 0; // usado para quando um NPC quer fechar a UI de ajuda, mas outro NPC quer deixar ela aberta
    private bool isEnabled = true;
    private bool isVisible = false;
    private bool shouldBeShowingUI = false;

    private void Awake()
    {
        helpRequestCount = 0;
    }

    public void ShowTalkingHelp()
    {
        helpRequestCount++;

        // Só faz a animação de exibir a UI na primeira chamada
        if (helpRequestCount > 1)
            return;

        if (!isEnabled)
            return;

        OpenUI();
    }

    public void CloseTalkingHelp()
    {
        helpRequestCount--;
        // Só fecha a UI quando todos tiverem chamado a função
        if (helpRequestCount > 0)
            return;

        if (!isEnabled)
            return;

        shouldBeShowingUI = false;
        CloseUI();
    }

    public void Disable()
    {
        shouldBeShowingUI = false;
        isEnabled = false;
        if (isVisible)
        {
            CloseUI();
            shouldBeShowingUI = true;
        }
    }

    public void Enable()
    {
        isEnabled = true;
        if (shouldBeShowingUI)
        {
            OpenUI();
            shouldBeShowingUI = false;
        }
    }

    private void OpenUI()
    {
        isVisible = true;
        talkHelpUIAnimator.SetTrigger("Open");
    }

    private void CloseUI()
    {
        isVisible = false;
        talkHelpUIAnimator.SetTrigger("Close");
    }
}
