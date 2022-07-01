using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlataformMove : MonoBehaviour
{
    [SerializeField] private FloatVariable moveSpeed = null;

    private void Update()
    {
        if(moveSpeed != null)
            transform.position += Vector3.left * moveSpeed.value * Time.deltaTime;
    }
}
