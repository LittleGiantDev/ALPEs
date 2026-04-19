using UnityEngine;

public class Coin : MonoBehaviour
{
    [SerializeField] private int value = 1;
    [SerializeField] private float waitBeforeFollow = 1.2f;
    [SerializeField] private float initialFlySpeed = 5f;
    [SerializeField] private float acceleration = 15f;
    
    private Transform playerTransform;
    private Rigidbody2D rb;
    private Collider2D colllider;
    private bool isFollowing;
    private float currentSpeed;
    private float followTimer;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        colllider = GetComponent<Collider2D>();
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
    }

    public void SpawnEffect(Vector2 shotDirection)
    {
        float randomUp = Random.Range(4f, 8f);
        float randomSide = Random.Range(-6f, 6f);
        Vector2 push = new Vector2(shotDirection.x * 5f + randomSide, randomUp);
        
        rb.AddForce(push, ForceMode2D.Impulse);
        rb.AddTorque(Random.Range(-360f, 360f));
        
        followTimer = waitBeforeFollow;
    }

    private void Update()
    {
        if (isFollowing == false)
        {
            if (followTimer > 0)
            {
                followTimer -= Time.deltaTime;
                if (followTimer <= 0)
                {
                    StartFollowing();
                }
            }
            return;
        }

        currentSpeed += acceleration * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, playerTransform.position, currentSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, playerTransform.position) < 0.5f)
        {
            GameEvents.OnCoinCollected.Invoke(value);
            Destroy(gameObject);
        }
    }

    private void StartFollowing()
    {
        isFollowing = true;
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.linearVelocity = Vector2.zero;
        colllider.enabled = false;
        currentSpeed = initialFlySpeed;
    }
}