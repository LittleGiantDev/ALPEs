using UnityEngine;
using UnityEngine.Pool;

public class EnemyAI : MonoBehaviour
{
    private enum State { Patrolling, Attacking }

    [SerializeField] private Transform[] patrolPoints;
    [SerializeField] private float movementSpeed = 3f;
    [SerializeField] private float idleWaitTime = 2f;

    [SerializeField] private float detectionRange = 15f;
    [SerializeField] private float minShootDelay = 1f;
    [SerializeField] private float maxShootDelay = 3f;
    [SerializeField] private EnemyBullet bulletPrefab;
    [SerializeField] private Transform firePoint;

    [SerializeField] private Animator animator;
    [SerializeField] private string idleParam = "isIdle";
    [SerializeField] private string moveParam = "isMoving";

    private State currentState;
    private int currentPointIndex;
    private Transform player;
    private Vector3[] fixedPatrolCoordinates;
    
    private ObjectPool<EnemyBullet> bulletPool;
    
    private float waitTimer = 0f;
    private float shootTimer = 0f;
    private bool isMoving;
    private bool isDead;
    private bool moveWhileAttacking;

    private void Awake()
    {
        bulletPool = new ObjectPool<EnemyBullet>(
            createFunc: OnCreateBullet,
            actionOnGet: OnGetBullet,
            actionOnRelease: OnReleaseBullet,
            actionOnDestroy: OnDestroyPoolObject,
            defaultCapacity: 10,
            maxSize: 20
        );
    }

    private void Start()
    {
        fixedPatrolCoordinates = new Vector3[patrolPoints.Length];
        for (int i = 0; i < patrolPoints.Length; i++)
        {
            if (patrolPoints[i] != null)
            {
                fixedPatrolCoordinates[i] = patrolPoints[i].position;
                patrolPoints[i].SetParent(null); 
            }
        }

        currentState = State.Patrolling;
    }

    private void Update()
    {
        if (isDead) return;

        if (player == null)
        {
            GameObject pObj = GameObject.FindGameObjectWithTag("Player");
            if (pObj != null)
            {
                player = pObj.transform;
            }
            else
            {
                return; 
            }
        }

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        State nextState = (distanceToPlayer <= detectionRange) ? State.Attacking : State.Patrolling;

        if (nextState == State.Attacking && currentState == State.Patrolling)
        {
            moveWhileAttacking = Random.value > 0.5f;
            SetAnimationState(moveWhileAttacking);
            shootTimer = 0.2f;        
        }

        currentState = nextState;

        if (currentState == State.Patrolling)
        {
            HandlePatrol();
        }
        else if (currentState == State.Attacking)
        {
            HandleAttack();
        }
    }

    private void HandlePatrol()
    {
        if (fixedPatrolCoordinates.Length == 0) return;

        Vector3 targetPoint = fixedPatrolCoordinates[currentPointIndex];

        if (Vector2.Distance(transform.position, targetPoint) > 0.2f)
        {
            SetAnimationState(true);
            transform.position = Vector2.MoveTowards(transform.position, targetPoint, movementSpeed * Time.deltaTime);
            FlipTowards(targetPoint);
        }
        else
        {
            SetAnimationState(false);
            waitTimer += Time.deltaTime;
            if (waitTimer >= idleWaitTime)
            {
                waitTimer = 0f;
                currentPointIndex = (currentPointIndex + 1) % fixedPatrolCoordinates.Length;
            }
        }
    }

    private void HandleAttack()
    {
        FlipTowards(player.position);

        if (moveWhileAttacking && fixedPatrolCoordinates.Length > 0)
        {
            Vector3 targetPoint = fixedPatrolCoordinates[currentPointIndex];

            if (Vector2.Distance(transform.position, targetPoint) > 0.2f)
            {
                transform.position = Vector2.MoveTowards(transform.position, targetPoint, movementSpeed * Time.deltaTime);
                SetAnimationState(true);
            }
            else
            {
                currentPointIndex = (currentPointIndex + 1) % fixedPatrolCoordinates.Length;
                SetAnimationState(true);
            }
        }
        else
        {
            SetAnimationState(false);
        }

        shootTimer -= Time.deltaTime;
        if (shootTimer <= 0f)
        {
            Shoot();
            shootTimer = Random.Range(minShootDelay, maxShootDelay); 
        }
    }

    private void Shoot()
    {
        if (bulletPrefab == null || firePoint == null) return;

        EnemyBullet bullet = bulletPool.Get();
        Vector2 dir = (player.position - firePoint.position).normalized;
        bullet.transform.position = firePoint.position + (Vector3)dir * 0.8f; 
        bullet.Setup(dir);
    }

    private void FlipTowards(Vector2 target)
    {
        float yRotation = target.x > transform.position.x ? 0f : 180f;
        transform.rotation = Quaternion.Euler(0f, yRotation, 0f);
    }

    private void SetAnimationState(bool moving)
    {
        if (isMoving == moving) return;
        isMoving = moving;
        
        if (animator != null)
        {
            animator.SetBool(idleParam, !isMoving);
            animator.SetBool(moveParam, isMoving);
        }
    }

    public void StopAI()
    {
        isDead = true;
        this.enabled = false;
    }

    private EnemyBullet OnCreateBullet()
    {
        EnemyBullet bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
        bullet.MyPool = bulletPool; 
        return bullet;
    }

    private void OnGetBullet(EnemyBullet bullet)
    {
        bullet.gameObject.SetActive(true); 
    }

    private void OnReleaseBullet(EnemyBullet bullet)
    {
        bullet.gameObject.SetActive(false); 
    }

    private void OnDestroyPoolObject(EnemyBullet bullet)
    {
        Destroy(bullet.gameObject);
    }
}