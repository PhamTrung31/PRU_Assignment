using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Windows;
using System.Collections;
using System;
public class PLayer : MonoBehaviour
{
    private Vector2 movementInput;
    private Rigidbody2D _rb;

    public LayerMask _groundLayer;
    private BoxCollider2D _bxCollider2D;

    private HeightScoring _heightScoring;
    [SerializeField] private float _dashSpeed = 10f;
    [SerializeField] private float _dashTime = 0.2f;
    [SerializeField] private float _jumpForce = 5f;
    [SerializeField] private float _BoostForce = 100f;
    public int health = 2;
    private bool _isDashing = false;
    private bool _canmove = false;
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


    private void OnMove(InputValue value)
    {
        if (!isGrounded())
        {
            return;  // Prevent movement if not grounded
        }
        Vector2 input = value.Get<Vector2>();

        if (_isDashing || !_canmove)
            return;

        if (input.x < 0)
        {
            StartCoroutine(Dash(-1));
        }
        else if (input.x > 0)
        {
            StartCoroutine(Dash(1));
        }
    }

    private System.Collections.IEnumerator Dash(int direction)
    {
        _isDashing = true;

        // Set velocity instantly
        _rb.linearVelocity = new Vector2(direction * _dashSpeed, 0);

        // Wait for the dash time
        yield return new WaitForSeconds(_dashTime);

        // Stop velocity
        _rb.linearVelocity = Vector2.zero;

        _isDashing = false;
    }

    private void OnJump(InputValue value)
    {
        if (value.isPressed)
        {
            if (isGrounded() && !_isDashing)
            {
                Jump();
            }
        }
    }

    private void Jump()
    {
        _rb.linearVelocity = new Vector3(_rb.linearVelocity.x, _jumpForce);
        _canmove = true;
    }

    private bool isGrounded()
    {
        var bounds = _bxCollider2D.bounds;
        float rayLength = 0.1f;

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
                Debug.Log("Player is dead");
                _heightScoring.SaveScore();
                // Handle death here, e.g., reload scene
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
    }

    private void Boost()
    {
        _rb.linearVelocity = new Vector3(_rb.linearVelocity.x, _BoostForce);
        _canmove = true;
    }


}
