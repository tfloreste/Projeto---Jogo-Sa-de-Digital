using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InputManager : MonoBehaviour
{
    public delegate void InputAction();
    public static event InputAction OnTouchStart;

    public static InputManager instance;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("Found more than one Input Manager in the scene: " + SceneManager.GetActiveScene().name);
        }
        instance = this;
    }

    private void Update()
    {
        if(Input.touchCount > 0 && Input.touches[0].phase == TouchPhase.Began)
        {
            OnTouchStart?.Invoke();
        }
    }
}
