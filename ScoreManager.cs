using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;

    public Text currentScoreText;

    private int currentScore = 0;

    private void Awake()
    {
        Instance = this;
    }

    public void AddScore(int points)
    {
        currentScore += points;

        if (currentScoreText != null)
        {
            currentScoreText.text = "Очки: " + currentScore.ToString();
        }

        //побит ли рекорд, сохраненный в PlayerPrefs?
        int currentHighScore = PlayerPrefs.GetInt("HighScore", 0);
        if (currentScore > currentHighScore)
        {
            //перезаписываем рекорд в памяти
            PlayerPrefs.SetInt("HighScore", currentScore);
        }
    }
}
