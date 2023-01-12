using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuButtons : MonoBehaviour
{
    [SerializeField] private MainMenuButton newGameButton;
    [SerializeField] private MainMenuButton continueButton;

    private bool buttonClicked = false;

    private void Start()
    {
        DisableButtonsDependingOnData();
        ScreenEffect.Instance.FadeIn(false);
    }

    private void DisableButtonsDependingOnData()
    {
        if (!DataPersistenceManager.instance.HasGameData())
        {
            continueButton.Disable();
        }
    }

    private void DisableAllButtons()
    {
        newGameButton.Disable();
        continueButton.Disable();
    }


    public void OnNewGameClicked()
    {
        if (buttonClicked)
            return;

        buttonClicked = true;

        DataPersistenceManager.instance.NewGame();
        DataPersistenceManager.instance.SaveGame();
        StartCoroutine(LoadGameSceneCO(DataPersistenceManager.instance.GetGameData().sceneName));
    }

    public void OnContinueClicked()
    {
        if (buttonClicked)
            return;

        buttonClicked = true;
        StartCoroutine(LoadGameSceneCO(DataPersistenceManager.instance.GetGameData().sceneName));
    }

    private IEnumerator LoadGameSceneCO(string sceneName)
    {
        ScreenEffect.Instance.FadeOut(true);
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene(sceneName);
    }
}
