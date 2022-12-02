using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerPlane : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("OnTriggerEnter2D fired");
        if (collision.gameObject.CompareTag("PlataformEndPosition"))
        {
            PlataformSpawner.Instance.SpawnRandomPlataform(collision.transform.position);
        }
    }
}
