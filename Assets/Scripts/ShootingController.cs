using UnityEngine;
using System.Collections;

public class ShootingController : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float weaponRange = 50f;
    [SerializeField] private LayerMask enemyLayer;

    [Header("Dependencies")]
    [SerializeField] private Transform firePoint;
    [SerializeField] private LineRenderer laserLine;

    private void Start()
    {
        InputManager.Instance.OnLeftClickInitiated += Shoot;
    }

    private void OnDestroy()
    {
        if (InputManager.Instance != null)
        {
            InputManager.Instance.OnLeftClickInitiated -= Shoot;
        }
    }

    private void Shoot()
    {
        StartCoroutine(ShowLaserCoroutine());

        RaycastHit2D hit = Physics2D.Raycast(firePoint.position, firePoint.right, weaponRange, enemyLayer);

        if (hit.collider != null)
        {
            Debug.Log(hit.collider.name);
        }
    }

    private IEnumerator ShowLaserCoroutine()
    {
        laserLine.enabled = true;
        laserLine.SetPosition(0, firePoint.position);

        RaycastHit2D hit = Physics2D.Raycast(firePoint.position, firePoint.right, weaponRange, enemyLayer);
        if (hit.collider != null)
        {
            laserLine.SetPosition(1, hit.point);
        }
        else
        {
            laserLine.SetPosition(1, firePoint.position + firePoint.right * weaponRange);
        }

        yield return new WaitForSeconds(0.05f);
        laserLine.enabled = false;
    }
}