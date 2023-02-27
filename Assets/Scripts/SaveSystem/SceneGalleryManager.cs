using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneGalleryManager : MonoBehaviour
{
    public static SceneGalleryManager Instance { get; private set; }

    private string baseDirectory = "gallery";
    private string baseFileName = "scene";

    private string galleryPath;
    private FileDataHandler dataHandler;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.Log("Found more than one Data Persistence Manager in the scene. Destroying the newest one.");
            Destroy(this.gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(this.gameObject);

        galleryPath = Path.Combine(Application.persistentDataPath, baseDirectory);
    }

    private void Start()
    {
        dataHandler = new FileDataHandler(galleryPath, baseFileName, DataPersistenceManager.Instance.UseEncryption);
    }

    public void AddSceneToGallery(int sceneIndex, GameData gameData, string gallerySceneName)
    {
        string profileName = GetProfileName(sceneIndex);
        gameData.lastUpdated = System.DateTime.Now.ToBinary();
        gameData.gallerySceneName = gallerySceneName;
        dataHandler.Save(gameData, profileName);
    }

    public GameData GetSceneData(int sceneIndex)
    {
        string profileName = GetProfileName(sceneIndex);
        return GetSceneData(profileName);
    }

    public GameData GetSceneData(string profileName)
    {
        if(dataHandler == null)
            dataHandler = new FileDataHandler(galleryPath, baseFileName, DataPersistenceManager.Instance.UseEncryption);

        return dataHandler.Load(profileName, true);
    }

    public List<GameData> GetAllScenesData()
    {
        bool sceneFound;
        List<GameData> sceneDataList = new List<GameData>();
        int sceneIndex = 0;

        do
        {
            sceneFound = false;

            GameData sceneData = GetSceneData(sceneIndex);
            if(sceneData != null)
            {
                sceneFound = true;
                sceneDataList.Add(sceneData);
            }

            sceneIndex++;
           
        } while (sceneFound);

        return sceneDataList;
    }

    public void PlayScene(int sceneIndex)
    {
        DataPersistenceManager.Instance.SetDataHander(dataHandler);
        DataPersistenceManager.Instance.ChangeSelectedProfileId(GetProfileName(sceneIndex));
        DataPersistenceManager.Instance.SetGameLoadedMode(GameLoadedMode.GALLERY);
        DataPersistenceManager.Instance.LoadGame();
        StartCoroutine(LoadSceneCO(DataPersistenceManager.Instance.GetGameData().sceneName));
    }

    private IEnumerator LoadSceneCO(string sceneName)
    {
        ScreenEffect.Instance.FadeOut(true);
        yield return new WaitForSeconds(ScreenEffect.Instance.FadeOutDuration);
        SceneManager.LoadScene(sceneName);
    }

    private string GetProfileName(int sceneIndex)
    {
        return baseFileName + "_" + sceneIndex;
    }
}
