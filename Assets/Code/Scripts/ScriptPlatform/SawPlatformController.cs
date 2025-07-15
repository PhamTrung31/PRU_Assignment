using UnityEngine;

public class SawPlatformController : MonoBehaviour
{
    public SawBladeController sawBlade; // Kéo GameObject SawBlade con vào đây trong Inspector

    private bool hasPlayerLanded = false; // Cờ để đảm bảo chỉ dừng một lần

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Kiểm tra xem đối tượng va chạm có phải là Player không
        if (collision.gameObject.CompareTag("Player") && !hasPlayerLanded)
        {
            if (sawBlade != null)
            {
                sawBlade.StopSaw(); // Gọi hàm dừng lưỡi cưa
                hasPlayerLanded = true;
            }
            else
            {
                Debug.LogWarning("SawBladeController not assigned to SawPlatformController on " + gameObject.name);
            }
        }
    }

    // Tùy chọn: Nếu bạn muốn lưỡi cưa di chuyển lại khi player rời khỏi platform
    // void OnCollisionExit2D(Collision2D collision)
    // {
    //     if (collision.gameObject.CompareTag("Player") && hasPlayerLanded)
    //     {
    //         if (sawBlade != null)
    //         {
    //             sawBlade.StartSaw();
    //             hasPlayerLanded = false; // Reset cờ
    //         }
    //     }
    // }
}