using UnityEngine;
using UnityEngine.Events;

// Baseado em: 
// https://www.raywenderlich.com/2826197-scriptableobject-tutorial-getting-started
// https://www.youtube.com/watch?v=lgA8KirhLEU

public class GameEventListener : MonoBehaviour
{
    [SerializeField]
    private GameEvent gameEvent; 
    [SerializeField]
    private UnityEvent response; 

    private void OnEnable() 
    {
        gameEvent.RegisterListener(this);
    }

    private void OnDisable() 
    {
        gameEvent.UnregisterListener(this);
    }

    public void RaiseEvent() 
    {
        response.Invoke();
    }
}