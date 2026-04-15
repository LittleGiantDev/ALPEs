using UnityEngine;

public class PlayerSystem : MonoBehaviour
{
    protected PlayerMain main;

    protected virtual void Awake()
    {
        main = transform.root.GetComponent<PlayerMain>();
    }
}