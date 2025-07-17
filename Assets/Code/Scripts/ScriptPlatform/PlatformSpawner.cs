using UnityEngine;
using System.Collections;

public class PlatformSpawner : MonoBehaviour
{
    public GameObject[] platformPrefabs;
    public GameObject[] disappearingPlatformPrefabs;
    public GameObject[] sawPlatformPrefabs;

    public Transform playerTransform;
    public float spawnHeightOffset = 2.5f;
    public float fixedPlatformX = 0f;
    public GameObject startingGround;

    public DeadGroundController deadGroundController;

    private float nextPlatformYPosition;

    [Header("Disappearing Platform Settings")]
    [Range(0f, 1f)]
    public float disappearingPlatformChance = 0.1f;
    public float minHeightForDisappearingPlatforms = 20f;

    [Header("Saw Platform Settings")]
    [Range(0f, 1f)]
    public float sawPlatformChance = 0.1f;
    public float minHeightForSawPlatforms = 30f;

    [Header("Global Difficulty Scaling")]
    public float baseDifficultyMultiplier = 1f;
    public float difficultyIncreaseRate = 0.05f;
    private float currentDifficultyMultiplier;

    [Header("Level 1 Settings (Easy Phase)")]
    public float level1MaxHeight = 50f;

    [Header("Level 2 Settings (Medium/Hard Phase)")]
    public float level2SpeedMultiplierFactor = 1.5f;
    public float level2RangeMultiplierFactor = 1.2f;
    public float level2BaseMovingChance = 0.5f;
    public float level2ChanceIncreaseRate = 0.03f;

    [Header("Moving Platform Chance")]
    public float minHeightForMovingPlatforms = 10f;
    [Range(0f, 1f)]
    public float baseMovingPlatformChance = 0.3f;
    public float chanceIncreaseRate = 0.02f;

    [Header("Collectible Item Prefabs")]
    public GameObject heartItemPrefab;
    public GameObject boostItemPrefab;
    public GameObject shieldItemPrefab;

    [Range(0f, 1f)]
    public float itemSpawnChance = 0.07f;

    [Header("Individual Item Spawn Weights")]
    [Range(0f, 1f)]
    public float heartWeight = 0.6f;
    [Range(0f, 1f)]
    public float boostWeight = 0.25f;
    [Range(0f, 1f)]
    public float shieldWeight = 0.15f;

    private GameObject lastPlatform;

    void Start()
    {
        if (playerTransform == null)
        {
            playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        }

        if (platformPrefabs == null || platformPrefabs.Length == 0)
        {
            Debug.LogError("Platform Prefabs array is empty! Please assign platform prefabs (e.g., Short, Medium, Long) in the Inspector.");
            enabled = false;
            return;
        }

        if (disappearingPlatformPrefabs == null || disappearingPlatformPrefabs.Length == 0)
        {
            Debug.LogWarning("Disappearing Platform Prefabs array is empty. Disappearing platforms will not spawn.");
        }
        if (sawPlatformPrefabs == null || sawPlatformPrefabs.Length == 0)
        {
            Debug.LogWarning("Saw Platform Prefabs array is empty. Saw platforms will not spawn.");
        }

        if (startingGround == null)
        {
            Debug.LogError("StartingGround not assigned! Please assign the StartingGround GameObject in the Inspector.");
            enabled = false;
            return;
        }

        if (deadGroundController == null)
        {
            deadGroundController = FindObjectOfType<DeadGroundController>();
            if (deadGroundController == null)
            {
                Debug.LogError("DeadGroundController not found in the scene! Dead Ground will not start.");
            }
        }

        nextPlatformYPosition = startingGround.transform.position.y + (spawnHeightOffset * 1.5f);
        currentDifficultyMultiplier = baseDifficultyMultiplier;

        float currentSpawnY = nextPlatformYPosition;
        for (int i = 0; i < 5; i++)
        {
            GameObject chosenPrefab = GetRandomPlatformPrefab(false);
            GameObject newPlatform = SpawnSinglePlatform(chosenPrefab, currentSpawnY, 1f, 0f, false);
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
        GameObject chosenPrefab;
        bool isDisappearing = false;
        bool isSawPlatform = false;

        float randomChance = Random.value;

        if (sawPlatformPrefabs != null && sawPlatformPrefabs.Length > 0 &&
            nextPlatformYPosition > minHeightForSawPlatforms &&
            randomChance < sawPlatformChance)
        {
            isSawPlatform = true;
            chosenPrefab = GetRandomSawPlatformPrefab();
        }
        else if (disappearingPlatformPrefabs != null && disappearingPlatformPrefabs.Length > 0 &&
                 nextPlatformYPosition > minHeightForDisappearingPlatforms &&
                 randomChance < disappearingPlatformChance + sawPlatformChance)
        {
            isDisappearing = true;
            chosenPrefab = GetRandomDisappearingPlatformPrefab();
        }
        else
        {
            chosenPrefab = GetRandomPlatformPrefab(true);
        }

        Vector2 spawnPos = new Vector2(fixedPlatformX, nextPlatformYPosition);
        GameObject newPlatform = Instantiate(chosenPrefab, spawnPos, Quaternion.identity);

        currentDifficultyMultiplier += difficultyIncreaseRate;

        bool isMoving = false;
        if (!isDisappearing && !isSawPlatform)
        {
            float finalSpeedMultiplier = 1f;
            float finalRangeMultiplier = 0f;
            float currentChance = 0f;

            if (nextPlatformYPosition < level1MaxHeight)
            {
                if (nextPlatformYPosition > minHeightForMovingPlatforms)
                {
                    currentChance = baseMovingPlatformChance + (chanceIncreaseRate * (nextPlatformYPosition - minHeightForMovingPlatforms) / spawnHeightOffset);
                    currentChance = Mathf.Clamp01(currentChance);
                }
                finalSpeedMultiplier = currentDifficultyMultiplier;
                finalRangeMultiplier = currentDifficultyMultiplier;
            }
            else
            {
                finalSpeedMultiplier = currentDifficultyMultiplier * level2SpeedMultiplierFactor;
                finalRangeMultiplier = currentDifficultyMultiplier * level2RangeMultiplierFactor;

                currentChance = level2BaseMovingChance + (level2ChanceIncreaseRate * (nextPlatformYPosition - level1MaxHeight) / spawnHeightOffset);
                currentChance = Mathf.Clamp01(currentChance);
            }

            if (Random.value < currentChance)
            {
                isMoving = true;
            }

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
        }
        nextPlatformYPosition += spawnHeightOffset;

        if (lastPlatform != null && Random.value < itemSpawnChance)
        {
            Vector2 pos1 = lastPlatform.transform.position;
            Vector2 pos2 = newPlatform.transform.position;
            Vector2 middlePos = (pos1 + pos2) / 2f;

            SpawnRandomItemAt(middlePos);
        }

        lastPlatform = newPlatform;
    }

    void SpawnRandomItemAt(Vector2 position)
    {
        if (heartItemPrefab == null || boostItemPrefab == null || shieldItemPrefab == null)
        {
            Debug.LogWarning("One or more item prefabs are not assigned.");
            return;
        }

        float totalWeight = heartWeight + boostWeight + shieldWeight;

        float randomValue = Random.value * totalWeight;

        GameObject prefabToSpawn = null;

        if (randomValue < heartWeight)
        {
            prefabToSpawn = heartItemPrefab;
        }
        else if (randomValue < heartWeight + boostWeight)
        {
            prefabToSpawn = boostItemPrefab;
        }
        else
        {
            prefabToSpawn = shieldItemPrefab;
        }

        Instantiate(prefabToSpawn, position, Quaternion.identity);
    }

    private GameObject GetRandomPlatformPrefab(bool allowMovingPlatforms)
    {
        if (platformPrefabs == null || platformPrefabs.Length == 0)
        {
            return null;
        }
        int randomIndex = Random.Range(0, platformPrefabs.Length);
        return platformPrefabs[randomIndex];
    }

    private GameObject GetRandomDisappearingPlatformPrefab()
    {
        if (disappearingPlatformPrefabs == null || disappearingPlatformPrefabs.Length == 0)
        {
            Debug.LogWarning("No disappearing platform prefabs assigned, spawning a regular platform instead.");
            return GetRandomPlatformPrefab(true);
        }
        int randomIndex = Random.Range(0, disappearingPlatformPrefabs.Length);
        return disappearingPlatformPrefabs[randomIndex];
    }

    private GameObject GetRandomSawPlatformPrefab()
    {
        if (sawPlatformPrefabs == null || sawPlatformPrefabs.Length == 0)
        {
            Debug.LogWarning("No saw platform prefabs assigned, spawning a regular platform instead.");
            return GetRandomPlatformPrefab(true);
        }
        int randomIndex = Random.Range(0, sawPlatformPrefabs.Length);
        return sawPlatformPrefabs[randomIndex];
    }

    GameObject SpawnSinglePlatform(GameObject prefabToSpawn, float yPos, float speedMultiplier, float rangeMultiplier, bool isMoving)
    {
        Vector2 spawnPos = new Vector2(fixedPlatformX, yPos);
        GameObject newPlatform = Instantiate(prefabToSpawn, spawnPos, Quaternion.identity);

        MovingPlatform movingPlatform = newPlatform.GetComponent<MovingPlatform>();
        if (movingPlatform != null)
        {
            movingPlatform.SetDifficulty(speedMultiplier, rangeMultiplier);
            movingPlatform.enabled = isMoving;
        }
        return newPlatform;
    }
}