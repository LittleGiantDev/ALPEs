using UnityEngine;

public class CrashDetector : MonoBehaviour
{
    [SerializeField] private string groundTag = "Ground";
    [SerializeField] private string bridgeTag = "Bridge";

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.isTrigger) return;

        if (collision.CompareTag(groundTag) || collision.CompareTag(bridgeTag))
        {
            GetComponentInParent<PlayerMovement>().Crash();
        }
    }
}