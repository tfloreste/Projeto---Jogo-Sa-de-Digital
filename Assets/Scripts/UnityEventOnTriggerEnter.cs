using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class UnityEventOnTriggerEnter : MonoBehaviour
{
    [SerializeField] private UnityEvent onTriggerEnterEvent;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("OOnTriggerEnter2D fired");
        if (collision.CompareTag("Player"))
        {
            Debug.Log("Player fired trigger");
            onTriggerEnterEvent?.Invoke();
        }
    }
}
