using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EndlessRunnerGoals : MonoBehaviour
{
    [SerializeField] private Animator goalPanelAnimator;
    [SerializeField] private TextMeshProUGUI goalText;

    private Animator goalTextAnimator;
    private AudioSource thisAudioSource;

    private bool goalEnabled = false;
    private bool goalCompleted = false;

    // Start is called before the first frame update
    void Start()
    {
        goalTextAnimator = goalText.gameObject.GetComponent<Animator>();
        thisAudioSource = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            StartCoroutine(ShowCurrentGoal());
        }
    }

    private IEnumerator ShowCurrentGoal()
    {
        int pointsToWin = EndlessRunnerManager.Instance.pointsToWin;
        goalText.text = "<b>OBJETIVO:</b> Consiga <b>" + pointsToWin + "</b> pontos";

        goalPanelAnimator.SetTrigger("Open");
        goalTextAnimator.SetTrigger("Open");
        
        yield return new WaitForSeconds(0.5f);

        goalEnabled = true;
    }

    private IEnumerator GoalCompleted()
    {
        goalPanelAnimator.SetTrigger("Completed");

        if (thisAudioSource)
            thisAudioSource.Play();

        yield return new WaitForSeconds(0.5f);
        yield return CloseGoalPanel();
    }

    private IEnumerator CloseGoalPanel()
    {
        goalPanelAnimator.SetTrigger("Close");
        goalTextAnimator.SetTrigger("Close");

        yield return new WaitForSeconds(0.5f);
    }

    // Update is called once per frame
    void Update()
    {
        if (!goalEnabled || goalCompleted)
            return;

        if(ScoreManager.GetInstance().GetCurrentScore() >= EndlessRunnerManager.Instance.pointsToWin)
        {
            goalCompleted = true;
            StartCoroutine(GoalCompleted());
        }
    }
}
