using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuButtons : MonoBehaviour
{
    [SerializeField] private Button newGameButton;
    [SerializeField] private Button continueButton;

    private void Start()
    {
        DisableButtonsDependingOnData();
    }

    private void DisableButtonsDependingOnData()
    {
        if (!DataPersistenceManager.instance.HasGameData())
        {
            continueButton.interactable = false;
        }
    }

    private void DisableAllButtons()
    {
        newGameButton.interactable = false;
        continueButton.interactable = false;
    }


    public void OnNewGameClicked()
    {
        DisableAllButtons();

        DataPersistenceManager.instance.NewGame();
        DataPersistenceManager.instance.SaveGame();
        SceneManager.LoadScene(DataPersistenceManager.instance.GetGameData().sceneName);
    }

    public void OnContinueClicked()
    {
        DisableAllButtons();
        SceneManager.LoadScene(DataPersistenceManager.instance.GetGameData().sceneName);
    }
}
