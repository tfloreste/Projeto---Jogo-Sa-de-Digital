using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


/// <summary>
/// Referência: https://forum.unity.com/threads/how-can-i-determine-where-my-player-will-appear-in-the-next-scene.736007/
/// </summary>
public class SceneChangeManager : Singleton<SceneChangeManager>
{
    private Dictionary<GameObject, string> gameObjectTargets;
    private Dictionary<GameObject, Vector3> gameObjectPositions;

    private void Awake()
    {
        DontDestroyOnLoad(this);
        gameObjectTargets = new Dictionary<GameObject, string>();
        gameObjectPositions = new Dictionary<GameObject, Vector3>();
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Place all objects by position
        foreach (KeyValuePair<GameObject, Vector3> objectToPosition in gameObjectPositions)
        {
            if (objectToPosition.Key == null)
                continue;

            objectToPosition.Key.transform.position = objectToPosition.Value;
        }
        gameObjectPositions.Clear();

        // Place all objects by target name
        foreach (KeyValuePair<GameObject, string> objectToPosition in gameObjectTargets)
        {
            GameObject target = GameObject.Find(objectToPosition.Value);

            if (objectToPosition.Key == null || target == null)
                continue;

            objectToPosition.Key.transform.position = target.transform.position;
        }
        gameObjectTargets.Clear();
    }

    public void LoadScene(string scene)
    {
        SceneManager.LoadScene(scene);
    }

    public void SetObjectPositionNextScene(GameObject gameObject, string target)
    {
        gameObjectTargets.Add(gameObject, target);
    }

    public void SetObjectPositionNextScene(GameObject gameObject, Vector3 position)
    {
        gameObjectPositions.Add(gameObject, position);
    }
}
