using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    public float moveSpeed = 2f;
    public float moveRange = 3f; // Phạm vi di chuyển từ vị trí khởi điểm

    private Vector2 startPosition;
    private float currentDirection = 1f; // 1f for right, -1f for left

    // Biến để kiểm soát liệu platform có đang di chuyển hay không
    private bool isMoving = true; // << THÊM BIẾN NÀY >>

    void Start()
    {
        startPosition = transform.position;
    }

    void Update()
    {
        // Chỉ di chuyển nếu isMoving là true
        if (isMoving) // << THÊM ĐIỀU KIỆN NÀY >>
        {
            // Calculate the target position based on direction
            Vector2 targetPosition = startPosition + new Vector2(moveRange * currentDirection, 0);

            // Move the platform towards the target position
            transform.position = Vector2.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

            // Check if the platform has reached the target position
            if (Vector2.Distance(transform.position, targetPosition) < 0.05f)
            {
                // Reverse direction
                currentDirection *= -1f;
            }
        }
    }

    // Hàm này được gọi bởi PlatformSpawner để thiết lập độ khó
    public void SetDifficulty(float speedMultiplier, float rangeMultiplier)
    {
        moveSpeed *= speedMultiplier;
        moveRange *= rangeMultiplier;
    }
}