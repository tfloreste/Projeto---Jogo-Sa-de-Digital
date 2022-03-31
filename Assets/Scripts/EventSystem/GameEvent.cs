using System.Collections.Generic;
using UnityEngine;

// Baseado em: 
// https://www.raywenderlich.com/2826197-scriptableobject-tutorial-getting-started
// https://www.youtube.com/watch?v=lgA8KirhLEU

[CreateAssetMenu(fileName = "New Game Event", menuName = "Game Event", order = 52)] 
public class GameEvent : ScriptableObject // 2
{
    private HashSet<GameEventListener> listeners = new HashSet<GameEventListener>(); 

    public void Invoke() 
    {
        foreach (GameEventListener eventListener in listeners)
        {
            eventListener.RaiseEvent();
        }
    }

    public void RegisterListener(GameEventListener listener) 
    {
        listeners.Add(listener);
    }

    public void UnregisterListener(GameEventListener listener) 
    {
        listeners.Remove(listener);
    }
}