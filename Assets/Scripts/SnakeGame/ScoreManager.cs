using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;

    public TMP_Text scoreText;
    public TMP_Text controlScoreText;

    public float gameTime = 60f;

    private bool isGameOver = false;

    void Awake()
    {
        Instance = this;
    }

    void Update()
    {
        if (isGameOver || Snake.Instance == null) return;

        gameTime -= Time.deltaTime;
        if (gameTime <= 0f)
        {
            gameTime = 0f;
            isGameOver = true;
            ShowFinalScores();
        }

        UpdateUI();
    }

    void UpdateUI()
    {
        scoreText.text = $"Score: {Snake.Instance.foodCollected}";
        controlScoreText.text = $"Control Score: {Snake.Instance.GetSnakeScore():0.00}";
    }

    void ShowFinalScores()
    {
        Debug.Log("Game Over");
        Debug.Log("Final Score: " + Snake.Instance.foodCollected);
        Debug.Log("Control Score: " + Snake.Instance.GetSnakeScore().ToString("0.00"));
    }
}
