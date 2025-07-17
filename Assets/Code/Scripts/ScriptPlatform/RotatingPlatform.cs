using UnityEngine;
using System.Collections;

public class RotatingPlatform : MonoBehaviour
{
    public float rotationSpeed = 90f;      // Độ mỗi giây
    public float waitDuration = 1.5f;      // Thời gian dừng giữa các lần xoay

    private float currentRotation = 0f;    // << BỔ SUNG DÒNG NÀY
    private bool isRotating = true;
    private bool rotatedUp = true;

    private PlatformEffector2D effector;

    void Start()
    {
        effector = GetComponent<PlatformEffector2D>();
        transform.rotation = Quaternion.Euler(0f, 0f, rotatedUp ? 0f : 180f);

        if (effector == null)
        {
            Debug.LogWarning("PlatformEffector2D not found on rotating platform.");
        }
    }

    void Update()
    {
        if (!isRotating) return;

        float delta = rotationSpeed * Time.deltaTime;
        currentRotation += delta;

        // Gán rotation Z trực tiếp, thay vì tích lũy trôi
        float newZ = rotatedUp ? currentRotation : 180f + currentRotation;
        transform.rotation = Quaternion.Euler(0f, 0f, newZ);

        if (currentRotation >= 180f)
        {
            currentRotation = 0f;
            rotatedUp = !rotatedUp;

            // Đổi hướng PlatformEffector
            if (effector != null)
                effector.rotationalOffset = rotatedUp ? 0f : 180f;

            // Dừng xoay tạm thời
            isRotating = false;
            StartCoroutine(WaitThenContinueRotation());
        }
    }

    IEnumerator WaitThenContinueRotation()
    {
        yield return new WaitForSeconds(waitDuration);
        isRotating = true;
    }

    void ResumeRotation()
    {
        isRotating = true;
    }
}