using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSwipeController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 1.0f;
    [SerializeField] Animator animator = null;

    private Rigidbody2D rigidbody2d = null;
    private Vector3 playerVelocity;
    private bool consumedSwipe = false;
    private bool isInDialogue = false;
    private bool isInCutscene = false;
    private bool movementBlocked = false;

    private void Awake()
    {
        rigidbody2d = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        SwipeDetector.OnSwipe += ProcessMovement;
    }

    private void OnDestroy()
    {
        SwipeDetector.OnSwipe -= ProcessMovement;
    }

    private void FixedUpdate()
    {
        if (isInDialogue || isInCutscene || movementBlocked)
        {
            rigidbody2d.velocity = Vector3.zero;
            animator.SetBool("isWalking", false);
            return;
        }

        if (consumedSwipe)
        {
            rigidbody2d.velocity = Vector3.zero;
            animator.SetBool("isWalking", false);

        }
        else
        {
            rigidbody2d.velocity = playerVelocity * moveSpeed * Time.fixedDeltaTime;
            consumedSwipe = true;
        }
    }

    public void SetInDialogue(bool inDialogue)
    {
        isInDialogue = inDialogue;
    }

    public void SetInCutscene(bool InCutscene)
    {
        isInCutscene = InCutscene;
    }

    public void BlockMovement()
    {
        movementBlocked = true;
    }

    public void UnblockMovement()
    {
        movementBlocked = false;
    }

    private void ProcessMovement(SwipeData swipeData)
    {
        if (isInDialogue || isInCutscene || movementBlocked)
        {
            return;
        }

        consumedSwipe = false;

        float xDirection = 0f;
        float yDirection = 0f;

        switch (swipeData.Direction) 
        {
            case Direction.Up:
                yDirection = 1.0f;
                playerVelocity = Vector3.up;
                break;

            case Direction.Down:
                yDirection = -1.0f;
                playerVelocity = Vector3.down;
                break;

            case Direction.Right:
                xDirection = 1.0f;
                playerVelocity = Vector3.right;
                break;
            case Direction.Left:
                xDirection = -1.0f;
                playerVelocity = Vector3.left;
                break;

            default:
                playerVelocity = Vector3.zero;
                break;
        }

        bool isWalking = (Mathf.Abs(xDirection) > 0.0f || Mathf.Abs(yDirection) > 0.0f);

        animator.SetFloat("xDirection", xDirection);
        animator.SetFloat("yDirection", yDirection);
        animator.SetBool("isWalking", isWalking);
    }
}
