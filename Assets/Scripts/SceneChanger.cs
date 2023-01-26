using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class SceneChanger : Singleton<SceneChanger>, IDataPersistence
{
    [SerializeField] private ScreenEffect screenEffect;
    private bool triggered = false;
    private string sceneName;

    private bool saveBeforeChangingScene = true;
    private bool performFadeAnimation = false;

    public void ChangeTo(string sceneName)
    {
        if(performFadeAnimation)
        {
            StartCoroutine(ChangeToSceneWithFadeOutCO(sceneName));
            return;
        }

        this.triggered = true;
        this.sceneName = sceneName;

        if (saveBeforeChangingScene)
            DataPersistenceManager.instance.SaveGame();
       
        SceneManager.LoadScene(sceneName);
    }

    public void ChangeToSceneWithFadeOut(string sceneName)
    {
        performFadeAnimation = true;
        ChangeTo(sceneName);
    }

    public void SetSavingMode(bool shouldSave)
    {
        saveBeforeChangingScene = shouldSave;
    }

    public void SetFadeMode(bool shouldFade)
    {
        performFadeAnimation = shouldFade;
    }

    private IEnumerator ChangeToSceneWithFadeOutCO(string sceneName)
    {
        this.triggered = true;
        this.sceneName = sceneName;

        if(saveBeforeChangingScene)
            DataPersistenceManager.instance.SaveGame();

        screenEffect.FadeOut(true);
        yield return new WaitForSeconds(screenEffect.FadeOutDuration);
        SceneManager.LoadScene(sceneName);
    }

    public void ChangeToSceneWithoutSaving(string sceneName)
    {
        saveBeforeChangingScene = false;
        ChangeTo(sceneName);
    }

    public void LoadData(GameData data)
    {
        // Nothing to load here
    }

    public void SaveData(GameData data)
    {
        if (!triggered)
            return;

        triggered = false;
        data.sceneName = sceneName;
    }
}
