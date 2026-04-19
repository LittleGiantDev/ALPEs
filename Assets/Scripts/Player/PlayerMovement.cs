using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlayerMovement : PlayerSystem
{
    [Header("Velocity")]
    [SerializeField] private float initialBaseSpeed = 12f;
    [SerializeField] private float maxBaseSpeed = 25f;
    [SerializeField] private float maxOverloadSpeed = 30f;
    [SerializeField] private float speedIncreasePerMeter = 0.002f;
    [SerializeField] private float groundAcceleration = 2f;
    [SerializeField] private float currentSpeed;
    [SerializeField] private bool isOverloaded;
    
    [Header("Landing")]
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
    [SerializeField] private float minAirTimeForBoost = 0.4f;

    [Header("GroundCheck")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.5f;
    [SerializeField] private float groundCheckDistance = 0.6f;
    
    [Header("Effects")]
    [SerializeField] private GameObject shieldVisual;
    [SerializeField] private ParticleSystem doubleJumpVFX;
    [SerializeField] private GameObject rockBreakVFXPrefab;

    private bool isGrounded;
    private bool isHoldingJump;
    private Vector2 currentGroundNormal = Vector2.up;
    private bool isDead;
    private bool isInTavern;
    private float startX;
    private float airTime;
    private float auraTimer;

    private float simulatedAngularVelocity;
    private float lastRotation;

    private bool canDoubleJump;
    private bool hasUsedDoubleJump;
    private bool hasShield;

    protected override void Awake()
    {
        base.Awake(); 
    }

    private void Start()
    {
        startX = transform.position.x;
        currentSpeed = initialBaseSpeed;

        InputManager.Instance.OnJumpInitiated += HandleJumpStart;
        InputManager.Instance.OnJumpCanceled += HandleJumpEnd;
        
        GameEvents.OnBackflipCompleted += ActivateAura;
    }

    private void OnDestroy()
    {
        InputManager.Instance.OnJumpInitiated -= HandleJumpStart;
        InputManager.Instance.OnJumpCanceled -= HandleJumpEnd;
        
        GameEvents.OnBackflipCompleted -= ActivateAura;
    }

    private void Update()
    {
        if (isDead)
        {
            return;
        }
        
        CheckGrounded();
        HandleAuraTimer();
    }

    private void FixedUpdate()
    {
        if (isDead)
        {
            return;
        }

        UpdateSpeedLogic();

        main.Rb.linearVelocity = new Vector2(currentSpeed, main.Rb.linearVelocity.y);

        if (isGrounded)
        {
            AlignWithGround();
        }
        else
        {
            if (isHoldingJump)
            {
                float newAngle = main.Rb.rotation + airRotationSpeed * Time.fixedDeltaTime;
                main.Rb.MoveRotation(newAngle);
            }
            else
            {
                PredictiveRotation();
            }
        }

        simulatedAngularVelocity = Mathf.DeltaAngle(lastRotation, main.Rb.rotation) / Time.fixedDeltaTime;
        lastRotation = main.Rb.rotation;
    }

    private void UpdateSpeedLogic()
    {
        float distance = transform.position.x - startX;
        
        float safeDistance = distance;
        if (safeDistance < 0)
        {
            safeDistance = 0;
        }

        float calculatedBase = initialBaseSpeed + (safeDistance * speedIncreasePerMeter);
        
        if (calculatedBase > maxBaseSpeed)
        {
            calculatedBase = maxBaseSpeed;
        }

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

        if (currentSpeed < initialBaseSpeed)
        {
            currentSpeed = initialBaseSpeed;
        }
        
        if (currentSpeed > maxOverloadSpeed)
        {
            currentSpeed = maxOverloadSpeed;
        }
    }

    private void EvaluateLanding()
    {
        if (isInTavern) return;

        float targetUpAngle = Vector2.SignedAngle(Vector2.up, currentGroundNormal);
        float angleDifference = Mathf.Abs(Mathf.DeltaAngle(main.Rb.rotation, targetUpAngle));

        if (angleDifference <= maxSafeLandingAngle)
        {
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayLandSound();
            }

            if (airTime >= minAirTimeForBoost)
            {
                if (GameEvents.OnPerfectLanding != null)
                {
                    GameEvents.OnPerfectLanding.Invoke();
                }
                currentSpeed += trickBoost;
                ActivateAura();
            }
            main.Rb.SetRotation(targetUpAngle);
        }
        else
        {
            Crash();
            return;
        }
        
        if (currentSpeed < initialBaseSpeed)
        {
            currentSpeed = initialBaseSpeed;
        }
        
        if (currentSpeed > maxOverloadSpeed)
        {
            currentSpeed = maxOverloadSpeed;
        }
    }

    private void CheckGrounded()
    {
        RaycastHit2D[] hits = Physics2D.CircleCastAll(groundCheck.position, groundCheckRadius, Vector2.down, groundCheckDistance);
        
        bool wasGrounded = isGrounded;
        isGrounded = false;

        for (int i = 0; i < hits.Length; i++)
        {
            if (hits[i].collider != null)
            {
                if (hits[i].collider.CompareTag("Ground"))
                {
                    isGrounded = true;
                    currentGroundNormal = hits[i].normal;
                    break;
                }
            }
        }

        if (isGrounded)
        {
            if (wasGrounded == false)
            {
                EvaluateLanding();
            }
            
            airTime = 0f;
        }
        else
        {
            airTime += Time.deltaTime;
        }

        if (main.Anim != null)
        {
            if (isGrounded == false)
            {
                main.Anim.SetBool("isJumping", true);
            }
            else
            {
                main.Anim.SetBool("isJumping", false);
            }
        }
    }

    private void AlignWithGround()
    {
        float targetAngle = Vector2.SignedAngle(Vector2.up, currentGroundNormal);
        float newAngle = Mathf.LerpAngle(main.Rb.rotation, targetAngle, Time.fixedDeltaTime * groundAlignmentSpeed);
        main.Rb.MoveRotation(newAngle);
    }

    private void PredictiveRotation()
    {
        RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, Vector2.down, 50f);
        float targetAngle = 0f;
        
        for (int i = 0; i < hits.Length; i++)
        {
            if (hits[i].collider != null)
            {
                if (hits[i].collider.CompareTag("Ground"))
                {
                    targetAngle = Vector2.SignedAngle(Vector2.up, hits[i].normal);
                    break;
                }
            }
        }

        float newAngle = Mathf.MoveTowardsAngle(main.Rb.rotation, targetAngle, predictiveRotationSpeed * Time.fixedDeltaTime);
        main.Rb.MoveRotation(newAngle);
    }

    private void HandleJumpStart()
    {
        if (isDead)
        {
            return;
        }
        
        isHoldingJump = true;
        
        if (isGrounded)
        {
            main.Rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            hasUsedDoubleJump = false;
            AudioManager.Instance.PlayJumpSound();
        }
        else if (canDoubleJump && hasUsedDoubleJump == false)
        {
            main.Rb.linearVelocity = new Vector2(main.Rb.linearVelocity.x, 0);
            main.Rb.AddForce(Vector2.up * jumpForce * 0.8f, ForceMode2D.Impulse);
            hasUsedDoubleJump = true;
            AudioManager.Instance.PlayJumpSound();
            doubleJumpVFX.Play();
        }
    }

    private void HandleJumpEnd()
    {
        if (isDead)
        {
            return;
        }
        
        isHoldingJump = false;
        
        if (main.Rb.linearVelocity.y > 0)
        {
            main.Rb.linearVelocity = new Vector2(main.Rb.linearVelocity.x, main.Rb.linearVelocity.y * 0.5f);
        }
    }

    private void HandleAuraTimer()
    {
        if (auraTimer > 0)
        {
            auraTimer -= Time.deltaTime;
            
            if (auraVFX != null)
            {
                if (auraVFX.isPlaying == false)
                {
                    isOverloaded = true;
                    auraVFX.Play();
                    
                    if (AudioManager.Instance != null)
                    {
                        AudioManager.Instance.PlayAuraSound();
                    }
                }
            }
        }
        else if (auraVFX != null)
        {
            if (auraVFX.isPlaying)
            {
                isOverloaded = false;
                auraVFX.Stop();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isDead)
        {
            return;
        }

        if (collision.CompareTag("Hazard"))
        {
            if (isOverloaded)
            {
                Destroy(collision.gameObject);
                Instantiate(rockBreakVFXPrefab, collision.transform.position, Quaternion.identity);
            }
            else
            {
                Crash();
            }
        }
    }

    public void VoidCrash()
    {
        if (isDead)
        {
            return;
        }
        
        hasShield = false;
        Crash();
    }

    public void Crash()
    {
        if (isDead || isInTavern)
        {
            return;
        }
        
        if (hasShield)
        {
            hasShield = false;
            if (shieldVisual != null)
            {
                shieldVisual.SetActive(false);
            }
            
            main.Rb.linearVelocity = new Vector2(main.Rb.linearVelocity.x, jumpForce);
            return;
        }

        isDead = true;
        auraTimer = 0;
        
        if (auraVFX != null)
        {
            auraVFX.Stop();
        }

        main.TriggerCrash();
        main.TriggerDeath();

        this.enabled = false;
    }
    
    public void SetTavernState(bool state)
    {
        isInTavern = state;
        
        if (state)
        {
            isGrounded = true;
            airTime = 0f;
        }

        if (main.Anim != null)
        {
            main.Anim.SetBool("isInTavern", state);
            if (state)
            {
                main.Anim.SetBool("isJumping", false);
            }
        }
    }

    public void EnableDoubleJump()
    {
        canDoubleJump = true;
    }

    public void UpgradeBaseSpeed(float amount)
    {
        initialBaseSpeed += amount;
        maxBaseSpeed += amount;
        maxOverloadSpeed += amount;
    }
    
    public void UpgradeRotationSpeed(float amount)
    {
        airRotationSpeed += amount;
        predictiveRotationSpeed += amount;
    }

    public void UpgradeTrickBoost(float amount)
    {
        trickBoost += amount;
    }

    public void UpgradeAcceleration(float amount)
    {
        groundAcceleration += amount;
    }

    public void EnableShield()
    {
        hasShield = true;
        if (shieldVisual != null)
        {
            shieldVisual.SetActive(true);
        }
    }

    public bool GetIsGrounded()
    {
        return isGrounded;
    }

    public float GetCurrentSpeed()
    {
        return currentSpeed;
    }

    public float GetAngularVelocity()
    {
        return simulatedAngularVelocity;
    }

    public bool GetIsDead()
    {
        return isDead;
    }
    
    private void ActivateAura()
    {
        auraTimer = auraDuration;
    }
    
    public bool HasDoubleJump()
    {
        return canDoubleJump;
    }

    public bool HasShield()
    {
        return hasShield;
    }
}