using UnityEngine;

public interface IDamageable
{
    void TakeDamage(int amount, Vector2 hitPoint, Vector2 shotDirection, Rigidbody2D hitPart = null);
}