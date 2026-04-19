using UnityEngine;

public class BackgroundChunk : MonoBehaviour
{
    [SerializeField] private Transform startPoint;
    [SerializeField] private Transform endPoint;

    public Vector3 StartPosition
    {
        get { return startPoint.position; }
    }

    public Vector3 EndPosition
    {
        get { return endPoint.position; }
    }
}