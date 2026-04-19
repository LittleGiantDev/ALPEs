using UnityEngine;

public class EnemyBullet : MonoBehaviour, IDamageable
{
    [SerializeField] private float defaultSpeed = 10f;
    [SerializeField] private int damage = 1;

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
        rb.linearVelocity = direction * currentSpeed;
    }

    //Invierte la dirección de la bala, aumenta su velocidad y cambia su capa cuando el jugador la golpea
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

    //Gestiona los impactos de la bala
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
            if (collision.CompareTag("Player"))
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

        if (collision.CompareTag("Ground"))
        {
            ReleaseBullet();
        }
    }

    // Frena la bala por completo y la desactiva para que el pool manual pueda reutilizarla
    private void ReleaseBullet()
    {
        if (isReleased) return;
        
        isReleased = true;
        rb.linearVelocity = Vector2.zero;
        
        gameObject.SetActive(false);
    }
}