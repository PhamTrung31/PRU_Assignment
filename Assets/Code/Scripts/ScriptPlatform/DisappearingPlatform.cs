using UnityEngine;
using System.Collections; // Rất quan trọng để sử dụng Coroutine

public class DisappearingPlatform : MonoBehaviour
{
    [Header("Disappearing Settings")]
    public float standTimeBeforeBlink = 1.3f;
    public float blinkDuration = 1.5f;
    public float blinkInterval = 0.1f;

    private SpriteRenderer spriteRenderer;
    private Collider2D platformCollider;
    private bool isDisappearing = false;
    private Coroutine disappearRoutine;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        platformCollider = GetComponent<Collider2D>();

        if (!spriteRenderer) Debug.LogError("Missing SpriteRenderer");
        if (!platformCollider) Debug.LogError("Missing Collider2D");
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        if (isDisappearing) return;

        if (collision.gameObject.CompareTag("Player"))
        {
            foreach (ContactPoint2D contact in collision.contacts)
            {
                // Kiểm tra tiếp xúc từ trên (normal hướng lên)
                if (contact.normal.y > 0.7f)
                {
                    // Player đang đứng trên
                    if (disappearRoutine == null)
                    {
                        disappearRoutine = StartCoroutine(DisappearCycle());
                    }
                    break;
                }
            }
        }
    }

    IEnumerator DisappearCycle()
    {
        isDisappearing = true;

        // ⏱ Chờ player đứng trên trong 1.3s
        yield return new WaitForSeconds(standTimeBeforeBlink);

        // 💡 Nhấp nháy trong 1.5s
        float elapsed = 0f;
        while (elapsed < blinkDuration)
        {
            spriteRenderer.enabled = !spriteRenderer.enabled;
            elapsed += blinkInterval;
            yield return new WaitForSeconds(blinkInterval);
        }

        // Ẩn hoàn toàn
        spriteRenderer.enabled = false;
        platformCollider.enabled = false;

        // ⏳ Hồi sinh sau vài giây
        yield return new WaitForSeconds(2f);

        spriteRenderer.enabled = true;
        platformCollider.enabled = true;

        // Reset cho lần tiếp theo
        disappearRoutine = null;
        isDisappearing = false;
    }
}