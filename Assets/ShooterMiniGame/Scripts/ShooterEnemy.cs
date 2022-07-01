using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShooterEnemy : MonoBehaviour
{
    [SerializeField] private float verticalMoveSpeed;
    [SerializeField] private float horizontalMoveSpeed;

    private GameObject player;
   
    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }
    
    void Update()
    {
        float playerDirection = 0.0f;

        if(player != null)
        {
            playerDirection = player.transform.position.x > transform.position.x ? 1.0f : -1.0f;
        }

        Vector3 moveDirection = new Vector3(playerDirection, -1.0f, 0.0f);
        moveDirection.Normalize();


        moveDirection.Normalize();
        transform.position += Vector3.Scale(moveDirection, new Vector3(horizontalMoveSpeed, verticalMoveSpeed, 0.0f)) * Time.deltaTime;

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            Destroy(collision.gameObject);
        }
    }
}
