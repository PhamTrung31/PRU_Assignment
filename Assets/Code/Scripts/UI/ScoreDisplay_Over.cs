using TMPro;
using UnityEngine;

public class ScoreDisplay_Over : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI recordText;

    private void Start()
    {
        float lastScore = PlayerPrefs.GetInt("LastScore", 0);
        float bestScore = PlayerPrefs.GetInt("RecordScore", 0);

        scoreText.text = $"Score: {lastScore:0}";
        recordText.text = $"Record: {bestScore:0}";
    }
}
