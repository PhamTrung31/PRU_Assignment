using UnityEngine;
using System.Collections;
using TMPro;

public class ScoreDisplay : MonoBehaviour
{
    public TextMeshProUGUI scoreText;

    private int currentScore = 0;
    private Coroutine updateRoutine;

    public void UpdateScore(int newScore)
    {
        if (updateRoutine != null)
        {
            StopCoroutine(updateRoutine);
        }
        updateRoutine = StartCoroutine(AnimateScoreChange(newScore));
    }

    private IEnumerator AnimateScoreChange(int targetScore)
    {
        int displayedScore = currentScore;
        float duration = 0.3f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            int interpolatedScore = Mathf.FloorToInt(Mathf.Lerp(displayedScore, targetScore, t));
            scoreText.text = interpolatedScore.ToString("D4");
            yield return null;
        }

        currentScore = targetScore;
        scoreText.text = currentScore.ToString("D4");
    }
}
