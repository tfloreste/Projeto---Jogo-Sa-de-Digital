using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChangeTrigger : MonoBehaviour, IDataPersistence
{
    [Header("New Scene Data")]
    [SerializeField] private string gameObjectName;
    [SerializeField] private Vector3 position;
    [SerializeField] private Direction direction;

    [Header("Requirements")]
    [SerializeField] private string sceneName;
    [SerializeField] private StartingPositionData startPositionData;

    [Header("Data")]
    [SerializeField] private bool isDoor = false;

    private bool instanceTriggered;

    private void OnEnable()
    {
        instanceTriggered = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if(collision.TryGetComponent(out PlayerSwipeController playerController))
                playerController.BlockMovement();
            
            ChangeScene(true);
        }
    }

    public void ChangeScene(bool performFadeEffect)
    {
        if (DataPersistenceManager.Instance.LoadedMode == GameLoadedMode.GALLERY)
            return;

        instanceTriggered = true;
        //SetStartPositionData();
        DataPersistenceManager.Instance.SaveGame();
        StartCoroutine(LoadNextScene(performFadeEffect));
    }

    private IEnumerator LoadNextScene(bool performFadeEffect)
    {
        if (performFadeEffect)
        {
            ScreenEffect.Instance.FadeOut(true);
            yield return new WaitForSeconds(1.5f);
        }
        
        SceneManager.LoadScene(sceneName);
    }

    public void LoadData(GameData data)
    {
        // Nada para carregar
        return;
    }

    public void SaveData(GameData data)
    {
        // Somente a instância em que o trigger foi chamado
        // deverá salvar os dados para a próxima cena
        if (!instanceTriggered)
            return;

        Debug.Log("Scene ChangeTrigger save fired");
        data.playerPosition = position;
        data.gameObjectNameForPosition = gameObjectName;
        data.sceneName = sceneName;
        data.playerDirection = direction;
    }
}
