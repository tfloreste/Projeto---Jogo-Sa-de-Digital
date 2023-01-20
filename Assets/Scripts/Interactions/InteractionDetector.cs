using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionDetector : MonoBehaviour
{
    [SerializeField] private float interactionRadius = 2.0f;

    private HashSet<IInteractable> currentInteractablesInRange;
    private HashSet<IInteractable> previousInteractablesInRange;

    private void Awake()
    {
        currentInteractablesInRange = new HashSet<IInteractable>();
        previousInteractablesInRange = new HashSet<IInteractable>();
    }

    // Update is called once per frame
    void Update()
    {
        PrepareInteractionSets();

        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, interactionRadius);
        foreach(Collider2D collider in colliders)
        {
            if(collider.TryGetComponent<IInteractable>(out IInteractable interactable))
            {
                interactable.InInteractionRange();

                currentInteractablesInRange.Add(interactable);

                if (previousInteractablesInRange.Contains(interactable))
                    previousInteractablesInRange.Remove(interactable);
            }
        }

        foreach(IInteractable interactable in previousInteractablesInRange)
        {
            interactable.OutOfInteractionRange();
        }
    }

    private void PrepareInteractionSets()
    {
        previousInteractablesInRange.Clear();
        foreach (IInteractable interactable in currentInteractablesInRange)
        {
            previousInteractablesInRange.Add(interactable);
        }
        currentInteractablesInRange.Clear();
    }

    private void OnDrawGizmos()
    {
        //Gizmos.DrawSphere(transform.position, interactionRadius);
        Gizmos.DrawWireSphere(transform.position, interactionRadius);
    }
}
