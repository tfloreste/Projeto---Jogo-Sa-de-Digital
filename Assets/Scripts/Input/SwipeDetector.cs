using System;
using UnityEngine;

public class SwipeDetector : MonoBehaviour
{
    private Vector2 fingerDownPosition;
    private Vector2 fingerUpPosition;
    private Direction lastSwipeDirection;
    private bool swipeStarted = false;

    [SerializeField] private bool detectSwipeOnlyAfterRelease = false;
    [SerializeField] private bool detectSwipeWhenStationary = true;

    [SerializeField] private float minDistanceForSwipe = 20f;

    public static event Action<SwipeData> OnSwipe = delegate { };

    private Camera cam;
    private void Start()
    {
        cam = Camera.main;    
    }

    private void Update()
    {
        if(Input.touchCount > 0)
        {            
            ProcessTouch(Input.GetTouch(0));   
        } 
        else
        {
            swipeStarted = false;
        }
    }

    private void ProcessTouch(Touch touch)
    {
        if (touch.phase == TouchPhase.Began)
        {
            fingerUpPosition = touch.position;
            fingerDownPosition = touch.position;
        }
        else if (!detectSwipeOnlyAfterRelease && touch.phase == TouchPhase.Moved)
        {
            fingerDownPosition = touch.position;
            DetectSwipe();
        }
        else if (!detectSwipeOnlyAfterRelease && detectSwipeWhenStationary && swipeStarted && touch.phase == TouchPhase.Stationary)
        {
            SendSwipe(lastSwipeDirection);
        }
        else if (touch.phase == TouchPhase.Ended)
        {
            fingerDownPosition = touch.position;
            DetectSwipe();
        }
    }

    private void DetectSwipe()
    {
        if (SwipeDistanceCheckMet())
        {
            swipeStarted = true; 
            Direction direction;

            if (IsVerticalSwipe())
            {
                direction = fingerDownPosition.y - fingerUpPosition.y > 0 ? Direction.Up : Direction.Down;
            }
            else
            {
                direction = fingerDownPosition.x - fingerUpPosition.x > 0 ? Direction.Right : Direction.Left;
                SendSwipe(direction);
            }

            SendSwipe(direction);

            //Debug.Log(fingerUpPosition);
            //Debug.Log(fingerDownPosition);

            lastSwipeDirection = direction;
            fingerUpPosition = fingerDownPosition;
        }
        else if(detectSwipeWhenStationary && swipeStarted)
        {
            SendSwipe(lastSwipeDirection);
        }
    }

    private bool IsVerticalSwipe()
    {
        return VerticalMovementDistance() > HorizontalMovementDistance();
    }

    private bool SwipeDistanceCheckMet()
    {
        return VerticalMovementDistance() > minDistanceForSwipe || HorizontalMovementDistance() > minDistanceForSwipe;
    }

    private float VerticalMovementDistance()
    {
        return Mathf.Abs(fingerDownPosition.y - fingerUpPosition.y);
    }

    private float HorizontalMovementDistance()
    {
        return Mathf.Abs(fingerDownPosition.x - fingerUpPosition.x);
    }

    private void SendSwipe(Direction direction)
    {
        Debug.DrawLine(fingerUpPosition, fingerDownPosition, Color.red);
        SwipeData swipeData = new SwipeData()
        {
            Direction = direction,
            StartPosition = fingerDownPosition,
            EndPosition = fingerUpPosition
        };
        OnSwipe(swipeData);
    }
}

public struct SwipeData
{
    public Vector2 StartPosition;
    public Vector2 EndPosition;
    public Direction Direction;
}

[System.Serializable]
public enum Direction
{
    Up,
    Down,
    Left,
    Right
}