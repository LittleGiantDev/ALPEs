using System;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float initialBaseSpeed = 12f;
    [SerializeField] private float maxBaseSpeed = 25f;
    [SerializeField] private float maxOverloadSpeed = 30f;
    [SerializeField] private float speedIncreasePerMeter = 0.002f;
    [SerializeField] private float groundAcceleration = 2f;
    
    [SerializeField] private float overloadDecayRate = 1.5f;
    [SerializeField] private float trickBoost = 7f;
    [SerializeField] private float auraDuration = 5f;
    [SerializeField] private ParticleSystem auraVFX;

    [SerializeField] private float maxSafeLandingAngle = 20f;
    [SerializeField] private float jumpForce = 14f;
    [SerializeField] private float airRotationSpeed = 300f;
    [SerializeField] private float predictiveRotationSpeed = 40f; 
    [SerializeField] private float groundAlignmentSpeed = 15f;
    [SerializeField] private float safeAlignmentThreshold = 45f;

    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.5f;
    [SerializeField] private float groundCheckDistance = 0.6f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float minAirTimeForBoost = 0.4f;

    private Rigidbody2D rb;
    private bool isGrounded;
    private bool isHoldingJump;
    private Vector2 currentGroundNormal = Vector2.up;
    private bool isDead;
    private float startX;
    private float airTime;
    private float auraTimer;
    [SerializeField] private float currentSpeed;
    [SerializeField] private bool isOverloaded;

    private float simulatedAngularVelocity;
    private float lastRotation;
    
    private ContactFilter2D groundFilter;
    private RaycastHit2D[] raycastHits = new RaycastHit2D[1];

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        startX = transform.position.x;
        currentSpeed = initialBaseSpeed;

        InputManager.Instance.OnJumpInitiated += HandleJumpStart;
        InputManager.Instance.OnJumpCanceled += HandleJumpEnd;
        
        GameEvents.OnBackflipCompleted += ActivateAura;

        groundFilter = new ContactFilter2D();
        groundFilter.SetLayerMask(groundLayer);
        groundFilter.useTriggers = false; 
    }

    private void OnDestroy()
    {
        if (InputManager.Instance != null)
        {
            InputManager.Instance.OnJumpInitiated -= HandleJumpStart;
            InputManager.Instance.OnJumpCanceled -= HandleJumpEnd;
        }
        
        GameEvents.OnBackflipCompleted -= ActivateAura;
    }

    private void Update()
    {
        if (isDead) return;
        CheckGrounded();
        HandleAuraTimer();
    }

    private void FixedUpdate()
    {
        if (isDead) return;

        UpdateSpeedLogic();

        rb.linearVelocity = new Vector2(currentSpeed, rb.linearVelocity.y);

        if (isGrounded)
        {
            AlignWithGround();
        }
        else
        {
            if (isHoldingJump)
            {
                float newAngle = rb.rotation + airRotationSpeed * Time.fixedDeltaTime;
                rb.MoveRotation(newAngle);
            }
            else
            {
                PredictiveRotation();
            }
        }

        simulatedAngularVelocity = Mathf.DeltaAngle(lastRotation, rb.rotation) / Time.fixedDeltaTime;
        lastRotation = rb.rotation;
    }

    private void UpdateSpeedLogic()
    {
        float distance = transform.position.x - startX;
        float calculatedBase = initialBaseSpeed + (Mathf.Max(0, distance) * speedIncreasePerMeter);
        calculatedBase = Mathf.Min(calculatedBase, maxBaseSpeed);

        if (isGrounded)
        {
            currentSpeed += groundAcceleration * Time.fixedDeltaTime;
        }

        if (currentSpeed > calculatedBase)
        {
            currentSpeed = Mathf.MoveTowards(currentSpeed, calculatedBase, overloadDecayRate * Time.fixedDeltaTime);
        }
        else if (currentSpeed < calculatedBase)
        {
            currentSpeed = Mathf.MoveTowards(currentSpeed, calculatedBase, groundAcceleration * 2f * Time.fixedDeltaTime);
        }

        currentSpeed = Mathf.Clamp(currentSpeed, initialBaseSpeed, maxOverloadSpeed);
    }

    private void EvaluateLanding()
    {
        float targetUpAngle = Vector2.SignedAngle(Vector2.up, currentGroundNormal);
        float angleDifference = Mathf.Abs(Mathf.DeltaAngle(rb.rotation, targetUpAngle));

        if (angleDifference <= maxSafeLandingAngle)
        {
            if (airTime >= minAirTimeForBoost)
            {
                GameEvents.OnPerfectLanding?.Invoke();
                currentSpeed += trickBoost;
                ActivateAura();
            }
            rb.SetRotation(targetUpAngle);
        }
        else
        {
            Crash();
            return;
        }
        
        currentSpeed = Mathf.Clamp(currentSpeed, initialBaseSpeed, maxOverloadSpeed);
    }

    private void CheckGrounded()
    {
        int count = Physics2D.CircleCast(groundCheck.position, groundCheckRadius, Vector2.down, groundFilter, raycastHits, groundCheckDistance);
        bool wasGrounded = isGrounded;
        isGrounded = count > 0;

        if (isGrounded)
        {
            currentGroundNormal = raycastHits[0].normal;
            if (!wasGrounded) EvaluateLanding();
            airTime = 0f;
        }
        else
        {
            airTime += Time.deltaTime;
        }
    }

    private void AlignWithGround()
    {
        float targetAngle = Vector2.SignedAngle(Vector2.up, currentGroundNormal);
        float newAngle = Mathf.LerpAngle(rb.rotation, targetAngle, Time.fixedDeltaTime * groundAlignmentSpeed);
        rb.MoveRotation(newAngle);
    }

    private void PredictiveRotation()
    {
        int count = Physics2D.Raycast(transform.position, Vector2.down, groundFilter, raycastHits, 50f);
        float targetAngle = 0f;
        
        if (count > 0) targetAngle = Vector2.SignedAngle(Vector2.up, raycastHits[0].normal);

        float newAngle = Mathf.MoveTowardsAngle(rb.rotation, targetAngle, predictiveRotationSpeed * Time.fixedDeltaTime);
        rb.MoveRotation(newAngle);
    }

    private void HandleJumpStart()
    {
        if (isDead) return;
        isHoldingJump = true;
        if (isGrounded)
        {
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }
    }

    private void HandleJumpEnd()
    {
        if (isDead) return;
        isHoldingJump = false;
        if (rb.linearVelocity.y > 0) rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * 0.5f);
    }

    private void HandleAuraTimer()
    {
        if (auraTimer > 0)
        {
            auraTimer -= Time.deltaTime;
            if (auraVFX != null && !auraVFX.isPlaying)
            {
                isOverloaded = true;
                auraVFX.Play();
            }
        }
        else if (auraVFX != null && auraVFX.isPlaying)
        {
            isOverloaded = false;
            auraVFX.Stop();
        }
    }

    private void ActivateAura() => auraTimer = auraDuration;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isDead) return;

        if (collision.CompareTag("Hazard"))
        {
            if (isOverloaded) Destroy(collision.gameObject);
            else Crash();
        }
    }

    public void Crash()
    {
        if (isDead) return;
        isDead = true;
        rb.bodyType = RigidbodyType2D.Static;
        auraTimer = 0;
        GameEvents.OnPlayerCrash?.Invoke();
    }

    public bool GetIsGrounded() => isGrounded;
    public float GetCurrentSpeed() => currentSpeed;
    public float GetAngularVelocity() => simulatedAngularVelocity;

    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = isGrounded ? Color.green : Color.red;
            Gizmos.DrawWireSphere(groundCheck.position + (Vector3)Vector2.down * groundCheckDistance, groundCheckRadius);
        }
    }
}