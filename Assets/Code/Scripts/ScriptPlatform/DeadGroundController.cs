using UnityEngine;
using UnityEngine.SceneManagement;

public class DeadGroundController : MonoBehaviour
{
    public Transform playerTransform;
    // public float followOffset = 5f; // Không cần thiết nếu DeadGround chỉ di chuyển lên từ vị trí ban đầu
    public float moveSpeed = 0.5f;
    [Header("Advanced Tracking")]
    public float maxFollowDistance = 10f;
    public float catchUpSpeed = 1.5f;
    public float minFollowSpeed = 0.3f;
    [Header("Kill Pressure Settings")]
    public float waitBeforeKilling = 5f;
    private float waitTimer = 0f;

    [Header("Difficulty Scaling")]
    public float speedIncreaseRate = 0.01f;
    private float currentMoveSpeed;

    // << THÊM THAM CHIẾU STARTING GROUND >>
    public GameObject startingGround; // Kéo GameObject StartingGround vào đây trong Inspector
    public float initialOffsetBelowStartingGround = 5f; // Khoảng cách ban đầu DeadGround cách StartingGround
    // << KẾT THÚC THÊM THAM CHIẾU >>

    void Start()
    {
        if (playerTransform == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                playerTransform = player.transform;
            }
            else
            {
                Debug.LogError("Player GameObject with Tag 'Player' not found!");
            }
        }

        // << THÊM KIỂM TRA STARTING GROUND VÀ THIẾT LẬP VỊ TRÍ BAN ĐẦU >>
        if (startingGround == null)
        {
            Debug.LogError("StartingGround not assigned! Please assign the StartingGround GameObject in DeadGroundController's Inspector.");
            enabled = false;
            return;
        }

        // Đặt vị trí Y ban đầu của DeadGround dưới StartingGround một khoảng nhất định
        transform.position = new Vector3(transform.position.x, startingGround.transform.position.y - initialOffsetBelowStartingGround, transform.position.z);
        // << KẾT THÚC THAY ĐỔI >>

        currentMoveSpeed = moveSpeed;
    }

    void Update()
    {
        if (playerTransform == null) return;

        float distanceToPlayer = playerTransform.position.y - transform.position.y;

        if (distanceToPlayer > maxFollowDistance)
        {
            // Quá xa → tăng tốc để đuổi kịp
            waitTimer = 0f;
            transform.position += Vector3.up * catchUpSpeed * Time.deltaTime;
        }
        else
        {
            // Đã gần player → bắt đầu đếm thời gian
            waitTimer += Time.deltaTime;

            if (waitTimer < waitBeforeKilling)
            {
                // Trong thời gian chờ → đứng yên hoặc bò chậm
                transform.position += Vector3.up * minFollowSpeed * Time.deltaTime;
            }
            else
            {
                // Đủ thời gian chờ rồi → bắt đầu tiến lên để giết
                transform.position += Vector3.up * catchUpSpeed * Time.deltaTime;
            }
        }
    }
}