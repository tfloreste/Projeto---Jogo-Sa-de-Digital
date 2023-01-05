using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    [SerializeField] Text scoreText = null;

    private static ScoreManager instance = null;
    private int score = 0;

    public static ScoreManager GetInstance()
    {
        return instance;
    }

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("Mais de uma instância de ScoreManager");
        }

        instance = this;
    }

    private void Start()
    {
        ResetScore();
    }

    public void SetScore(int newScore)
    {
        this.score = newScore;
        UpdateScoreText();
    }

    public void IncreaseScore(int value)
    {
        SetScore(this.score + value);
    }

    public void IncreaseScore()
    {
        IncreaseScore(1);
    }

    public void ResetScore()
    {
        SetScore(0);
    }

    public int GetCurrentScore()
    {
        return this.score;
    }

    private void UpdateScoreText()
    {
        if(scoreText != null)
        {
            scoreText.text = this.score.ToString();
        }
    }
}
    
