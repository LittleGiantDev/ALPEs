using UnityEngine;

public class GrindRope : MonoBehaviour
{
    [SerializeField] private Transform[] segments;
    
    private EdgeCollider2D edgeCollider;
    private Vector2[] colliderPoints;

    private void Awake()
    {
        edgeCollider = GetComponent<EdgeCollider2D>();
        
        if (segments != null)
        {
            colliderPoints = new Vector2[segments.Length];
        }
    }

    private void Update()
    {
        if (segments == null || segments.Length == 0) return;

        for (int i = 0; i < segments.Length; i++)
        {
            Vector3 localPos = transform.InverseTransformPoint(segments[i].position);
            colliderPoints[i] = localPos;
        }

        edgeCollider.points = colliderPoints;
    }
}