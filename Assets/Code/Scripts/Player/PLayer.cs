using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Windows;
using System.Collections;
using System;
using Unity.VisualScripting;
using Unity.Cinemachine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
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

    [Header("Audio")]
    public AudioClip boostSFX;
    public AudioClip coinSFX, itemSFX, itemUseSFX, deathSFX;

    [Header("VFX")]
    public ParticleSystem boostExplodeFX;
    public ParticleSystem dustFX, bloodFX;
    private Light2D _playerLight;

    [Header("Post-processing")]
    [SerializeField] private VolumeProfile volume;
    private ChromaticAberration chromaticEffect;
    public CameraShake cameraShake;
    private CinemachineImpulseSource _impulseSource;
    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _bxCollider2D = GetComponent<BoxCollider2D>();
        _impulseSource = GetComponent<CinemachineImpulseSource>();
        _playerLight = GetComponent<Light2D>();

    }
    public void Start()
    {
        _heightScoring = HeightScoring.Instance; // Assuming HeightScoring is a singleton
        if (volume != null && volume.TryGet(out chromaticEffect))
        {
            chromaticEffect.intensity.overrideState = true;
            chromaticEffect.intensity.value = 0f;
        }
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
            if (dustFX != null && _rb.linearVelocityY <= 0) Instantiate(dustFX, transform.position, Quaternion.identity);

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
            PlaySFXClip(itemSFX);
            StartShieldCoroutine();
            Destroy(other.gameObject); // Remove the shield item after collection
        }
        else if (other.CompareTag("Trap"))
        {
            if (shieldOn)
            {
                if (bloodFX != null) Instantiate(bloodFX, transform.position, Quaternion.identity);
                PlaySFXClip(itemUseSFX);
                return;
            }
            health--;
            Dead();

        }
    }

    private void Boost()
    {
        _rb.linearVelocity = new Vector3(_rb.linearVelocity.x, _BoostForce);
        if (cameraShake != null) CameraShake.instance.Shake(_impulseSource);
        PlaySFXClip(boostSFX);
        Debug.Log($"Chromatic status: {chromaticEffect != null}");
        if (chromaticEffect != null)
        {
            StartCoroutine(ChromaticFlash());
        }
        if (boostExplodeFX != null)
        {
            Instantiate(boostExplodeFX, transform.position, Quaternion.identity);
        }

        _canmove = true;


    }

    private IEnumerator ChromaticFlash()
    {
        float duration = 0.5f;
        float holdTime = 1f;

        // Lerp từ 0 → 1 trong 0.5s
        float time = 0f;
        while (time < duration)
        {
            time += Time.deltaTime;
            chromaticEffect.intensity.value = Mathf.Lerp(0f, 1f, time / duration);
            _playerLight.intensity = Mathf.Lerp(0f, 1f, time / duration);
            yield return null;
        }

        // Giữ ở 1 trong 1s
        chromaticEffect.intensity.value = 1f;
        yield return new WaitForSeconds(holdTime);

        // Lerp từ 1 → 0 trong 0.5s
        time = 0f;
        while (time < duration)
        {
            time += Time.deltaTime;
            chromaticEffect.intensity.value = Mathf.Lerp(1f, 0f, time / duration);
            _playerLight.intensity = Mathf.Lerp(1f, 0f, time / duration);
            yield return null;
        }

        chromaticEffect.intensity.value = 0f;
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
            PlaySFXClip(deathSFX);
            Debug.Log("Player is dead");
            _heightScoring.SaveScore();
        }
    }
    private void PlaySFXClip(AudioClip soundClip)
    {
        if (soundClip == null || SFXManager.instance == null) return;
        SFXManager.instance.PlaySFXClip(soundClip, transform, 1f);
    }

}
