using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float jumpForce = 10f;
    public float horizontalMoveSpeed = 5f;

    [Header("Emergency Jump Settings")] // << THÊM HEADER NÀY >>
    public float emergencyJumpMultiplier = 2.0f; // Hệ số nhân lực nhảy cho cú nhảy cứu sinh (ví dụ: 2x jumpForce)
    public float emergencyJumpMinY = -10f; // Chiều cao Y tối thiểu để kích hoạt cú nhảy cứu sinh
                                           // (Player rơi xuống dưới ngưỡng này sẽ có thể dùng cú nhảy đặc biệt)
    public float emergencyJumpCooldown = 1.0f; // Thời gian cooldown cho cú nhảy cứu sinh
    private float lastEmergencyJumpTime; // Thời điểm cú nhảy cứu sinh cuối cùng

    private Rigidbody2D rb;
    private bool isGrounded; // Biến này kiểm tra xem Player có đang chạm đất/platform không

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        isGrounded = true;
        lastEmergencyJumpTime = -emergencyJumpCooldown; // Khởi tạo để có thể nhảy ngay lập tức
    }

    void Update()
    {
        float currentJumpForce = jumpForce;

        // << LOGIC CHO CÚ NHẢY CỨU SINH >>
        // Kiểm tra xem Player có đang ở dưới ngưỡng nguy hiểm không
        // VÀ có đủ thời gian cooldown không
        // VÀ KHÔNG ĐANG CHẠM ĐẤT (để không dùng cú nhảy khẩn cấp trên platform)
        if (transform.position.y < emergencyJumpMinY && !isGrounded && Time.time >= lastEmergencyJumpTime + emergencyJumpCooldown)
        {
            // Debug.Log("Player in emergency zone! Can perform emergency jump."); // Để kiểm tra
            currentJumpForce *= emergencyJumpMultiplier; // Tăng lực nhảy
        }
        // << KẾT THÚC LOGIC CÚ NHẢY CỨU SINH >>


        // Logic nhảy
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, currentJumpForce); // Sử dụng currentJumpForce
            isGrounded = false;
        }
        // << CHO PHÉP NHẢY KHẨN CẤP NGAY CẢ KHI KHÔNG CHẠM ĐẤT >>
        else if (Input.GetButtonDown("Jump") && transform.position.y < emergencyJumpMinY && !isGrounded && Time.time >= lastEmergencyJumpTime + emergencyJumpCooldown)
        {
            // Chỉ nhảy khẩn cấp nếu đang trong vùng nguy hiểm và không chạm đất
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, currentJumpForce); // currentJumpForce đã được nhân đôi
            lastEmergencyJumpTime = Time.time; // Cập nhật thời gian sử dụng
            // Debug.Log("Emergency Jump Activated!"); // Để kiểm tra
        }
        // << KẾT THÚC NHẢY KHẨN CẤP >>


        // Logic di chuyển ngang A/D
        float moveInput = Input.GetAxis("Horizontal");
        Vector2 currentVelocity = rb.linearVelocity;
        currentVelocity.x = moveInput * horizontalMoveSpeed;
        rb.linearVelocity = currentVelocity;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Kiểm tra xem đối tượng va chạm có tag "Platform" hoặc "StartingGround" không
        if (collision.gameObject.CompareTag("Platform") || collision.gameObject.CompareTag("StartingGround"))
        {
            isGrounded = true;
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Platform") || collision.gameObject.CompareTag("StartingGround"))
        {
            isGrounded = false;
        }
    }
}