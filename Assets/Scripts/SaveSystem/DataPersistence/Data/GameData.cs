using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    public long lastUpdated;
    public Vector3 playerPosition;
    public string gameObjectNameForPosition;
    public string sceneName;
    public Direction playerDirection;
    public SerializableDictionary<string, bool> conditions;
    public DayTime dayTime;
    public string dialogueVariablesJsonState;

    // the values defined in this constructor will be the default values
    // the game starts with when there's no data to load
    public GameData() 
    {
        Debug.Log("new GameData created");
        playerPosition = Vector3.zero;
        sceneName = "ThoughtsBubble";
        gameObjectNameForPosition = "";
        playerDirection = Direction.Up;
        dayTime = DayTime.DAY;

        conditions = new SerializableDictionary<string, bool>();
    }
}
