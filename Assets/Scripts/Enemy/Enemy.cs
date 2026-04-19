using UnityEngine;

public class Enemy : MonoBehaviour, IDamageable
{
    [Header("Stats")]
    [SerializeField] private int health = 100;
    
    [Header("Loot")]
    [SerializeField] private GameObject coinPrefab;
    [SerializeField] private int minCoins = 2;
    [SerializeField] private int maxCoins = 5;

    [Header("Ragdoll")]
    [SerializeField] private Rigidbody2D[] ragdollParts;
    [SerializeField] private Animator animator;
    [SerializeField] private float impactForce = 50f;

    [Header("Effects")]
    [SerializeField] private ParticleSystem bloodPrefab;

    private bool isDead;
    private EnemyAI enemyAI;

    private void Start()
    {
        enemyAI = GetComponent<EnemyAI>();
        SetRagdollState(false);
    }

    public void TakeDamage(int amount, Vector2 hitPoint, Vector2 shotDirection, Rigidbody2D hitPart = null)
    { 
        ParticleSystem blood = Instantiate(bloodPrefab, hitPoint,  Quaternion.identity);
        AudioManager.Instance.PlayBloodSound();

        if (isDead)
        {
            if (hitPart != null)
            {
                hitPart.AddForceAtPosition(shotDirection * impactForce, hitPoint, ForceMode2D.Impulse);
            }
            return;
        }

        int finalDamage = amount;
    
        if (hitPart != null && hitPart.gameObject.name == "Head")
        {
            finalDamage *= 2;
            AudioManager.Instance.PlayHeadshotSound();
        }

        health -= finalDamage;
    
        if (health <= 0)
        {
            GameEvents.OnEnemyKilled?.Invoke();
            Die(hitPoint, shotDirection, hitPart);
        }
    }

    private void Die(Vector2 hitPoint, Vector2 shotDirection, Rigidbody2D hitPart)
    {
        isDead = true;
        enemyAI.StopAI();
        
        SetRagdollState(true); 
        hitPart.AddForceAtPosition(shotDirection * impactForce, hitPoint, ForceMode2D.Impulse);

        DropLoot(shotDirection);
    }

    private void DropLoot(Vector2 shotDirection)
    {
        
        int coinsToDrop = Random.Range(minCoins, maxCoins + 1);
        
        for (int i = 0; i < coinsToDrop; i++)
        {
            Vector3 spawnPosition = transform.position + Vector3.up * 0.5f;
            GameObject coinObject = Instantiate(coinPrefab, spawnPosition, Quaternion.identity);
            coinObject.GetComponent<Coin>().SpawnEffect(shotDirection * 3);
        }
    }

    private void SetRagdollState(bool state)
    {
        animator.enabled = !state;

        foreach (Rigidbody2D rb in ragdollParts)
        {
            if (state)
            {
                rb.bodyType = RigidbodyType2D.Dynamic;
            }
            else
            {
                rb.bodyType = RigidbodyType2D.Kinematic;
            }
        }
    }
}