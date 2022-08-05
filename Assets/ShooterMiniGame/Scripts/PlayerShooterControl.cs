using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShooterControl : MonoBehaviour
{

    [SerializeField] private bool canMoveUp = false;
    [SerializeField] private bool canMoveDown = false;

    [SerializeField] private bool canMoveLeft = true;
    [SerializeField] private bool canMoveRight = true;


    [SerializeField] private float moveSpeed = 5.0f;

    private void Awake()
    {
        SwipeDetector.OnSwipe += MoveOnSwipe;
    }

    private void OnDestroy()
    {
        SwipeDetector.OnSwipe -= MoveOnSwipe;
    }

    private void MoveOnSwipe(SwipeData swipeData)
    {
        if(canMoveUp && swipeData.Direction == Direction.Up)
        {
            PerformMovement(Vector3.up);
        }
        else if (canMoveDown && swipeData.Direction == Direction.Down)
        {
            PerformMovement(Vector3.down);
        }


        if(canMoveLeft && swipeData.Direction == Direction.Left)
        {
            PerformMovement(Vector3.left);
        }
        else if(canMoveRight && swipeData.Direction == Direction.Right)
        {
            PerformMovement(Vector3.right);
        }

    }

    private void PerformMovement(Vector3 direction)
    {
        transform.position += direction * moveSpeed * Time.deltaTime; 
    }
}
