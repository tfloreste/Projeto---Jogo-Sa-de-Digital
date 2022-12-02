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

    private bool instanceTriggered;

    private void OnEnable()
    {
        instanceTriggered = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            ChangeScene();
        }
    }

    public void ChangeScene()
    {
        instanceTriggered = true;
        //SetStartPositionData();
        DataPersistenceManager.instance.SaveGame();
        LoadNextScene();
    }

    private void SetStartPositionData()
    {
        startPositionData.SetPosition(position);
        startPositionData.SetPosition(gameObjectName);
        startPositionData.SetDirection(direction);
    }


    private void LoadNextScene()
    {
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
