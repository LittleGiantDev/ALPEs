using UnityEngine;
using Unity.Cinemachine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private CinemachineCamera vcam;
    [SerializeField] private Transform player;
    [SerializeField] private Transform cameraTarget;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float raycastLength = 150f;
    [SerializeField] private float lookAheadDistance = 5f;
    
    [SerializeField] private float minZoom = 15f;
    [SerializeField] private float maxZoom = 35f;
    [SerializeField] private float zoomSmoothSpeed = 4f;

    [SerializeField] private float verticalSmoothTime = 0.2f;
    [SerializeField] private float groundWeight = 0.4f; 
    
    private bool isPlayerDead;
    private float currentVelocityY;
    private float lastKnownTargetY;

    private void Start()
    {
        GameEvents.OnPlayerDeath += () => isPlayerDead = true;
    }

    private void OnDestroy()
    {
        GameEvents.OnPlayerDeath -= () => isPlayerDead = true;
    }
    
    private void LateUpdate()
    {
        if (isPlayerDead) return;

        float targetX = player.position.x;
        float targetY = lastKnownTargetY;
        float targetZoom = minZoom;

        Vector2 rayOrigin = new Vector2(player.position.x + lookAheadDistance, player.position.y);
        RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.down, raycastLength, groundLayer);

        if (hit.collider != null)
        {
            float distance = hit.distance;
            targetZoom = Mathf.Clamp(minZoom + (distance * 0.45f), minZoom, maxZoom);
            targetY = Mathf.Lerp(player.position.y, hit.point.y, groundWeight);
            lastKnownTargetY = targetY;
        }
        else
        {
            targetZoom = maxZoom;
            targetY = lastKnownTargetY;
        }

        vcam.Lens.OrthographicSize = Mathf.Lerp(vcam.Lens.OrthographicSize, targetZoom, Time.unscaledDeltaTime * zoomSmoothSpeed);

        float smoothY = Mathf.SmoothDamp(cameraTarget.position.y, targetY, ref currentVelocityY, verticalSmoothTime, Mathf.Infinity, Time.unscaledDeltaTime);
        cameraTarget.position = new Vector3(targetX, smoothY, 0f);
    }
}