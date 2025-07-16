using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public int score = 0;
    public TextMeshProUGUI scoreText;

    void Start()
    {
        UpdateScoreText();
    }

    public void AddScore()
    {
        score++;
        UpdateScoreText();
    }

    void UpdateScoreText()
    {
        scoreText.text = "Score: " + score;
    }
}
