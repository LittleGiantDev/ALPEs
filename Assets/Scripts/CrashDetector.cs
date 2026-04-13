using UnityEngine;

public class CrashDetector : MonoBehaviour
{
    [SerializeField] private string groundTag = "Ground";
    [SerializeField] private string grindRailTag = "GrindRail";

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.isTrigger) return;

        if (collision.CompareTag(groundTag) || collision.CompareTag(grindRailTag))
        {
            GetComponentInParent<PlayerMovement>().Crash();
        }
    }
}