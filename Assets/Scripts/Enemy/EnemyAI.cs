using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    [SerializeField] private Transform[] patrolPoints;
    [SerializeField] private float movementSpeed = 3f;
    [SerializeField] private float idleWaitTime = 2f;

    [SerializeField] private float detectionRange = 15f;
    [SerializeField] private float minShootDelay = 1f;
    [SerializeField] private float maxShootDelay = 3f;
    [SerializeField] private EnemyBullet bulletPrefab;
    [SerializeField] private Transform firePoint;
    
    [SerializeField] private int initialPoolSize = 10;

    [SerializeField] private Animator animator;
    [SerializeField] private string idleAnim = "isIdle";
    [SerializeField] private string moveAnim = "isMoving";

    private int currentPointIndex;
    private Transform player;
    private Vector3[] patrolCordinates;
    
    private List<EnemyBullet> bulletPool = new List<EnemyBullet>();
    
    private float waitTimer = 0f;
    private float shootTimer = 0f;
    private bool isMoving;
    private bool isDead;
    private bool isAttacking;
    private bool moveAttacking;

    private void Awake()
    {
        for (int i = 0; i < initialPoolSize; i++)
        {
            CreateNewBullet();
        }
    }

    //Configura las coordenadas fijas de los puntos de patrulla
    private void Start()
    {
        patrolCordinates = new Vector3[patrolPoints.Length];
        for (int i = 0; i < patrolPoints.Length; i++)
        {
            patrolCordinates[i] = new Vector3(patrolPoints[i].position.x, patrolPoints[i].position.y, 0);
            Destroy(patrolPoints[i].gameObject);
        }
    }

    //Comprueba la distancia con el jugador y la lógica
    private void Update()
    {
        if (isDead) return;

        if (player == null)
        {
            GameObject pObj = GameObject.FindGameObjectWithTag("Player");
            player = pObj.transform;
        }

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        
        bool wasAttacking = isAttacking;

        if (distanceToPlayer <= detectionRange)
        {
            isAttacking = true;
        }
        else
        {
            isAttacking = false;
        }

        if (isAttacking && !wasAttacking)
        {
            // Determina aleatoriamente si el enemigo se moverá mientras ataca o no
            if (Random.value > 0.5f)
            {
                moveAttacking = true;
            }
            else
            {
                moveAttacking = false;
            }

            SetAnimationState(moveAttacking);
            shootTimer = 0.5f;      
        }

        if (isAttacking)
        {
            HandleAttack();
        }
        else
        {
            HandlePatrol();
        }
    }

    //Mueve al enemigo entre los puntos de patrulla configurados
    private void HandlePatrol()
    {
        Vector3 targetPoint = patrolCordinates[currentPointIndex];

        if (Vector2.Distance(transform.position, targetPoint) > 0.2f)
        {
            SetAnimationState(true);
            transform.position = Vector3.MoveTowards(transform.position, targetPoint, movementSpeed * Time.deltaTime);
        }
        else
        {
            SetAnimationState(false);
            waitTimer += Time.deltaTime;
            if (waitTimer >= idleWaitTime)
            {
                waitTimer = 0f;
                currentPointIndex = (currentPointIndex + 1) % patrolCordinates.Length;
            }
        }
    }

    //Gestiona el disparo y el movimiento en combate
    private void HandleAttack()
    {
        if (moveAttacking)
        {
            Vector3 targetPoint = patrolCordinates[currentPointIndex];

            if (Vector2.Distance(transform.position, targetPoint) > 0.2f)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPoint, movementSpeed * Time.deltaTime);
                SetAnimationState(true);
            }
            else
            {
                currentPointIndex = (currentPointIndex + 1) % patrolCordinates.Length;
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

    //Obtiene una bala del pool y la dispara hacia el jugador
    private void Shoot()
    {
        EnemyBullet bullet = GetPooledBullet();
        Vector2 dir = (player.position - firePoint.position).normalized;
        
        bullet.transform.position = firePoint.position + (Vector3)dir * 0.8f; 
        bullet.gameObject.SetActive(true);
        bullet.Setup(dir);
    }

    private void SetAnimationState(bool moving)
    {
        if (isMoving == moving) return;
        isMoving = moving;
        animator.SetBool(idleAnim, !isMoving);
        animator.SetBool(moveAnim, isMoving);
    }

    public void StopAI()
    {
        isDead = true;
        this.enabled = false;
    }

    //Busca una bala desactivada en el pool o crea una nueva si no hay disponibles
    private EnemyBullet GetPooledBullet()
    {
        for (int i = 0; i < bulletPool.Count; i++)
        {
            if (bulletPool[i].gameObject.activeInHierarchy == false)
            {
                return bulletPool[i];
            }
        }
        return CreateNewBullet();
    }

    //Instancia una nueva bala y la añade a la lista
    private EnemyBullet CreateNewBullet()
    {
        EnemyBullet bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
        bullet.gameObject.SetActive(false);
        bulletPool.Add(bullet);
        return bullet;
    }
}