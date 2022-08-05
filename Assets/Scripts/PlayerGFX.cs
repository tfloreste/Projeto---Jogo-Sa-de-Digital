using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class PlayerGFX : MonoBehaviour
{
    private Animator animator;
    private Vector2 moveArray;

    [SerializeField] private AIPath aiPath;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        // Verifica se ha movimento horizontal
        if (aiPath.desiredVelocity.x >= 0.01f)
            moveArray.x = 1.0f;
        else if (aiPath.desiredVelocity.x <= -0.01f)
            moveArray.x = -1.0f;
        else
            moveArray.x = 0;

        // Verifica se ha movimento vertical
        if (aiPath.desiredVelocity.y >= 0.01f)
            moveArray.y = 1.0f;
        else if (aiPath.desiredVelocity.y <= -0.01f)
            moveArray.y = -1.0f;
        else
            moveArray.y = 0f;

        // Mantem apenas o movimento o eixo do movimento principal
        if(Mathf.Abs(moveArray.x) > 0 && Mathf.Abs(moveArray.y) > 0)
        {
            if(Mathf.Abs(aiPath.desiredVelocity.x) > Mathf.Abs(aiPath.desiredVelocity.y))
            {
                moveArray.y = 0f;
            }
            else
            {
                moveArray.x = 0f;
            }
        }


        UpdateAnimator();

    }

    // Atualiza a animação de acordo com o movimento
    private void UpdateAnimator()
    {
        /*if (Mathf.Approximately(moveArray.x, 0.0f) && Mathf.Approximately(moveArray.y, 0.0f))
        {
            animator.SetBool("isWalking", false);
        }
        else
        {
            animator.SetBool("isWalking", true);
            animator.SetFloat("xDirection", moveArray.x);
            animator.SetFloat("yDirection", moveArray.y);
        }*/
    }
}
