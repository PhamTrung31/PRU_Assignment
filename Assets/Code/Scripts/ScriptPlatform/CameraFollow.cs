using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    
    public Transform playerTransform;

    
    public float smoothSpeed = 0.125f;

    
    public Vector3 offset;

    
    void LateUpdate()
    {
        
        if (playerTransform != null)
        {
            Vector3 desiredPosition = new Vector3(transform.position.x, playerTransform.position.y + offset.y, offset.z);          
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);           
            transform.position = smoothedPosition;
        }
    }
}