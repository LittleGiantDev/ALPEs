using UnityEngine;

public class TrickDetector : MonoBehaviour
{
    [SerializeField] private PlayerMovement player;
    [SerializeField] private float trickThreshold = 280f;
    [SerializeField] private GameObject coinPrefab;
    [SerializeField] private int coinsPerTrick = 15;

    private float currentAirRotation;

    private void FixedUpdate()
    {
        if (player.GetIsDead()) return;

        if (!player.GetIsGrounded())
        {
            float deltaRotation = Mathf.Abs(player.GetAngularVelocity()) * Time.fixedDeltaTime;
            currentAirRotation += deltaRotation;

            if (currentAirRotation >= trickThreshold)
            {
                GameEvents.OnBackflipCompleted?.Invoke();
                SpawnComboCoins();
                currentAirRotation -= 360f;
            }
        }
        else
        {
            currentAirRotation = 0f;
        }
    }

    private void SpawnComboCoins()
    {
        if (coinPrefab == null) return;

        for (int i = 0; i < coinsPerTrick; i++)
        {
            GameObject coinObj = Instantiate(coinPrefab, player.transform.position, Quaternion.identity);
            Coin coin = coinObj.GetComponent<Coin>();
            if (coin != null) coin.SpawnEffect(Vector2.up);
        }
    }
}