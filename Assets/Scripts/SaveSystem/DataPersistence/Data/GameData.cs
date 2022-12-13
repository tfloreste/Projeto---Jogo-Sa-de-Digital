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
    public SerializableDictionary<string, bool> cutscenesConditions;
    public DayTime dayTime;
    public string dialogueVariablesJsonState;

    // the values defined in this constructor will be the default values
    // the game starts with when there's no data to load
    public GameData() 
    {
        Debug.Log("new GameData created");
        playerPosition = Vector3.zero;
        sceneName = "School";
        gameObjectNameForPosition = "";
        playerDirection = Direction.Up;
        dayTime = DayTime.DAY;

        cutscenesConditions = new SerializableDictionary<string, bool>();

        if(cutscenesConditions != null)
        {
            Debug.Log("cutscenesConditions created");
        }
        else
        {
            Debug.Log("cutscenesConditions creation failed");
        }
    }
}
