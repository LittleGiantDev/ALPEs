using UnityEngine;

public class Enemy : MonoBehaviour, IDamageable
{
    [SerializeField] private int health = 100;
    [SerializeField] private ParticleSystem bloodVFXPrefab;

    public void TakeDamage(int amount, Vector2 hitPoint)
    {
        if (health <= 0) return;

        if (bloodVFXPrefab != null)
        {
            Instantiate(bloodVFXPrefab, hitPoint, Quaternion.identity);
        }

        health -= amount;
        
        if (health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Destroy(gameObject);
    }
}