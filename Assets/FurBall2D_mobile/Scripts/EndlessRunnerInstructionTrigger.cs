using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndlessRunnerInstructionTrigger : MonoBehaviour
{
    [SerializeField] private GameObject intructionsGameObject;

    private Animator instructionsAnimator;
    private AudioSource thisAudioSource;

    private void Start()
    {
        instructionsAnimator = intructionsGameObject.GetComponent<Animator>();
        thisAudioSource = GetComponent<AudioSource>();
        intructionsGameObject.SetActive(false);
    }

    private void OnEnable()
    {
        EndlessRunnerManager.Instance.onGameOver += CloseInstructions;
    }

    private void OnDisable()
    {
        if(EndlessRunnerManager.HasInstance())
            EndlessRunnerManager.Instance.onGameOver -= CloseInstructions;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            intructionsGameObject.SetActive(true);
            InputManager.OnTouchStart += CloseInstructions;
        }
    }

    private void CloseInstructions()
    {
        InputManager.OnTouchStart -= CloseInstructions;
        EndlessRunnerManager.Instance.onGameOver -= CloseInstructions;

        StartCoroutine(CloseInstructionsCO());
    }

    private IEnumerator CloseInstructionsCO()
    {
        if (!instructionsAnimator || !intructionsGameObject.activeSelf)
            yield break;

        if (!EndlessRunnerManager.Instance.gameOver && thisAudioSource)
            thisAudioSource.Play();
        
        if (instructionsAnimator)
        {
            instructionsAnimator.SetTrigger("Close");
            yield return new WaitForSeconds(1.0f);
        }

        intructionsGameObject.SetActive(false);
    }

}
