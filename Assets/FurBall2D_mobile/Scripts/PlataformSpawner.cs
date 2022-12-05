using System.Collections.Generic;
using UnityEngine;

public class PlataformSpawner : Singleton<PlataformSpawner>
{

    [SerializeField] private  List<GameObject> plataformList;


    public void SpawnRandomPlataform(Vector3 position)
    {
        if(plataformList != null && plataformList.Count > 0)
        {
            GameObject plataformToSpawn = plataformList[Random.Range(0, plataformList.Count)];
            GameObject plataformInstance = Instantiate(plataformToSpawn, 
                position, Quaternion.identity);

            EndlessRunnerManager.Instance.AddPlataform(plataformInstance);
        }
    }



}
