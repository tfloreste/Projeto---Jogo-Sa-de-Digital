using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionButton : MonoBehaviour
{
    private const string uiLayerName = "UI";
    private IInteractable interactableObject;
    private Collider2D thisCollider;
    private Animator animator;

    private bool isInDialogue = false;
    private bool isVisible = false;

    private void Awake()
    {
        thisCollider = GetComponent<Collider2D>();
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        gameObject.SetActive(false);
    }
    
    private void Update()
    {
        if (isInDialogue)
            return;

        if (Input.touchCount > 0)
        {
            int uiLayerMaks = 1 << LayerMask.NameToLayer("UI");

            Vector3 wp = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);
            Vector2 touchPos = new Vector2(wp.x, wp.y);
            Collider2D touchCollider = Physics2D.OverlapPoint(touchPos, uiLayerMaks);

            if (thisCollider == Physics2D.OverlapPoint(touchPos, uiLayerMaks))
            {
                ClickPerformed();
            }
        }
    }

    public void Show(IInteractable interactable)
    {
        if (isInDialogue || isVisible)
            return;

        interactableObject = interactable;
        isVisible = true;

        if (gameObject.activeSelf)
            animator.SetTrigger("Open");
        else
            gameObject.SetActive(true);
        
    }

    public void Hide()
    {
        Hide(true);
    }

    public void Hide(bool disableAfterClosing)
    {
        if (!gameObject.activeSelf)
            return;

        StartCoroutine(HideCO(disableAfterClosing));
    }

    private IEnumerator HideCO(bool disableAfterClosing)
    {
        if (animator)
        {
            animator.SetTrigger("Close");
            yield return new WaitForSeconds(0.4f);
        }

        isVisible = false;
        if (disableAfterClosing)
            gameObject.SetActive(false);
    }

    public void OnDialogueStarted()
    {
        isInDialogue = true;
        if(gameObject.activeSelf)
            Hide(false);
    }

    public void OnDialogueEnded()
    {
        Debug.Log("OnDialogueEnded fired");
        isInDialogue = false;
    }


    private void ClickPerformed()
    {
        if (SoundRepository.Instance && SoundRepository.Instance.NPCInteractionClickedSound)
            SFXManager.GetInstance().PlayClip(SoundRepository.Instance.NPCInteractionClickedSound);

        interactableObject.Interact();
    }

}
