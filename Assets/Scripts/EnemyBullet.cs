using UnityEngine;
using UnityEngine.Pool;

public class EnemyBullet : MonoBehaviour, IDamageable
{
    [SerializeField] private float defaultSpeed = 10f;
    [SerializeField] private int damage = 1;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private LayerMask groundLayer;

    public ObjectPool<EnemyBullet> MyPool { get; set; }

    private Vector2 direction;
    private bool isReflected;
    private float currentSpeed;
    private Rigidbody2D rb;
    
    private bool isReleased; 

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void Setup(Vector2 targetDirection)
    {
        direction = targetDirection.normalized;
        currentSpeed = defaultSpeed;
        isReflected = false;
        
        isReleased = false; 

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }

    private void FixedUpdate()
    {
        if (rb != null)
        {
            rb.linearVelocity = direction * currentSpeed;
        }
    }

    public void TakeDamage(int amount, Vector2 hitPoint, Vector2 shotDirection, Rigidbody2D hitPart = null)
    {
        if (isReflected || isReleased) return;

        isReflected = true;
        direction = -direction; 
        currentSpeed *= 1.5f; 
        gameObject.layer = LayerMask.NameToLayer("PlayerBullet"); 

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isReleased) return; 

        if (isReflected)
        {
            IDamageable target = collision.GetComponentInParent<IDamageable>();
            if (target != null && !collision.CompareTag("Player"))
            {
                target.TakeDamage(damage, transform.position, direction);
                ReleaseBullet();
            }
        }
        else
        {
            if (((1 << collision.gameObject.layer) & playerLayer) != 0)
            {
                PlayerMovement playerMove = collision.transform.root.GetComponentInChildren<PlayerMovement>();
                
                if (playerMove != null && !playerMove.GetIsDead())
                {
                    playerMove.Crash();
                }
                ReleaseBullet();
                return;
            }
        }

        if (((1 << collision.gameObject.layer) & groundLayer) != 0)
        {
            ReleaseBullet();
        }
    }

    private void ReleaseBullet()
    {
        if (isReleased) return;
        
        isReleased = true;

        if (MyPool != null)
        {
            if (rb != null)
            {
                rb.linearVelocity = Vector2.zero;
            }
            MyPool.Release(this);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}