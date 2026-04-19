using UnityEngine;

public class LevelChunk : MonoBehaviour
{
    [SerializeField] private Transform endPoint;

    public Vector3 EndPosition
    {
        get 
        { 
            return endPoint.position; 
        }
    }
}