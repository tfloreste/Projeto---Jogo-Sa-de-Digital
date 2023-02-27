using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SceneGalleryUI : MonoBehaviour
{
    [SerializeField] private Button[] buttons;
    [SerializeField] private string blockedSceneText = "???";
    [SerializeField] private AudioClip returnButtonClickAudio;
    [SerializeField] private AudioClip playSceneButtonClickAudio;

    private Text[] btnTexts;

    private void Start()
    {
        Debug.Log("SceneGalleryUI Start fired");
        InitializeButtons();
        InitializeButtonTexts();
        InitializeUI();

        Debug.Log("SceneGalleryUI returning DataPersistenceManager to default settings");
        DataPersistenceManager.Instance.ReturnDefaultSettings();
        Debug.Log("SceneGalleryUI Calling fadeIn effect");
        ScreenEffect.Instance.FadeIn(false);
        Debug.Log("SceneGalleryUI Start ended");
    }

    public void PlayScene(int sceneIndex)
    {
        if (playSceneButtonClickAudio)
            SFXManager.GetInstance().PlayClip(playSceneButtonClickAudio);

        SceneGalleryManager.Instance.PlayScene(sceneIndex);
    }
    
    public void ReturnToMainMenuButtonClicked()
    {
        if (returnButtonClickAudio)
            SFXManager.GetInstance().PlayClip(returnButtonClickAudio);

        SceneChanger.Instance.SetFadeMode(true);
        SceneChanger.Instance.SetSaveCurrentScene(false);
        SceneChanger.Instance.SetSavingMode(false);
        SceneChanger.Instance.ChangeTo("MainMenu");
    }

    private void InitializeUI()
    {
        Debug.Log("SceneGalleryUI InitializeUI fired");
        List<GameData> gameDataList = SceneGalleryManager.Instance.GetAllScenesData();

        for(int i = 0; i < buttons.Length; i++)
        {
            if(gameDataList != null && i < gameDataList.Count)
            {
                btnTexts[i].text = gameDataList[i].gallerySceneName;
                buttons[i].interactable = true;
            }
            else
            {
                btnTexts[i].text = blockedSceneText;
                buttons[i].interactable = false;
            }
        }
        Debug.Log("SceneGalleryUI InitializeUI finished");
    }

    private void InitializeButtonTexts()
    {
        Debug.Log("SceneGalleryUI InitializeButtonTexts fired");
        btnTexts = new Text[buttons.Length];

        for(int i = 0; i < buttons.Length; i++)
        {
            btnTexts[i] = buttons[i].transform.GetChild(0).GetComponent<Text>();
        }
        Debug.Log("SceneGalleryUI InitializeButtonTexts finished");
    }

    private void InitializeButtons()
    {
        Debug.Log("SceneGalleryUI InitializeButtons fired");
        for (int i = 0; i < buttons.Length; i++)
        {
            int btnIndex = i;
            buttons[i].onClick.AddListener(() => PlayScene(btnIndex));
        }
        Debug.Log("SceneGalleryUI InitializeButtons finished");
    }
}
