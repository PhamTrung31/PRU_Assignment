using UnityEngine;
using System.Collections;

public class PlatformSpawner_Lv2 : MonoBehaviour // Đổi tên class thành PlatformSpawner_Lv2
{
    public GameObject[] platformPrefabs;
    public GameObject[] disappearingPlatformPrefabs;
    public GameObject[] sawPlatformPrefabs;
    // START - THÊM BIẾN CHO ROTATING PLATFORM
    public GameObject[] rotatingPlatformPrefabs;
    // END - THÊM BIẾN CHO ROTATING PLATFORM

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

    // START - THÊM HEADER VÀ BIẾN CHO ROTATING PLATFORM
    [Header("Rotating Platform Settings")]
    [Range(0f, 1f)]
    public float rotatingPlatformChance = 0.2f; // Xác suất xuất hiện platform xoay (điều chỉnh theo ý bạn)
    public float minHeightForRotatingPlatforms = 50f; // Chiều cao tối thiểu để platform xoay bắt đầu xuất hiện
    // END - THÊM HEADER VÀ BIẾN CHO ROTATING PLATFORM

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
        // START - KIỂM TRA ROTATING PLATFORM PREFABS
        if (rotatingPlatformPrefabs == null || rotatingPlatformPrefabs.Length == 0)
        {
            Debug.LogWarning("Rotating Platform Prefabs array is empty. Rotating platforms will not spawn.");
        }
        // END - KIỂM TRA ROTATING PLATFORM PREFABS

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
        bool isRotating = false; // Biến cờ mới cho rotating platform
        bool isMoving = false; // Biến cờ cho moving platform

        float randomChance = Random.value;
        float currentHeight = nextPlatformYPosition; // Dùng biến này cho dễ đọc

        // Tính toán xác suất cho Moving Platform
        float currentMovingChance = 0f;
        float finalSpeedMultiplier = 1f;
        float finalRangeMultiplier = 0f;

        if (currentHeight < level1MaxHeight)
        {
            if (currentHeight > minHeightForMovingPlatforms)
            {
                currentMovingChance = baseMovingPlatformChance + (chanceIncreaseRate * (currentHeight - minHeightForMovingPlatforms) / spawnHeightOffset);
                currentMovingChance = Mathf.Clamp01(currentMovingChance);
            }
            finalSpeedMultiplier = currentDifficultyMultiplier;
            finalRangeMultiplier = currentDifficultyMultiplier;
        }
        else // Level 2 phase (hoặc cao hơn Level 1 Max Height)
        {
            finalSpeedMultiplier = currentDifficultyMultiplier * level2SpeedMultiplierFactor;
            finalRangeMultiplier = currentDifficultyMultiplier * level2RangeMultiplierFactor;

            currentMovingChance = level2BaseMovingChance + (level2ChanceIncreaseRate * (currentHeight - level1MaxHeight) / spawnHeightOffset);
            currentMovingChance = Mathf.Clamp01(currentMovingChance);
        }

        // Quyết định loại platform
        // Ưu tiên Saw Platform (cao nhất)
        if (sawPlatformPrefabs != null && sawPlatformPrefabs.Length > 0 &&
            currentHeight >= minHeightForSawPlatforms &&
            randomChance < sawPlatformChance)
        {
            isSawPlatform = true;
            chosenPrefab = GetRandomSawPlatformPrefab();
        }
        // Ưu tiên Disappearing Platform
        else if (disappearingPlatformPrefabs != null && disappearingPlatformPrefabs.Length > 0 &&
                 currentHeight >= minHeightForDisappearingPlatforms &&
                 randomChance < sawPlatformChance + disappearingPlatformChance)
        {
            isDisappearing = true;
            chosenPrefab = GetRandomDisappearingPlatformPrefab();
        }
        // START - ƯU TIÊN ROTATING PLATFORM
        else if (rotatingPlatformPrefabs != null && rotatingPlatformPrefabs.Length > 0 &&
                 currentHeight >= minHeightForRotatingPlatforms &&
                 randomChance < sawPlatformChance + disappearingPlatformChance + rotatingPlatformChance)
        {
            isRotating = true;
            chosenPrefab = GetRandomRotatingPlatformPrefab();
        }
        // END - ƯU TIÊN ROTATING PLATFORM
        // Ưu tiên Moving Platform nếu không phải loại đặc biệt nào trên
        else if (currentHeight >= minHeightForMovingPlatforms &&
                 Random.value < currentMovingChance) // Sử dụng Random.value riêng cho moving chance
        {
            isMoving = true;
            chosenPrefab = GetRandomPlatformPrefab(true); // GetRandomPlatformPrefab(true) để chọn loại có thể di chuyển
        }
        // Mặc định là Regular Platform
        else
        {
            chosenPrefab = GetRandomPlatformPrefab(false); // GetRandomPlatformPrefab(false) để chọn loại không di chuyển
        }

        Vector2 spawnPos = new Vector2(fixedPlatformX, nextPlatformYPosition);
        GameObject newPlatform = Instantiate(chosenPrefab, spawnPos, Quaternion.identity);

        currentDifficultyMultiplier += difficultyIncreaseRate;

        // Áp dụng các controllers tùy thuộc vào loại platform được chọn
        MovingPlatform movingPlatform = newPlatform.GetComponent<MovingPlatform>();
        if (movingPlatform != null)
        {
            if (isMoving) // Chỉ kích hoạt nếu nó được chọn là moving platform
            {
                movingPlatform.SetDifficulty(finalSpeedMultiplier, finalRangeMultiplier);
                movingPlatform.enabled = true;
            }
            else // Nếu không, đảm bảo nó bị tắt
            {
                movingPlatform.enabled = false;
            }
        }

        DisappearingPlatform disappearingController = newPlatform.GetComponent<DisappearingPlatform>();
        if (disappearingController != null)
        {
            disappearingController.enabled = isDisappearing;
        }

        SawPlatformController sawController = newPlatform.GetComponent<SawPlatformController>();
        if (sawController != null)
        {
            sawController.enabled = isSawPlatform;
        }

        // Không cần kích hoạt RotatingPlatform component ở đây
        // vì script RotatingPlatform sẽ tự Start() khi Instantiate.
        // Bạn chỉ cần đảm bảo rằng prefab đã có script RotatingPlatform và nó tự xử lý.

        nextPlatformYPosition += spawnHeightOffset;
    }

    private GameObject GetRandomPlatformPrefab(bool allowMovingPlatforms)
    {
        if (platformPrefabs == null || platformPrefabs.Length == 0)
        {
            return null;
        }
        // Nếu cho phép moving platforms và có moving platform trong mảng platformPrefabs
        // (Giả định rằng bạn có các prefab riêng cho moving và non-moving trong platformPrefabs hoặc xử lý bên trong prefab)
        // Cách triển khai hiện tại của bạn trong GetRandomPlatformPrefab không phân biệt moving/non-moving prefab.
        // Nó chỉ trả về ngẫu nhiên từ platformPrefabs.
        // Logic `isMoving` trong SpawnNewPlatform() sẽ quyết định việc kích hoạt MovingPlatform component.
        int randomIndex = Random.Range(0, platformPrefabs.Length);
        return platformPrefabs[randomIndex];
    }

    private GameObject GetRandomDisappearingPlatformPrefab()
    {
        if (disappearingPlatformPrefabs == null || disappearingPlatformPrefabs.Length == 0)
        {
            Debug.LogWarning("No disappearing platform prefabs assigned, spawning a regular platform instead.");
            return GetRandomPlatformPrefab(false); // Fallback to a non-moving regular platform
        }
        int randomIndex = Random.Range(0, disappearingPlatformPrefabs.Length);
        return disappearingPlatformPrefabs[randomIndex];
    }

    private GameObject GetRandomSawPlatformPrefab()
    {
        if (sawPlatformPrefabs == null || sawPlatformPrefabs.Length == 0)
        {
            Debug.LogWarning("No saw platform prefabs assigned, spawning a regular platform instead.");
            return GetRandomPlatformPrefab(false); // Fallback to a non-moving regular platform
        }
        int randomIndex = Random.Range(0, sawPlatformPrefabs.Length);
        return sawPlatformPrefabs[randomIndex];
    }

    // START - THÊM HÀM MỚI CHO ROTATING PLATFORM
    private GameObject GetRandomRotatingPlatformPrefab()
    {
        if (rotatingPlatformPrefabs == null || rotatingPlatformPrefabs.Length == 0)
        {
            Debug.LogWarning("No rotating platform prefabs assigned, spawning a regular platform instead.");
            return GetRandomPlatformPrefab(false); // Fallback to regular platform (non-moving)
        }
        int randomIndex = Random.Range(0, rotatingPlatformPrefabs.Length);
        return rotatingPlatformPrefabs[randomIndex];
    }
    // END - THÊM HÀM MỚI CHO ROTATING PLATFORM

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