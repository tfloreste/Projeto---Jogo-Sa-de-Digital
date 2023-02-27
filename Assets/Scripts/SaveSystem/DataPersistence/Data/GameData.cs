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
    public string gallerySceneName; // utilizado apenas para a geleria de cenas
    public string currentRegionName; 

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

    public static GameData CopyGameData(GameData gameDataToCopy)
    {
        GameData newGameData = new GameData();
        newGameData.lastUpdated = gameDataToCopy.lastUpdated;
        newGameData.gameObjectNameForPosition = gameDataToCopy.gameObjectNameForPosition;
        newGameData.sceneName = gameDataToCopy.sceneName;
        newGameData.playerDirection = gameDataToCopy.playerDirection;
        newGameData.dayTime = gameDataToCopy.dayTime;
        newGameData.dialogueVariablesJsonState = gameDataToCopy.dialogueVariablesJsonState;
        newGameData.gallerySceneName = gameDataToCopy.gallerySceneName;
        newGameData.currentRegionName = gameDataToCopy.currentRegionName;

        newGameData.playerPosition = new Vector3(
            gameDataToCopy.playerPosition.x,
            gameDataToCopy.playerPosition.y,
            gameDataToCopy.playerPosition.z
        );

        newGameData.conditions = new SerializableDictionary<string, bool>();
        foreach (KeyValuePair<string, bool> entry in gameDataToCopy.conditions)
            newGameData.conditions[entry.Key] = entry.Value;


        return newGameData;
    }

}
