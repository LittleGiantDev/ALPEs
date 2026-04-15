using UnityEngine;

public class TavernEntrance : MonoBehaviour
{
    private bool hasTriggered;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (hasTriggered) return;

        if (collision.CompareTag("Player"))
        {
            hasTriggered = true;
            GameEvents.OnTavernEntered?.Invoke();
        }
    }
}