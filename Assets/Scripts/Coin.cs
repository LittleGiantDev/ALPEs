using UnityEngine;
using System.Collections;

public class Coin : MonoBehaviour
{
    [SerializeField] private int value = 1;
    [SerializeField] private float waitBeforeHoming = 1.2f;
    [SerializeField] private float initialFlySpeed = 5f;
    [SerializeField] private float acceleration = 15f;
    
    private Transform playerTransform;
    private Rigidbody2D rb;
    private Collider2D col;
    private bool isHoming;
    private float currentSpeed;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
    }

    public void SpawnEffect(Vector2 shotDirection)
    {
        float randomUp = Random.Range(4f, 8f);
        float randomSide = Random.Range(-6f, 6f);
        Vector2 push = new Vector2(shotDirection.x * 5f + randomSide, randomUp);
        
        rb.AddForce(push, ForceMode2D.Impulse);
        rb.AddTorque(Random.Range(-500f, 500f));

        StartCoroutine(StartHomingRoutine());
    }

    private IEnumerator StartHomingRoutine()
    {
        yield return new WaitForSeconds(waitBeforeHoming);
        
        isHoming = true;
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.linearVelocity = Vector2.zero;
        col.enabled = false;
        currentSpeed = initialFlySpeed;
    }

    private void Update()
    {
        if (!isHoming) return;

        currentSpeed += acceleration * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, playerTransform.position, currentSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, playerTransform.position) < 0.5f)
        {
            GameEvents.OnCoinCollected?.Invoke(value);
            Destroy(gameObject);
        }
    }
}