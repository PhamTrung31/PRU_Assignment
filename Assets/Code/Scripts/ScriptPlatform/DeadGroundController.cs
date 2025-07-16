using UnityEngine;
using UnityEngine.SceneManagement;

public class DeadGroundController : MonoBehaviour
{
    public Transform playerTransform;
    // public float followOffset = 5f; // Không cần thiết nếu DeadGround chỉ di chuyển lên từ vị trí ban đầu
    public float moveSpeed = 0.5f;

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
        currentMoveSpeed += speedIncreaseRate * Time.deltaTime;

        // Di chuyển DeadGround lên trên
        transform.position = new Vector3(transform.position.x, transform.position.y + currentMoveSpeed * Time.deltaTime, transform.position.z);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Game Over! Player hit the Dead Ground.");
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}