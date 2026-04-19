using UnityEngine;

public class ShootingController : PlayerSystem
{
    [Header("Stats")]
    [SerializeField] private float weaponRange = 50f;
    [SerializeField] private int damage = 100;
    [SerializeField] private LayerMask hitLayers;

    [Header("Ammo")]
    [SerializeField] private int maxAmmo = 10;
    [SerializeField] private float fireRate = 0.2f;
    [SerializeField] private float reloadTime = 1.5f;
    
    [Header("Explosion")]
    [SerializeField] private float explosionRadius = 3.5f;
    [SerializeField] private GameObject explosionVFXPrefab;

    [Header("References")]
    [SerializeField] private Transform firePoint;
    [SerializeField] private LineRenderer laserLine;
    [SerializeField] private GameObject weaponPivot;
    [SerializeField] private GameObject animatedFrontArm;

    private int currentAmmo;
    private float nextFireTime;
    private bool isReloading;
    private bool isAiming;
    private bool hasExplosiveBullets;
    private float reloadTimer;

    protected override void Awake()
    {
        base.Awake();
    }

    private void Start()
    {
        currentAmmo = maxAmmo;
        main.TriggerAmmoChanged(currentAmmo, maxAmmo);

        weaponPivot.SetActive(false);
        animatedFrontArm.SetActive(true);

        InputManager.Instance.OnLeftClickInitiated += TryShoot;
        InputManager.Instance.OnRightClickInitiated += HandleAimStart;
        InputManager.Instance.OnRightClickCanceled += HandleAimEnd;
        InputManager.Instance.OnReloadInitiated += HandleReloadInput;
    }

    private void Update()
    {
        if (isAiming)
        {
            UpdateLaser();
        }

        if (isReloading)
        {
            reloadTimer += Time.deltaTime;
            if (reloadTimer >= reloadTime)
            {
                currentAmmo = maxAmmo;
                isReloading = false;
                
                main.TriggerAmmoChanged(currentAmmo, maxAmmo);
                GameEvents.OnReloadFinished.Invoke();
            }
        }
    }

    private void UpdateLaser()
    {
        laserLine.SetPosition(0, firePoint.position);
        RaycastHit2D hit = Physics2D.Raycast(firePoint.position, firePoint.right, weaponRange, hitLayers);
        
        if (hit.collider != null)
        {
            laserLine.SetPosition(1, hit.point);
        }
        else
        {
            laserLine.SetPosition(1, (Vector2)firePoint.position + (Vector2)firePoint.right * weaponRange);
        }
    }

    private void HandleReloadInput()
    {
        if (isReloading == false && currentAmmo < maxAmmo)
        {
            StartReload();
        }
    }

    private void TryShoot()
    {
        if (isAiming == false || isReloading || Time.time < nextFireTime)
        {
            return;
        }
        
        Shoot();
    }

    private void Shoot()
    {
        currentAmmo--;
        nextFireTime = Time.time + fireRate;
        
        main.TriggerAmmoChanged(currentAmmo, maxAmmo);
        main.TriggerShoot();
        
        RaycastHit2D hit = Physics2D.Raycast(firePoint.position, firePoint.right, weaponRange, hitLayers);
        
        if (hit.collider != null)
        {
            HandleHit(hit);
        }

        if (currentAmmo <= 0)
        {
            StartReload();
        }
    }

    private void StartReload()
    {
        isReloading = true;
        reloadTimer = 0f;
        GameEvents.OnReloadStarted.Invoke(reloadTime);
    }

    private void HandleHit(RaycastHit2D hit)
    {
        if (hit.collider.CompareTag("Ground"))
        {
            ImpactParticles.SpawnAtPoint(hit.point, hit.normal);
        }

        if (hasExplosiveBullets)
        {
            ExecuteExplosion(hit.point);
        }
        else
        {
            ApplyDirectDamage(hit);
        }
    }
    
    private void ExecuteExplosion(Vector2 point)
    {
        GameObject explosionParticles = Instantiate(explosionVFXPrefab, point, Quaternion.identity);

        Collider2D[] colliders = Physics2D.OverlapCircleAll(point, explosionRadius);
        for (int i = 0; i < colliders.Length; i++)
        {
            IDamageable Idamage = colliders[i].GetComponentInParent<IDamageable>();
            if (Idamage != null)
            {
                Rigidbody2D rigidbodyP = colliders[i].GetComponent<Rigidbody2D>();
                Idamage.TakeDamage(damage, point, firePoint.right, rigidbodyP);
            }
        }
    }

    private void ApplyDirectDamage(RaycastHit2D hit)
    {
        IDamageable damageable = hit.collider.GetComponentInParent<IDamageable>();
        if (damageable != null)
        {
            Rigidbody2D rigidbodyPart = hit.collider.GetComponent<Rigidbody2D>();
            damageable.TakeDamage(damage, hit.point, firePoint.right, rigidbodyPart);
        }
    }

    private void OnDestroy()
    {
        InputManager.Instance.OnLeftClickInitiated -= TryShoot;
        InputManager.Instance.OnRightClickInitiated -= HandleAimStart;
        InputManager.Instance.OnRightClickCanceled -= HandleAimEnd;
        InputManager.Instance.OnReloadInitiated -= HandleReloadInput;
    }

    private void HandleAimStart()
    {
        isAiming = true;
        laserLine.enabled = true;
        
        AudioManager.Instance.PlayAimSound();
        
        weaponPivot.SetActive(true);
        animatedFrontArm.SetActive(false);
    }

    private void HandleAimEnd()
    {
        isAiming = false;
        laserLine.enabled = false;
        weaponPivot.SetActive(false);
        
        animatedFrontArm.SetActive(true);
    }

    public void UpgradeMaxAmmo(int extraAmmo)
    {
        maxAmmo += extraAmmo;
        currentAmmo = maxAmmo;
        main.TriggerAmmoChanged(currentAmmo, maxAmmo);
    }

    public void UpgradeFireRate(float reduction)
    {
        fireRate -= reduction;
        if (fireRate < 0.05f)
        {
            fireRate = 0.05f;
        }
    }

    public void UpgradeReloadSpeed(float reduction)
    {
        reloadTime -= reduction;
        if (reloadTime < 0.2f)
        {
            reloadTime = 0.2f;
        }
    }

    public void EnableExplosiveBullets()
    {
        hasExplosiveBullets = true;
    }
    
    public bool HasExplosiveBullets()
    {
        return hasExplosiveBullets;
    }
}