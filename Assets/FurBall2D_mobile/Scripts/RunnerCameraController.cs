using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunnerCameraController : MonoBehaviour
{
    [SerializeField] private Transform playerTransform = null;
    [SerializeField] private float camMoveSpeed = 1.0f;
    [SerializeField] private float deltaY= 0.5f; // Altera a posição vertical da câmera somente se o Player se movimentar esta distância verticalmente

    private Vector3 playerBasePosition;

    private void Start()
    {
        if(playerTransform != null)
        {
            playerBasePosition = playerTransform.position;
        }
    }
    // Update is called once per frame
    private void Update()
    {
        Vector3 playerPos = playerTransform.position;

        if(playerPos.y > (playerBasePosition.y + deltaY) || playerPos.y < (playerBasePosition.y - deltaY))
        {
            float maxDistance = camMoveSpeed * Time.deltaTime;
            Vector3 newCamPosition = new Vector3(transform.position.x, playerPos.y, transform.position.z);

            transform.position = Vector3.MoveTowards(transform.position, newCamPosition, maxDistance);
        }
    }
}
