using System;
using System.Collections;
using UnityEngine;

public class ShootingController : MonoBehaviour
{
    [SerializeField] private float weaponRange = 50f;
    [SerializeField] private int damage = 100;
    [SerializeField] private LayerMask hitLayers;
    
    [SerializeField] private float flashThicknessMultiplier = 3f;
    [SerializeField] private float flashDuration = 0.05f;

    [SerializeField] private Transform firePoint;
    [SerializeField] private LineRenderer laserLine;

    private bool isAiming;
    private float originalLineWidth;

    private void Awake()
    {
        originalLineWidth = laserLine.startWidth;
    }

    private void Start()
    {
        InputManager.Instance.OnLeftClickInitiated += Shoot;
        InputManager.Instance.OnRightClickInitiated += StartAiming;
        InputManager.Instance.OnRightClickCanceled += StopAiming;
    }

    private void OnDestroy()
    {
        if (InputManager.Instance != null)
        {
            InputManager.Instance.OnLeftClickInitiated -= Shoot;
            InputManager.Instance.OnRightClickInitiated -= StartAiming;
            InputManager.Instance.OnRightClickCanceled -= StopAiming;
        }
    }

    private void Update()
    {
        if (isAiming)
        {
            UpdateLaserPosition();
        }
    }

    private void StartAiming()
    {
        isAiming = true;
        laserLine.enabled = true;
    }

    private void StopAiming()
    {
        isAiming = false;
        laserLine.enabled = false;
    }

    private void UpdateLaserPosition()
    {
        laserLine.SetPosition(0, firePoint.position);
        
        RaycastHit2D hit = Physics2D.Raycast(firePoint.position, firePoint.right, weaponRange, hitLayers);

        if (hit.collider != null)
        {
            laserLine.SetPosition(1, hit.point);
        }
        else
        {
            laserLine.SetPosition(1, firePoint.position + firePoint.right * weaponRange);
        }
    }

    private void Shoot()
    {
        if (!isAiming) return;

        GameEvents.OnShoot?.Invoke();

        StartCoroutine(ShootFlashCoroutine());

        RaycastHit2D hit = Physics2D.Raycast(firePoint.position, firePoint.right, weaponRange, hitLayers);

        if (hit.collider != null)
        {
            IDamageable damageable = hit.collider.GetComponent<IDamageable>();
            
            if (damageable != null)
            {
                damageable.TakeDamage(damage, hit.point);
                
                if (hit.collider.CompareTag("Enemy"))
                {
                    GameEvents.OnEnemyKilled?.Invoke();
                }
            }
        }
    }

    private IEnumerator ShootFlashCoroutine()
    {
        laserLine.startWidth = originalLineWidth * flashThicknessMultiplier;
        laserLine.endWidth = originalLineWidth * flashThicknessMultiplier;
        
        yield return new WaitForSecondsRealtime(flashDuration);
        
        laserLine.startWidth = originalLineWidth;
        laserLine.endWidth = originalLineWidth;
    }
}