using UnityEngine;
using DG.Tweening;

public class GrindRope : MonoBehaviour
{
    [SerializeField] private Transform startPoint;
    [SerializeField] private Transform endPoint;
    [SerializeField] private LineRenderer lineRenderer;
    
    [Header("Sag Settings")]
    [SerializeField] private float restingSag = 0.5f; 
    [SerializeField] private float maxSag = 2.0f;     
    [SerializeField] private float sagDuration = 0.3f;
    [SerializeField] private float contactOffset = 0.5f; 
    [SerializeField] private int resolution = 20;

    private EdgeCollider2D edgeCollider;
    private Vector2[] colliderPoints;
    private float currentLocalY;
    private float currentLocalX;
    private bool isPlayerInZone;
    private bool isGrinding;
    private Transform playerTransform;

    private void Awake()
    {
        edgeCollider = GetComponent<EdgeCollider2D>();
        
        if (lineRenderer == null) lineRenderer = GetComponent<LineRenderer>();
            
        lineRenderer.positionCount = resolution;
        lineRenderer.useWorldSpace = false;
        colliderPoints = new Vector2[resolution];
        
        currentLocalY = -restingSag;
    }

    private void Start()
    {
        Vector3 localStart = transform.InverseTransformPoint(startPoint.position);
        Vector3 localEnd = transform.InverseTransformPoint(endPoint.position);
        currentLocalX = (localStart.x + localEnd.x) / 2f;
        
        UpdateElements();
    }

    private void Update()
    {
        UpdateElements();
    }

    private void UpdateElements()
    {
        Vector3 localStart = transform.InverseTransformPoint(startPoint.position);
        Vector3 localEnd = transform.InverseTransformPoint(endPoint.position);
        float centerX = (localStart.x + localEnd.x) / 2f;

        if (isPlayerInZone && playerTransform != null)
        {
            Vector3 playerLocalPos = transform.InverseTransformPoint(playerTransform.position);
            currentLocalX = Mathf.Clamp(playerLocalPos.x, localStart.x, localEnd.x);

            if (!isGrinding && playerLocalPos.y <= currentLocalY + contactOffset)
            {
                isGrinding = true;
                DOTween.Kill(this);
                DOTween.To(() => currentLocalY, x => currentLocalY = x, -maxSag, sagDuration).SetEase(Ease.OutQuad).SetId(this);
                GameEvents.OnGrindStarted?.Invoke();
            }
        }
        else
        {
            currentLocalX = Mathf.Lerp(currentLocalX, centerX, Time.deltaTime * 7f);
        }

        Vector3 localDipPoint = new Vector3(currentLocalX, currentLocalY, 0f);

        for (int i = 0; i < resolution; i++)
        {
            float t = i / (float)(resolution - 1);
            
            Vector2 m1 = Vector2.Lerp(localStart, localDipPoint, t);
            Vector2 m2 = Vector2.Lerp(localDipPoint, localEnd, t);
            Vector2 curvePoint = Vector2.Lerp(m1, m2, t);

            colliderPoints[i] = curvePoint;
            lineRenderer.SetPosition(i, curvePoint);
        }

        edgeCollider.points = colliderPoints;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !isPlayerInZone)
        {
            isPlayerInZone = true;
            playerTransform = collision.transform;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && isPlayerInZone)
        {
            isPlayerInZone = false;
            isGrinding = false;
            playerTransform = null;
            
            DOTween.Kill(this);
            DOTween.To(() => currentLocalY, x => currentLocalY = x, -restingSag, 0.5f).SetEase(Ease.OutElastic).SetId(this);
            GameEvents.OnGrindEnded?.Invoke();
        }
    }
}