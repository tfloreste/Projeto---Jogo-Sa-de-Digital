using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameEnded : MonoBehaviour
{

    [SerializeField] private string mainMenuSceneName = "MainMenu";
    [SerializeField] private bool deleteSaveData = true;

    public void OnGameEnded()
    {
        StartCoroutine(OnGameEndedCO());
    }

    private IEnumerator OnGameEndedCO()
    {
        if (deleteSaveData)
            DataPersistenceManager.Instance.DeleteCurrentProfileData();

        yield return new WaitForSeconds(0.2f);

        SceneManager.LoadScene(mainMenuSceneName);
    }
}
