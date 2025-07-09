using UnityEngine;

public class PlatformSpawner : MonoBehaviour
{
    public GameObject platformPrefab;
    public Transform playerTransform;
    public float spawnHeightOffset = 2.5f;
    public float fixedPlatformX = 0f;

    private float nextPlatformYPosition;

    
    public float difficultyIncreaseRate = 0.05f; 
    private float currentDifficultyMultiplier = 1f; 

    [Header("Moving Platform Settings")]
    public float minHeightForMovingPlatforms = 10f; 
    [Range(0f, 1f)] 
    public float movingPlatformChance = 0.3f; 
    public float chanceIncreaseRate = 0.02f; 

    void Start()
    {
        if (playerTransform == null)
        {
            playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        }

        nextPlatformYPosition = playerTransform.position.y - (spawnHeightOffset * 1.5f);

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

        
        bool isMoving = false;
        if (nextPlatformYPosition > minHeightForMovingPlatforms) 
        {
            
            float currentChance = movingPlatformChance + (chanceIncreaseRate * (nextPlatformYPosition / spawnHeightOffset - 5)); 
            currentChance = Mathf.Clamp01(currentChance); 

            if (Random.value < currentChance) 
            {
                isMoving = true;
            }
        }

  
        if (isMoving)
        {
            MovingPlatform movingPlatform = newPlatform.GetComponent<MovingPlatform>();
            if (movingPlatform != null)
            {
               
                float maxSpeedMultiplier = 3f; 
                float maxRangeMultiplier = 2f; 

                float actualSpeedMultiplier = Mathf.Min(currentDifficultyMultiplier, maxSpeedMultiplier);
                float actualRangeMultiplier = Mathf.Min(currentDifficultyMultiplier, maxRangeMultiplier);

                movingPlatform.SetDifficulty(actualSpeedMultiplier, actualRangeMultiplier);
                movingPlatform.enabled = true; 
            }
        }
        else
        {
            
            MovingPlatform movingPlatform = newPlatform.GetComponent<MovingPlatform>();
            if (movingPlatform != null)
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