using UnityEngine;
using UnityEngine.SceneManagement; 

public class DeadGroundController : MonoBehaviour
{
    public Transform playerTransform; 
    public float followOffset = 5f;   
    public float moveSpeed = 0.5f;    

    [Header("Difficulty Scaling")]
    public float speedIncreaseRate = 0.01f; 
    private float currentMoveSpeed;

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
        currentMoveSpeed = moveSpeed; 
    }

    void Update()
    {
        
        currentMoveSpeed += speedIncreaseRate * Time.deltaTime;

        
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