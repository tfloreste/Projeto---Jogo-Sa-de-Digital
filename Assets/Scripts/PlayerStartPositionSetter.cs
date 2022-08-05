using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStartPositionSetter : MonoBehaviour
{
    [SerializeField] private StartingPositionData positionData;


    private void Start()
    {
        Animator animator = transform.GetChild(0).GetComponent<Animator>();
        SetPosition();
        SetDirection(animator);
    }

    private void SetPosition()
    {
        Vector3 startingPosition = positionData.GetPosition();

        if (startingPosition.sqrMagnitude > 0)
            transform.position = startingPosition;
    }

    private void SetDirection(Animator animator)
    {
        float xDirection = 0f;
        float yDirection = 0f;
        bool directionSetted = true;

        switch (positionData.GetDirection())
        {
            case Direction.Up:
                yDirection = 1f;
                break;

            case Direction.Down:
                yDirection = -1f;
                break;

            case Direction.Right:
                xDirection = 1f;
                break;

            case Direction.Left:
                xDirection = -1f;
                break;

            default:
                directionSetted = false;
                break;
        }

        if(directionSetted)
        {
            animator.SetFloat("xDirection", xDirection);
            animator.SetFloat("yDirection", yDirection);
        }
    }

}
