using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class SceneChanger : Singleton<SceneChanger>, IDataPersistence
{
    private const string gallerySceneName = "SceneGallery";

    [SerializeField] private ScreenEffect screenEffect;
    private bool triggered = false;
    private string sceneName;

    private bool saveBeforeChangingScene = true;
    private bool performFadeAnimation = false;
    private bool saveCurrentSceneName = false;

    private bool savePlayerDirection = false;
    private Direction playerDirection;

    private bool savePlayerPosition = false;
    private bool setPositionByGameObject = false;
    private string gameObjectPosition;
    private Vector3 playerPosition;

    private bool checkForGalleryMode = true;

    private void Awake()
    {
        checkForGalleryMode = true; // por padrão verifica se está no modo de galeria
    }

    public void ChangeTo(string sceneName)
    {
        Debug.Log("ChangeTo called with sceneName: " + sceneName);
        Debug.Log("IsGalleryMode?: " + (DataPersistenceManager.Instance.LoadedMode == GameLoadedMode.GALLERY));
        Debug.Log("shouldCheckForGalleryMode?: " + checkForGalleryMode);

        if (checkForGalleryMode && DataPersistenceManager.Instance.LoadedMode == GameLoadedMode.GALLERY)
            return;

        ChangeScene(sceneName);
    }

    private void ChangeScene(string sceneName)
    {
        if (performFadeAnimation)
        {
            StartCoroutine(ChangeToSceneWithFadeOutCO(sceneName));
            return;
        }

        this.triggered = true;
        this.sceneName = sceneName;

        if (saveBeforeChangingScene)
            DataPersistenceManager.Instance.SaveGame();

        SceneManager.LoadScene(sceneName);
    }

    public void IgnoreGalleryModeForNextSceneChange()
    {
        checkForGalleryMode = false;
    }

    public void GoToSceneGallery()
    {
        ChangeScene(gallerySceneName);
    }

    public void GoToSceneGallery(bool performFade)
    {
        performFadeAnimation = performFade;
        GoToSceneGallery();
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

    public void SetSaveCurrentScene(bool shouldSaveCurrentScene)
    {
        saveCurrentSceneName = shouldSaveCurrentScene;
    }

    public void SetSavePlayerPosition(bool shouldSavePosition)
    {
        savePlayerPosition = shouldSavePosition;
    }

    public void SetSavePlayerDirection(bool shouldSavePlayerDirection)
    {
        savePlayerDirection = shouldSavePlayerDirection;
    }

    public void SetGameObjectPositionName(string gameObjectName)
    {
        setPositionByGameObject = true;
        gameObjectPosition = gameObjectName;
    }

    public void SetPlayerPositionVector(Vector3 positionVector)
    {
        setPositionByGameObject = false;
        playerPosition = positionVector;
    }

    public void SetPlayerDirection(Direction direction)
    {
        playerDirection = direction;
    }

    private IEnumerator ChangeToSceneWithFadeOutCO(string sceneName)
    {
        this.triggered = true;
        this.sceneName = sceneName;

        if(saveBeforeChangingScene)
            DataPersistenceManager.Instance.SaveGame();

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
        data.sceneName = saveCurrentSceneName ?
            SceneManager.GetActiveScene().name : 
            sceneName;

        if (savePlayerPosition)
        {
            if (setPositionByGameObject && gameObjectPosition != "")
            {
                data.gameObjectNameForPosition = gameObjectPosition;
                data.playerPosition = Vector3.zero;
            }
            else if (!setPositionByGameObject)
            {
                data.gameObjectNameForPosition = "";
                data.playerPosition = playerPosition;
            }
        }

        if(savePlayerDirection)
        {
            data.playerDirection = playerDirection;

            Debug.Log("DirectionValue: " + data.playerDirection);
            switch (playerDirection)
            {
                case Direction.Right:
                    Debug.Log("Saved Right direction");
                    break;
                case Direction.Left:
                    Debug.Log("Saved Left direction");
                    break;
                case Direction.Up:
                    Debug.Log("Saved Up direction");
                    break;
                case Direction.Down:
                    Debug.Log("Saved Down direction");
                    break;
                default:
                    Debug.Log("Saved is wrong");
                    break;
            }
        }
    }

}
