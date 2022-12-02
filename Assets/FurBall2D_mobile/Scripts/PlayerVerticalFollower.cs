using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerVerticalFollower : MonoBehaviour
{
    [SerializeField] Transform playerTransform;
    [SerializeField] float verticalDistance = 2.0f;
    [SerializeField] bool keepCurrentDistance = false;


    // Start is called before the first frame update
    void Start()
    {
        float xPos = transform.position.x;
        float yPos = playerTransform.position.y + verticalDistance;

        if(keepCurrentDistance)
        {
            yPos = transform.position.y;
            verticalDistance = yPos - playerTransform.position.y;
        }

        transform.position = new Vector3(xPos, yPos);

    }

    // Update is called once per frame
    void Update()
    {
        if (!EndlessRunnerManager.Instance.gameStarted)
            return;

        float dist = transform.position.y - playerTransform.position.y;
        
        if(!Mathf.Approximately(dist, verticalDistance))
        {
            float newYPos = playerTransform.position.y + verticalDistance;
            transform.position = new Vector3(transform.position.x, newYPos);
        }
    }
}
