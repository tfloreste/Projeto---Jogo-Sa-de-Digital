using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveByTouch : MonoBehaviour
{
    private bool moveEnabled = true;

    // Update is called once per frame
    void Update()
    { 
        if(moveEnabled && Input.touchCount > 0)
        {
            Debug.Log("Moving");
            Touch touch = Input.GetTouch(0);
            Vector3 touchPosition = Camera.main.ScreenToWorldPoint(touch.position);
            touchPosition.z = 0;
            transform.position = touchPosition;
        }
    }

    public void EnableMove()
    {
        Debug.Log("Movement Enabled");
        moveEnabled = true;
    }

    public void DisableMove()
    {
        Debug.Log("Movement Disabled");
        moveEnabled = false;
    }
}
