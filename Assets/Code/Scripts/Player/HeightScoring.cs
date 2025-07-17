using UnityEngine;

public class HeightScoring : MonoBehaviour
{
    private float _highestY;
    private int _score;

    [SerializeField] private float _pointsPerUnit = 10f;
    [SerializeField] private ScoreDisplay scoreDisplay;

    public static HeightScoring Instance { get; private set; }
    private void Start()
    {
        _highestY = transform.position.y;
        LoadScore();
       // Set the singleton instance

    }
    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        float currentY = transform.position.y;

        if (currentY > _highestY)
        {
            float delta = currentY - _highestY;
            int pointsToAdd = Mathf.FloorToInt(delta * _pointsPerUnit);

            if (pointsToAdd > 0)
            {
                _score += pointsToAdd;
                _highestY = currentY;
                if (scoreDisplay != null)
                    scoreDisplay.UpdateScore(_score);

                int recordScore = PlayerPrefs.GetInt("RecordScore", 0);

                Debug.Log($"New Score: {_score}, Record Score: {recordScore}");
            }
        }
    }

    public void SaveScore()
    {
        int recordScore = PlayerPrefs.GetInt("RecordScore", 0);
        if (_score > recordScore)
        {
            PlayerPrefs.SetInt("RecordScore", _score);
            PlayerPrefs.Save();
            Debug.Log($"New Record Score: {_score}");
        }

        PlayerPrefs.SetInt("LastScore", _score);
        PlayerPrefs.Save();

        // Always log both
        Debug.Log($"Run finished. Your Score: {_score}, Record Score: {PlayerPrefs.GetInt("RecordScore", 0)}");
    }

    public void LoadScore()
    {
        // Load the last score (if you want to show it), or just ignore
        _score = 0; // Start fresh when loading

        // You can get the record score when displaying the UI:
        int recordScore = PlayerPrefs.GetInt("RecordScore", 0);
        Debug.Log($"Record Score: {recordScore}");
    }

    public void StartNewGame()
    {
        // Save current score before resetting
        SaveScore();

        // Reset current score
        _score = 0;
        _highestY = transform.position.y;

        Debug.Log("Game restarted. Score reset to 0.");
    }

    public int GetScore()
    {
        return _score;
    }
}
