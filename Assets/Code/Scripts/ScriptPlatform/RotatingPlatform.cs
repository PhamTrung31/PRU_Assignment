using UnityEngine;
using System.Collections;

public class RotatingPlatform : MonoBehaviour
{
    public float rotationSpeed = 720f; // Tốc độ xoay (độ/giây) - Đặt cao hơn để xoay 360 độ nhanh hơn
    public float rotationInterval = 1.5f; // Thời gian chờ sau khi xoay xong một vòng
    public float rotationDuration = 0.5f; // Thời gian xoay một vòng (360 độ)

    private bool isRotating = false;
    private bool hasPlayerOn = false; // Biến cờ kiểm tra Player đang đứng trên platform

    void Start()
    {
        // Bắt đầu chu kỳ xoay
        StartCoroutine(RotationCycle());
    }

    IEnumerator RotationCycle()
    {
        while (true) // Lặp lại vô hạn
        {
            // Chờ trước khi bắt đầu xoay (rotationInterval)
            yield return new WaitForSeconds(rotationInterval);

            // Kiểm tra nếu người chơi đang ở trên platform, thì chờ
            while (hasPlayerOn)
            {
                yield return null; // Chờ đến frame tiếp theo
            }

            // Bắt đầu thực hiện xoay nếu không có người chơi
            yield return StartCoroutine(PerformRotation());
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            hasPlayerOn = true; // Đặt cờ khi Player chạm vào
            collision.transform.SetParent(this.transform); // Đặt platform làm cha của Player

            // Nếu platform đang xoay khi Player nhảy lên, dừng ngay lập tức
            if (isRotating)
            {
                StopCoroutine("PerformRotation");
                isRotating = false;
                // Có thể thêm logic để đặt platform về góc ổn định ở đây nếu cần,
                // nhưng thường thì dừng giữa chừng sẽ là đủ.
            }
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            hasPlayerOn = false; // Bỏ cờ khi Player rời đi
            collision.transform.SetParent(null); // Hủy bỏ quan hệ cha-con
        }
    }

    IEnumerator PerformRotation()
    {
        isRotating = true;
        float timer = 0f;

        // Tính toán tốc độ quay thực tế cần thiết để hoàn thành 360 độ trong rotationDuration
        float actualRotationSpeed = 360f / rotationDuration; // Luôn xoay 360 độ

        while (timer < rotationDuration)
        {
            // Nếu Player nhảy lên trong lúc đang xoay, thoát khỏi Coroutine xoay
            if (hasPlayerOn)
            {
                isRotating = false;
                yield break; // Kết thúc Coroutine này ngay lập tức
            }

            // Xoay platform một lượng nhỏ mỗi frame
            transform.Rotate(Vector3.forward * actualRotationSpeed * Time.deltaTime);
            timer += Time.deltaTime;
            yield return null; // Chờ frame tiếp theo
        }

        // Đảm bảo cờ isRotating được đặt lại sau khi hoàn thành chu kỳ xoay
        isRotating = false;
    }
}