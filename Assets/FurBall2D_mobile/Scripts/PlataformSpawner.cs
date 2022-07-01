using System.Collections.Generic;
using UnityEngine;

public class PlataformSpawner : MonoBehaviour
{

    [SerializeField] private  List<GameObject> plataformList;

    private static PlataformSpawner instance = null;

    public static PlataformSpawner GetInstance()
    {
        return instance;
    }

    private void Awake()
    {
        if(instance != null)
        {
            Debug.LogWarning("Mais de uma instancia de PlataformSpawner");
        }

        instance = this;
    }

    public void SpawnRandomPlataform(Vector3 position)
    {
        if(plataformList != null && plataformList.Count > 0)
        {
            GameObject plataformToSpawn = plataformList[Random.Range(0, plataformList.Count)];
            Instantiate(plataformToSpawn, position, Quaternion.identity);
        }
    }



}
