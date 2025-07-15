using UnityEngine;
using UnityEngine.SceneManagement;

public class SawBladeController : MonoBehaviour
{
    public float moveSpeed = 3f; // Tốc độ di chuyển cơ bản của lưỡi cưa (sẽ giảm đi 1 nửa)
    public float moveRange = 2f; // Phạm vi di chuyển tối đa từ tâm platform (nếu muốn bao phủ toàn bộ)
    public bool startMovingRight = true; // Lưỡi cưa bắt đầu di chuyển sang phải hay trái

    private float currentDirection; // 1f for right, -1f for left
    private bool isMoving = true; // Biến cờ để kiểm soát việc di chuyển

    // Các biến để giới hạn phạm vi di chuyển (sẽ là local X so với Platform cha)
    private float minMovementX;
    private float maxMovementX;

    void Start()
    {
        // Giảm tốc độ di chuyển xuống một nửa
        moveSpeed /= 2f;

        currentDirection = startMovingRight ? 1f : -1f;

        // Tính toán giới hạn di chuyển dựa trên kích thước của platform cha
        Transform parentPlatform = transform.parent;
        if (parentPlatform != null)
        {
            SpriteRenderer parentSprite = parentPlatform.GetComponent<SpriteRenderer>();
            if (parentSprite != null)
            {
                // Lấy kích thước thực tế của platform sau khi đã scale
                // Sử dụng bounds.size.x của SpriteRenderer của platform cha
                // Convert bounds từ World Space sang Local Space của parentPlatform để tính toán biên
                // parentSprite.bounds là World Bounds, chúng ta cần chuyển nó về Local Space của parentPlatform
                Vector3 parentBoundsMinLocal = parentPlatform.InverseTransformPoint(parentSprite.bounds.min);
                Vector3 parentBoundsMaxLocal = parentPlatform.InverseTransformPoint(parentSprite.bounds.max);

                // Lấy độ rộng của sprite của lưỡi cưa để tính toán lề an toàn
                SpriteRenderer bladeSprite = GetComponent<SpriteRenderer>();
                float bladeHalfWidth = (bladeSprite != null) ? bladeSprite.bounds.extents.x : 0.05f; // Đặt một giá trị nhỏ mặc định nếu không có SpriteRenderer trên Blade

                // Các điểm giới hạn cục bộ X cho lưỡi cưa
                // Đảm bảo lưỡi cưa nằm hoàn toàn trên platform
                minMovementX = parentBoundsMinLocal.x + bladeHalfWidth;
                maxMovementX = parentBoundsMaxLocal.x - bladeHalfWidth;

                // Điều chỉnh moveRange để đảm bảo nó không vượt quá chiều rộng có sẵn của platform
                float totalAvailableRange = maxMovementX - minMovementX;
                // Nếu moveRange được đặt lớn hơn nửa chiều rộng có thể di chuyển, giới hạn lại
                if (moveRange > totalAvailableRange / 2f)
                {
                    moveRange = totalAvailableRange / 2f;
                }

                // Thiết lập vị trí cục bộ ban đầu của SawBlade.
                // Để nó bắt đầu di chuyển từ một trong hai biên của phạm vi được phép.
                if (currentDirection > 0) // Bắt đầu di chuyển sang phải
                {
                    // Lưỡi cưa bắt đầu từ biên trái của phạm vi di chuyển của nó
                    transform.localPosition = new Vector2(minMovementX, transform.localPosition.y);
                }
                else // Bắt đầu di chuyển sang trái
                {
                    // Lưỡi cưa bắt đầu từ biên phải của phạm vi di chuyển của nó
                    transform.localPosition = new Vector2(maxMovementX, transform.localPosition.y);
                }

                // Debug Logs để kiểm tra giá trị
                Debug.Log($"SawBladeController: Platform Size (local) X: {parentBoundsMaxLocal.x - parentBoundsMinLocal.x}");
                Debug.Log($"SawBladeController: Blade Half Width: {bladeHalfWidth}");
                Debug.Log($"SawBladeController: minMovementX: {minMovementX}, maxMovementX: {maxMovementX}");
                Debug.Log($"SawBladeController: Initial localPosition: {transform.localPosition}");
                Debug.Log($"SawBladeController: Adjusted moveRange: {moveRange}");
            }
            else
            {
                Debug.LogWarning("SawBladeController: Parent platform does not have a SpriteRenderer. Cannot set dynamic move range for " + gameObject.name + ". Ensure your SawPlatform_Long prefab has a SpriteRenderer component on the parent object.", this);
                // Fallback: Nếu không tìm thấy SpriteRenderer, sử dụng moveRange mặc định
                minMovementX = transform.localPosition.x - moveRange;
                maxMovementX = transform.localPosition.x + moveRange;
            }
        }
        else
        {
            Debug.LogWarning("SawBladeController: No parent platform found. Using default move range relative to initial position for " + gameObject.name, this);
            // Fallback: Sử dụng moveRange mặc định nếu không có platform cha
            minMovementX = transform.localPosition.x - moveRange;
            maxMovementX = transform.localPosition.x + moveRange;
        }
    }

    void Update()
    {
        if (isMoving)
        {
            // Tính toán vị trí đích dựa trên hướng hiện tại và giới hạn minMovementX/maxMovementX
            float targetX;
            if (currentDirection > 0) // Di chuyển sang phải
            {
                targetX = maxMovementX;
            }
            else // Di chuyển sang trái
            {
                targetX = minMovementX;
            }

            Vector2 targetPosition = new Vector2(targetX, transform.localPosition.y); // Giữ nguyên Y

            // Di chuyển lưỡi cưa về phía vị trí đích
            transform.localPosition = Vector2.MoveTowards(transform.localPosition, targetPosition, moveSpeed * Time.deltaTime);

            // Kiểm tra xem lưỡi cưa đã đạt đến vị trí đích chưa
            // Sử dụng Mathf.Abs để so sánh gần đúng, tránh lỗi float
            if (Mathf.Abs(transform.localPosition.x - targetX) < 0.01f)
            {
                // Đảo ngược hướng
                currentDirection *= -1f;
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Game Over! Player hit the Saw Blade.");
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    public void StopSaw()
    {
        isMoving = false;
        Debug.Log("Saw Blade stopped!");
    }

    public void StartSaw()
    {
        isMoving = true;
        Debug.Log("Saw Blade started!");
    }
}