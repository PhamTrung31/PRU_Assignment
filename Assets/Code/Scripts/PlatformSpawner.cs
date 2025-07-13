using UnityEngine;

public class PlatformSpawner : MonoBehaviour
{
    public GameObject platformPrefab;
    public Transform playerTransform;
    public float spawnHeightOffset = 2.5f;
    public float fixedPlatformX = 0f;

    private float nextPlatformYPosition;

    [Header("Global Difficulty Scaling")]
    public float baseDifficultyMultiplier = 1f; 
    public float difficultyIncreaseRate = 0.05f; 
    private float currentDifficultyMultiplier; 

    [Header("Level 1 Settings (Easy Phase)")]
    public float level1MaxHeight = 50f; 
                                        // Trong Level 1, movingPlatformChance sẽ bằng 0 cho đến minHeightForMovingPlatforms
                                        // Sau đó sẽ tăng dần với chanceIncreaseRate

    [Header("Level 2 Settings (Medium/Hard Phase)")]
    public float level2SpeedMultiplierFactor = 1.5f; // Tốc độ platform di chuyển x lần so với Level 1 khi vào Level 2
    public float level2RangeMultiplierFactor = 1.2f;  // Phạm vi platform di chuyển x lần so với Level 1 khi vào Level 2
    public float level2BaseMovingChance = 0.5f; // Xác suất tối thiểu của platform di chuyển trong Level 2
    public float level2ChanceIncreaseRate = 0.03f; // Tốc độ tăng xác suất trong Level 2

    [Header("Moving Platform Chance")]
    public float minHeightForMovingPlatforms = 10f; // Chiều cao tối thiểu mà platform di chuyển có thể xuất hiện
    [Range(0f, 1f)]
    public float baseMovingPlatformChance = 0.3f; // Xác suất ban đầu platform di chuyển xuất hiện (khi đạt minHeight)
    public float chanceIncreaseRate = 0.02f; // Tốc độ tăng xác suất mỗi lần spawn

    void Start()
    {
        if (playerTransform == null)
        {
            playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        }

        nextPlatformYPosition = playerTransform.position.y - (spawnHeightOffset * 1.5f);
        currentDifficultyMultiplier = baseDifficultyMultiplier; // Khởi tạo độ khó

        float currentSpawnY = nextPlatformYPosition;
        for (int i = 0; i < 5; i++)
        {
            
            SpawnSinglePlatform(currentSpawnY, 1f, 0f, false);
            currentSpawnY += spawnHeightOffset;
        }
        nextPlatformYPosition = currentSpawnY;
    }

    void Update()
    {
        if (playerTransform.position.y + spawnHeightOffset * 2 > nextPlatformYPosition)
        {
            SpawnNewPlatform();
        }
    }

    void SpawnNewPlatform()
    {
        Vector2 spawnPos = new Vector2(fixedPlatformX, nextPlatformYPosition);
        GameObject newPlatform = Instantiate(platformPrefab, spawnPos, Quaternion.identity);

        
        currentDifficultyMultiplier += difficultyIncreaseRate;

        // Tính toán xác suất di chuyển và hệ số độ khó cho platform cụ thể
        bool isMoving = false;
        float finalSpeedMultiplier = 1f;
        float finalRangeMultiplier = 0f; 

        float currentChance = 0f;
        if (nextPlatformYPosition < level1MaxHeight) // Level 1 Logic
        {
            if (nextPlatformYPosition > minHeightForMovingPlatforms)
            {
                currentChance = baseMovingPlatformChance + (chanceIncreaseRate * (nextPlatformYPosition - minHeightForMovingPlatforms) / spawnHeightOffset);
                currentChance = Mathf.Clamp01(currentChance);
            }
            finalSpeedMultiplier = currentDifficultyMultiplier;
            finalRangeMultiplier = currentDifficultyMultiplier; // Phạm vi cũng tăng nhẹ
        }
        else // Level 2 Logic (nextPlatformYPosition >= level1MaxHeight)
        {
            // Reset base difficulty for Level 2 or apply a boost
            finalSpeedMultiplier = currentDifficultyMultiplier * level2SpeedMultiplierFactor;
            finalRangeMultiplier = currentDifficultyMultiplier * level2RangeMultiplierFactor;

            // Xác suất di chuyển cao hơn trong Level 2
            currentChance = level2BaseMovingChance + (level2ChanceIncreaseRate * (nextPlatformYPosition - level1MaxHeight) / spawnHeightOffset);
            currentChance = Mathf.Clamp01(currentChance);
        }

        // Quyết định xem platform có di chuyển hay không dựa trên xác suất
        if (Random.value < currentChance)
        {
            isMoving = true;
        }

        // Áp dụng độ khó và trạng thái di chuyển cho platform
        MovingPlatform movingPlatform = newPlatform.GetComponent<MovingPlatform>();
        if (movingPlatform != null)
        {
            if (isMoving)
            {
                movingPlatform.SetDifficulty(finalSpeedMultiplier, finalRangeMultiplier);
                movingPlatform.enabled = true;
            }
            else
            {
                movingPlatform.enabled = false;
            }
        }

        nextPlatformYPosition += spawnHeightOffset;
    }

    void SpawnSinglePlatform(float yPos, float speedMultiplier, float rangeMultiplier, bool isMoving)
    {
        Vector2 spawnPos = new Vector2(fixedPlatformX, yPos);
        GameObject newPlatform = Instantiate(platformPrefab, spawnPos, Quaternion.identity);

        MovingPlatform movingPlatform = newPlatform.GetComponent<MovingPlatform>();
        if (movingPlatform != null)
        {
            movingPlatform.SetDifficulty(speedMultiplier, rangeMultiplier);
            movingPlatform.enabled = isMoving;
        }
    }
}