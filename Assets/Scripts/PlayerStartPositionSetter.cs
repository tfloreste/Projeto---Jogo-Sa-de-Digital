using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStartPositionSetter : MonoBehaviour, IDataPersistence
{
    [SerializeField] private StartingPositionData positionData;

    private Vector3 playerPosition;
    private string gameObjectNameForPosition;
    private Direction playerDirection;

    private void Start()
    {
        Animator animator = transform.GetChild(0).GetComponent<Animator>();
        SetPosition();
        SetDirection(animator);
    }

    /*private void SetPosition()
    {
        Vector3 startingPosition = positionData.GetPosition();

        if (startingPosition.sqrMagnitude > 0)
            transform.position = startingPosition;
    }*/

    public void LoadData(GameData data)
    {
        playerPosition = data.playerPosition;
        gameObjectNameForPosition = data.gameObjectNameForPosition;
        playerDirection = data.playerDirection;
    }

    public void SaveData(GameData data)
    {
        // Nada para salvar
        return;
    }

    private void SetPosition()
    {
        Vector3 startingPosition = Vector3.zero;
        if (playerPosition.sqrMagnitude > 0)
        {
            startingPosition = playerPosition;
        }

        else if (gameObjectNameForPosition != "")
        {
            GameObject target = GameObject.Find(gameObjectNameForPosition);
            if (target)
                startingPosition = target.transform.position;
        }

        transform.position = startingPosition;
    }

    private void SetDirection(Animator animator)
    {
        float xDirection = 0f;
        float yDirection = 0f;
        bool directionSetted = true;

        //switch (positionData.GetDirection())
        switch (playerDirection)
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

        if (directionSetted)
        {
            animator.SetFloat("xDirection", xDirection);
            animator.SetFloat("yDirection", yDirection);
        }
    }

    public void SetPositionOnGFX(float delay)
    {
        StartCoroutine(COSetPositionOnGFX(delay));
    }

    private IEnumerator COSetPositionOnGFX(float delay)
    {
        yield return new WaitForSeconds(delay);

        Transform child = transform.GetChild(0);

        Debug.Log("childPosition: " + child.position);
        Debug.Log("childLocalPosition: " + child.localPosition);
        Debug.Log("transformPosition: " + transform.position);

        transform.position = child.position;
        child.localPosition = new Vector3(0, 0, 0);

        Debug.Log("new childPosition: " + child.position);
        Debug.Log("new childLocalPosition: " + child.localPosition);
        Debug.Log("new transformPosition: " + transform.position);
    }

}
