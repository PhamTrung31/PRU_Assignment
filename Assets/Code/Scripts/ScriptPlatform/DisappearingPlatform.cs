using UnityEngine;
using System.Collections; // Rất quan trọng để sử dụng Coroutine

public class DisappearingPlatform : MonoBehaviour
{
    [Header("Disappearing Settings")]
    public float disappearDelay = 2.5f; // Thời gian chờ trước khi nhấp nháy (sau khi Player chạm vào)
    public float blinkInterval = 0.1f; // Khoảng thời gian giữa mỗi lần nhấp nháy
    public int numberOfBlinks = 10; // Số lần nhấp nháy trước khi biến mất hoàn toàn

    private SpriteRenderer spriteRenderer;
    private Collider2D platformCollider;

    private bool hasPlayerTouched = false; // Biến cờ để đảm bảo chỉ kích hoạt một lần

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        platformCollider = GetComponent<Collider2D>();

        if (spriteRenderer == null)
        {
            Debug.LogError("DisappearingPlatform: SpriteRenderer not found on this GameObject!", this);
        }
        if (platformCollider == null)
        {
            Debug.LogError("DisappearingPlatform: Collider2D not found on this GameObject!", this);
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Kiểm tra xem đối tượng va chạm có phải là Player không
        // Và đảm bảo chuỗi biến mất chưa được kích hoạt
        if (collision.gameObject.CompareTag("Player") && !hasPlayerTouched)
        {
            // Kích hoạt chuỗi biến mất
            StartDisappearingSequence();
        }
    }

    public void StartDisappearingSequence()
    {
        // Đặt cờ để ngăn kích hoạt lại
        hasPlayerTouched = true;
        // Bắt đầu Coroutine để xử lý việc nhấp nháy và biến mất
        StartCoroutine(DisappearCoroutine());
    }

    // Coroutine để xử lý quá trình nhấp nháy và biến mất
    IEnumerator DisappearCoroutine()
    {
        // Đợi một khoảng thời gian trước khi bắt đầu nhấp nháy
        yield return new WaitForSeconds(disappearDelay);

        // Bắt đầu nhấp nháy
        for (int i = 0; i < numberOfBlinks; i++)
        {
            // Bật/tắt SpriteRenderer để tạo hiệu ứng nhấp nháy
            spriteRenderer.enabled = !spriteRenderer.enabled;
            yield return new WaitForSeconds(blinkInterval);
        }

        // Đảm bảo platform vô hình sau khi nhấp nháy
        spriteRenderer.enabled = false;

        // Vô hiệu hóa Collider để Player rơi qua
        if (platformCollider != null)
        {
            platformCollider.enabled = false;
        }

        // Hủy đối tượng sau một thời gian ngắn để dọn dẹp bộ nhớ
        Destroy(gameObject, 0.5f);
    }
}