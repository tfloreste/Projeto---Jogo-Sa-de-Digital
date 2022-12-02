using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class SceneChanger : Singleton<SceneChanger>, IDataPersistence
{
    private bool triggered = false;
    private string sceneName;

    public void ChangeTo(string sceneName)
    {
        this.triggered = true;
        this.sceneName = sceneName;

        DataPersistenceManager.instance.SaveGame();
        SceneManager.LoadScene(sceneName);
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
