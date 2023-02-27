using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuButtons : MonoBehaviour
{
    [SerializeField] private CustomButton newGameButton;
    [SerializeField] private CustomButton continueButton;
    [SerializeField] private CustomButton quitButton;

    private const string sceneGalleryUnitySceneName = "SceneGallery";

    private bool buttonClicked = false;

    private void Start()
    {
        DisableButtonsDependingOnData();
        ScreenEffect.Instance.FadeIn(false);
    }

    private void DisableButtonsDependingOnData()
    {
        if (!DataPersistenceManager.Instance.HasGameData())
        {
            continueButton.Disable();
        }
    }

    private void DisableAllButtons()
    {
        newGameButton.Disable();
        continueButton.Disable();
        quitButton.Disable();
    }


    public void OnNewGameClicked()
    {
        if (buttonClicked)
            return;

        buttonClicked = true;

        DataPersistenceManager.Instance.ReturnDefaultSettings();
        DataPersistenceManager.Instance.NewGame();
        DataPersistenceManager.Instance.SaveGame();
        StartCoroutine(LoadGameSceneCO(DataPersistenceManager.Instance.GetGameData().sceneName));
    }

    public void OnContinueClicked()
    {
        if (buttonClicked)
            return;

        buttonClicked = true;

        DataPersistenceManager.Instance.ReturnDefaultSettings();
        StartCoroutine(LoadGameSceneCO(DataPersistenceManager.Instance.GetGameData().sceneName));
    }

    public void OnSceneGalleryClicked()
    {
        if (buttonClicked)
            return;

        buttonClicked = true;
        StartCoroutine(LoadGameSceneCO(sceneGalleryUnitySceneName));
    }

    public void OnQuitClicked()
    {
        if (buttonClicked)
            return;

        buttonClicked = true;
        StartCoroutine(QuitCoroutine());
    }

    private IEnumerator QuitCoroutine()
    {
        ScreenEffect.Instance.FadeOut(true);
        yield return new WaitForSeconds(2f);
        Application.Quit();
    }

    private IEnumerator LoadGameSceneCO(string sceneName)
    {
        ScreenEffect.Instance.FadeOut(true);
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene(sceneName);
    }
}
