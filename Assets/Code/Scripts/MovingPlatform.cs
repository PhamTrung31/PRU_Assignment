using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    public float baseMoveSpeed = 1f; 
    public float baseMoveRange = 2f; 

    private Vector3 startPos;
    private float currentMoveSpeed;
    private float currentMoveRange;

    void Awake() 
    {
        startPos = transform.position;
        currentMoveSpeed = baseMoveSpeed; 
        currentMoveRange = baseMoveRange; 
    }

    public void SetDifficulty(float speedMultiplier, float rangeMultiplier)
    {
        currentMoveSpeed = baseMoveSpeed * speedMultiplier;
        currentMoveRange = baseMoveRange * rangeMultiplier;
    }

    void Update()
    {
        
        float newX = startPos.x + Mathf.Sin(Time.time * currentMoveSpeed) * currentMoveRange;
        transform.position = new Vector3(newX, transform.position.y, transform.position.z);
    }
}