using UnityEngine;

public class PlayerController : MonoBehaviour
{
    
    public float jumpForce = 10f;

    
    private Rigidbody2D rb;

    
    private bool isGrounded;

    
    void Start()
    {
        
        rb = GetComponent<Rigidbody2D>();

        
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            Debug.LogWarning("Player does not have 'Player' tag. Please assign it in Inspector.");
            
        }
    }

   
    void Update()
    {

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            
            isGrounded = false;
        }
    }

   
    void OnCollisionEnter2D(Collision2D collision)
    {       
        if (collision.gameObject.CompareTag("Platform"))
        {
            isGrounded = true;
        }
    }

    
    void OnCollisionExit2D(Collision2D collision)
    {       
        if (collision.gameObject.CompareTag("Platform"))
        {           
            isGrounded = false;
        }
    }
}