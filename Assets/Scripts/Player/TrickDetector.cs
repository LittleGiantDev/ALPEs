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
        if (player.GetIsDead() || player.enabled == false)
        {
            currentAirRotation = 0f;
            return;
        }

        if (player.GetIsGrounded() == false)
        {
            float angularVelocity = player.GetAngularVelocity();
            
            if (angularVelocity < 0f)
            {
                angularVelocity = -angularVelocity;
            }

            float rotation = angularVelocity * Time.fixedDeltaTime;
            currentAirRotation += rotation;

            if (currentAirRotation >= trickThreshold)
            {
                GameEvents.OnBackflipCompleted.Invoke();
                
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
        for (int i = 0; i < coinsPerTrick; i++)
        {
            GameObject coinObj = Instantiate(coinPrefab, player.transform.position, Quaternion.identity);
            coinObj.GetComponent<Coin>().SpawnEffect(Vector2.up);
        }
    }
}