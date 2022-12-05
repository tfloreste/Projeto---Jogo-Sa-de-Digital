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
            // Setando a posição z para 0 para evitar que algum valor acidental
            // se acumule e faça com que objetos não sejam renderizados após um
            // tempo de jogo
            Vector3 endPosition = collision.transform.position;
            Vector3 spawnPosition = new Vector3(endPosition.x, endPosition.y, 0.0f);

            PlataformSpawner.Instance.SpawnRandomPlataform(spawnPosition);
        }
    }
}
