using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChangeTrigger : MonoBehaviour
{
    [Header("New Scene Data")]
    [SerializeField] private string gameObjectName;
    [SerializeField] private Vector3 position;
    [SerializeField] private Direction direction;

    [Header("Requirements")]
    [SerializeField] private string sceneName;
    [SerializeField] private StartingPositionData startPositionData;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            SetStartPositionData();
            LoadNextScene();
        }
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
}
