using System;
using System.Collections;
using UnityEngine;

public class ShootingController : PlayerSystem
{
    [Header("Basic Stats")]
    [SerializeField] private float weaponRange = 50f;
    [SerializeField] private int damage = 100;
    [SerializeField] private LayerMask hitLayers;
    [SerializeField] private LayerMask groundLayer;

    [Header("Ammo & Fire Rate")]
    [SerializeField] private int maxAmmo = 10;
    [SerializeField] private float fireRate = 0.2f;
    [SerializeField] private float reloadTime = 1.5f;
    
    [Header("Explosive Settings")]
    [SerializeField] private float explosionRadius = 3.5f;
    [SerializeField] private GameObject explosionVFXPrefab;

    [Header("References")]
    [SerializeField] private Transform firePoint;
    [SerializeField] private LineRenderer laserLine;

    private int currentAmmo;
    private float nextFireTime;
    private bool isReloading;
    private bool isAiming;
    private bool hasExplosiveBullets;

    protected override void Awake()
    {
        base.Awake();
    }

    private void Start()
    {
        currentAmmo = maxAmmo;
        main.TriggerAmmoChanged(currentAmmo, maxAmmo);

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
        if (!isReloading && currentAmmo < maxAmmo)
        {
            StartCoroutine(Reload());
        }
    }

    private void TryShoot()
    {
        if (!isAiming || isReloading || Time.time < nextFireTime)
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
            StartCoroutine(Reload());
        }
    }

    private void HandleHit(RaycastHit2D hit)
    {
        if (((1 << hit.collider.gameObject.layer) & groundLayer) != 0 && !hit.collider.CompareTag("GrindRail"))
        {
            ImpactVFX.SpawnAtPoint(hit.point, hit.normal);
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
        if (explosionVFXPrefab != null)
        {
            GameObject vfx = Instantiate(explosionVFXPrefab, point, Quaternion.identity);
            Destroy(vfx, 2f);
        }

        Collider2D[] colliders = Physics2D.OverlapCircleAll(point, explosionRadius);
        foreach (Collider2D col in colliders)
        {
            IDamageable dmg = col.GetComponentInParent<IDamageable>();
            if (dmg != null)
            {
                Rigidbody2D rbPart = col.GetComponent<Rigidbody2D>();
                dmg.TakeDamage(damage, point, firePoint.right, rbPart);
            }
        }
    }

    private void ApplyDirectDamage(RaycastHit2D hit)
    {
        IDamageable damageable = hit.collider.GetComponentInParent<IDamageable>();
        if (damageable != null)
        {
            Rigidbody2D rbPart = hit.collider.GetComponent<Rigidbody2D>();
            damageable.TakeDamage(damage, hit.point, firePoint.right, rbPart);
        }
    }

    private IEnumerator Reload()
    {
        isReloading = true;
        GameEvents.OnReloadStarted?.Invoke(reloadTime);

        yield return new WaitForSeconds(reloadTime);

        currentAmmo = maxAmmo;
        isReloading = false;
        
        main.TriggerAmmoChanged(currentAmmo, maxAmmo);
        GameEvents.OnReloadFinished?.Invoke();
    }

    private void OnDestroy()
    {
        if (InputManager.Instance != null)
        {
            InputManager.Instance.OnLeftClickInitiated -= TryShoot;
            InputManager.Instance.OnRightClickInitiated -= HandleAimStart;
            InputManager.Instance.OnRightClickCanceled -= HandleAimEnd;
            InputManager.Instance.OnReloadInitiated -= HandleReloadInput;
        }
    }

    private void HandleAimStart()
    {
        isAiming = true;
        laserLine.enabled = true;
    }

    private void HandleAimEnd()
    {
        isAiming = false;
        laserLine.enabled = false;
    }

    public void UpgradeMaxAmmo(int extraAmmo)
    {
        maxAmmo += extraAmmo;
        currentAmmo = maxAmmo;
        main.TriggerAmmoChanged(currentAmmo, maxAmmo);
    }

    public void UpgradeFireRate(float reduction)
    {
        fireRate = Mathf.Max(0.05f, fireRate - reduction);
    }

    public void UpgradeReloadSpeed(float reduction)
    {
        reloadTime = Mathf.Max(0.2f, reloadTime - reduction);
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