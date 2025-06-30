using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Windows;
using System.Collections;
using System;
using Unity.VisualScripting;
public class PLayer : MonoBehaviour
{
    private Vector2 movementInput;
    private Rigidbody2D _rb;

    public LayerMask _groundLayer;
    private BoxCollider2D _bxCollider2D;

    private HeightScoring _heightScoring;
    [SerializeField] private float _Speed = 2f;
    //[SerializeField] private float _dashTime = 0.2f;
    [SerializeField] private float _jumpForce = 5f;
    [SerializeField] private float _BoostForce = 100f;
    public int health = 1;
    public int maxhealth = 2;
    //private bool _isDashing = false;
    private bool _canmove = false;
    private int jumpCount = 0;
    private const int maxJumpCount = 2; // 1 jump + 1 boost
    private bool wasGrounded = true;
    private bool canJumpBoost = true;
    private bool shieldOn = false;
    private Coroutine _shieldCoroutine;
    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _bxCollider2D = GetComponent<BoxCollider2D>();

    }
    public void Start()
    {
        _heightScoring = HeightScoring.Instance; // Assuming HeightScoring is a singleton
    }

    private void Update()
    {

    }

    private void FixedUpdate()
    {
        bool grounded = isGrounded();

        if (!wasGrounded && grounded)
        {
            jumpCount = 0; // Reset jump/boost on landing
        }

        wasGrounded = grounded;

        if (!grounded)
        {
            _rb.linearVelocity = new Vector2(0f, _rb.linearVelocity.y); // Optional: disable air movement
        }

        Move();
    }

    private void OnMove(InputValue value)
    {
        if (!isGrounded())
        {
            movementInput = Vector2.zero; // Clear input in air
            return;
        }

        movementInput = value.Get<Vector2>();

        //if (_isDashing || !_canmove)
        //    return;




    }
    private void Move()
    {
        if (!isGrounded())
        {
            return;  // Prevent movement if not grounded

        }


        Debug.Log("Moving");
        if (!_canmove)
        {
            return;

        }
        _rb.linearVelocity = new Vector2(movementInput.x * _Speed, _rb.linearVelocity.y);
    }


    //private System.Collections.IEnumerator Dash(int direction)
    //{
    //    _isDashing = true;

    //    // Set velocity instantly
    //    _rb.linearVelocity = new Vector2(direction * _dashSpeed, 0);

    //    // Wait for the dash time
    //    yield return new WaitForSeconds(_dashTime);

    //    // Stop velocity
    //    _rb.linearVelocity = Vector2.zero;

    //    _isDashing = false;
    //}

    private void OnJump(InputValue value)
    {
        if (!value.isPressed)
            return;

        if (isGrounded())
        {
            Jump();         // First jump
            jumpCount = 1;  // We've used 1 jump
        }
        else if (jumpCount == 1)
        {
            if (canJumpBoost)
            {
                Boost();        // Second "jump" becomes boost
                jumpCount = 2;  // Used both jump and boost
                canJumpBoost = false; // Disable further boosts until grounded
            }

        }
    }

    private void Jump()
    {
        _rb.linearVelocity = new Vector2(0f, _jumpForce); // Remove x velocity during jump
        _canmove = true;
    }

    private bool isGrounded()
    {
        var bounds = _bxCollider2D.bounds;
        float rayLength = 0.05f;

        Vector2 left = new(bounds.min.x + 0.01f, bounds.min.y);
        Vector2 right = new(bounds.max.x - 0.01f, bounds.min.y);

        return Physics2D.Raycast(left, Vector2.down, rayLength, _groundLayer) ||
               Physics2D.Raycast(right, Vector2.down, rayLength, _groundLayer);

    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Lava"))
        {
            health--;

            if (health <= 0)
            {
                Dead();
            }
            else
            {
                Debug.Log("Hit Lava but still alive, boosting...");
                Boost();
            }
        }
        else if (other.CompareTag("BoostItem"))
        {
            Debug.Log("Picked up boost item!");
            Boost();
            Destroy(other.gameObject); // Remove the boost item after collection
        }
        else if (other.CompareTag("Health"))
        {
            if (health < maxhealth)
            {
                health += 1;
            }
            Destroy(other.gameObject); // Remove the health item after collection
        }
        else if (other.CompareTag("Shield"))
        {
            StartShieldCoroutine();
            Destroy(other.gameObject); // Remove the shield item after collection
        }
        else if (other.CompareTag("Trap"))
        {
            if (shieldOn)
            {
                return;
            }
            health--;
            Dead();

        }
    }

    private void Boost()
    {
        _rb.linearVelocity = new Vector3(_rb.linearVelocity.x, _BoostForce);
        _canmove = true;
    }


    private IEnumerator TriggerShield()
    {
        shieldOn = true;
        yield return new WaitForSeconds(3f); // Shield lasts for 3 seconds, Evinsible after that
        shieldOn = false;
        _shieldCoroutine = null;
    }

    private void StartShieldCoroutine()
    {
        if (_shieldCoroutine != null)
        {
            StopCoroutine(TriggerShield());
        }
        _shieldCoroutine = StartCoroutine(TriggerShield());
    }

    public void Dead()
    {
        if (health == 0)
        {
            Debug.Log("Player is dead");
            _heightScoring.SaveScore();
        }
    }
}
