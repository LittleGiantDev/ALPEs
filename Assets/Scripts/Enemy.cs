using UnityEngine;

public class Enemy : MonoBehaviour, IDamageable
{
    [Header("Stats")]
    [SerializeField] private int health = 100;
    
    [Header("Loot")]
    [SerializeField] private GameObject coinPrefab;
    [SerializeField] private int minCoinsDrop = 2;
    [SerializeField] private int maxCoinsDrop = 5;

    [Header("Ragdoll")]
    [SerializeField] private Rigidbody2D[] ragdollParts;
    [SerializeField] private Animator animator;
    [SerializeField] private float impactForceMultiplier = 50f;

    [Header("VFX")]
    [SerializeField] private ParticleSystem bloodVFXPrefab;

    private bool isDead;
    private EnemyAI ai;

    private void Start()
    {
        ai = GetComponent<EnemyAI>();
        SetRagdollState(false);
    }

    public void TakeDamage(int amount, Vector2 hitPoint, Vector2 shotDirection, Rigidbody2D hitPart = null)
    {
        if (bloodVFXPrefab != null)
        {
            ParticleSystem blood = Instantiate(bloodVFXPrefab, hitPoint, Quaternion.LookRotation(shotDirection));
            var main = blood.main;
            Destroy(blood.gameObject, main.duration);
        }

        if (isDead)
        {
            if (hitPart != null) hitPart.AddForceAtPosition(shotDirection * impactForceMultiplier, hitPoint, ForceMode2D.Impulse);
            return;
        }

        health -= amount;
        
        if (health <= 0)
        {
            Die(hitPoint, shotDirection, hitPart);
        }
    }

    private void Die(Vector2 hitPoint, Vector2 shotDirection, Rigidbody2D hitPart)
    {
        isDead = true;
        
        if (ai != null)
        {
            ai.StopAI();
        }
        
        SetRagdollState(true); 

        if (hitPart != null)
        {
            hitPart.AddForceAtPosition(shotDirection * impactForceMultiplier, hitPoint, ForceMode2D.Impulse);
        }

        DropLoot(shotDirection);
    }

    private void DropLoot(Vector2 shotDirection)
    {
        int coinsToDrop = Random.Range(minCoinsDrop, maxCoinsDrop + 1);
        
        for (int i = 0; i < coinsToDrop; i++)
        {
            Vector3 spawnPos = transform.position + Vector3.up * 0.5f;
            GameObject coinObj = Instantiate(coinPrefab, spawnPos, Quaternion.identity);
            
            Coin coin = coinObj.GetComponent<Coin>();
            if (coin != null) coin.SpawnEffect(shotDirection);
        }
    }

    private void SetRagdollState(bool state)
    {
        if (animator != null) animator.enabled = !state;

        foreach (Rigidbody2D rb in ragdollParts)
        {
            rb.bodyType = state ? RigidbodyType2D.Dynamic : RigidbodyType2D.Kinematic;
        }
    }
}