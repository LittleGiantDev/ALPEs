using System;
using System.Collections;
using UnityEngine;

public class ShootingController : MonoBehaviour
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

    [Header("References")]
    [SerializeField] private Transform firePoint;
    [SerializeField] private LineRenderer laserLine;

    private int currentAmmo;
    private float nextFireTime;
    private bool isReloading;
    private bool isAiming;

    private void Start()
    {
        currentAmmo = maxAmmo;
        GameEvents.OnAmmoChanged?.Invoke(currentAmmo, maxAmmo);

        InputManager.Instance.OnLeftClickInitiated += TryShoot;
        InputManager.Instance.OnRightClickInitiated += () => { isAiming = true; laserLine.enabled = true; };
        InputManager.Instance.OnRightClickCanceled += () => { isAiming = false; laserLine.enabled = false; };
        InputManager.Instance.OnReloadInitiated += HandleReloadInput;
    }

    private void Update()
    {
        if (isAiming) UpdateLaser();
    }

    private void UpdateLaser()
    {
        laserLine.SetPosition(0, firePoint.position);
        RaycastHit2D hit = Physics2D.Raycast(firePoint.position, firePoint.right, weaponRange, hitLayers);
        laserLine.SetPosition(1, hit.collider != null ? hit.point : (Vector2)firePoint.position + (Vector2)firePoint.right * weaponRange);
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
        if (!isAiming || isReloading || Time.time < nextFireTime) return;
        Shoot();
    }

    private void Shoot()
    {
        currentAmmo--;
        nextFireTime = Time.time + fireRate;
        GameEvents.OnAmmoChanged?.Invoke(currentAmmo, maxAmmo);
        
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
        GameEvents.OnAmmoChanged?.Invoke(currentAmmo, maxAmmo);
        GameEvents.OnReloadFinished?.Invoke();
    }

    private void OnDestroy()
    {
        if (InputManager.Instance != null)
        {
            InputManager.Instance.OnReloadInitiated -= HandleReloadInput;
        }
    }
}