using UnityEngine;

public class PlayerDeathHandler : PlayerSystem
{
    [Header("Visual References")]
    [SerializeField] private GameObject aliveGraphics;
    [SerializeField] private GameObject weaponPivot;
    
    [Header("Ragdoll References")]
    [SerializeField] private GameObject ragdollRoot;
    [SerializeField] private Transform[] aliveBones;
    [SerializeField] private Transform[] ragdollBones;
    [SerializeField] private Rigidbody2D[] ragdollRbs;
    [SerializeField] private Collider2D[] ragdollColliders;
    
    [Header("Stickable Objects")]
    [SerializeField] private Rigidbody2D leftSki;
    [SerializeField] private Rigidbody2D rightSki;
    [SerializeField] private Rigidbody2D rifleRb; 
    [SerializeField] private Collider2D rifleCollider;
    [SerializeField] private float stickVelocityThreshold = 15f;

    protected override void Awake()
    {
        base.Awake();
        SetRagdollState(false);
    }

    private void Start()
    {
        if (main != null)
        {
            main.OnPlayerDeath += HandleDeath;
        }
    }

    private void OnDestroy()
    {
        if (main != null)
        {
            main.OnPlayerDeath -= HandleDeath;
        }
    }

    private void HandleDeath()
    {
        Vector2 impactVelocity = main.Rb.linearVelocity;

        main.Rb.simulated = false;
        
        Collider2D mainCol = main.GetComponent<Collider2D>();
        if (mainCol != null)
        {
            mainCol.enabled = false;
        }

        MatchBones();
        
        if (ragdollRoot != null)
        {
            ragdollRoot.transform.SetParent(null);
        }

        if (aliveGraphics != null)
        {
            aliveGraphics.SetActive(false);
        }

        if (weaponPivot != null)
        {
            weaponPivot.SetActive(false);
        }

        SetRagdollState(true);

        if (ragdollRbs != null)
        {
            for (int i = 0; i < ragdollRbs.Length; i++)
            {
                if (ragdollRbs[i] != null)
                {
                    ragdollRbs[i].linearVelocity = impactVelocity;
                }
            }
        }

        LaunchRifle(impactVelocity);
        LaunchSki(leftSki, impactVelocity);
        LaunchSki(rightSki, impactVelocity);
    }

    private void LaunchSki(Rigidbody2D skiRb, Vector2 impactVelocity)
    {
        if (skiRb == null) return;
        
        skiRb.transform.SetParent(null);
        skiRb.gameObject.SetActive(true);
        
        skiRb.bodyType = RigidbodyType2D.Dynamic;
        skiRb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        
        Collider2D col = skiRb.GetComponent<Collider2D>();
        if (col != null)
        {
            col.enabled = true;
        }
        
        skiRb.linearVelocity = impactVelocity;
        
        if (skiRb.GetComponent<SkiSticker>() == null)
        {
            skiRb.gameObject.AddComponent<SkiSticker>().Setup(stickVelocityThreshold);
        }
    }

    private void LaunchRifle(Vector2 impactVelocity)
    {
        if (rifleRb == null || rifleCollider == null) return;

        rifleRb.transform.SetParent(null);
        rifleRb.gameObject.SetActive(true);

        rifleRb.bodyType = RigidbodyType2D.Dynamic;
        rifleRb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        rifleCollider.enabled = true;

        rifleRb.linearVelocity = impactVelocity + new Vector2(Random.Range(-5f, 5f), Random.Range(5f, 10f));
        rifleRb.AddTorque(Random.Range(-300f, 300f));

        if (rifleRb.GetComponent<SkiSticker>() == null)
        {
            rifleRb.gameObject.AddComponent<SkiSticker>().Setup(stickVelocityThreshold * 0.5f);
        }
    }

    private void MatchBones()
    {
        if (aliveBones == null || ragdollBones == null) return;

        for (int i = 0; i < aliveBones.Length; i++)
        {
            if (i < ragdollBones.Length && aliveBones[i] != null && ragdollBones[i] != null)
            {
                ragdollBones[i].position = aliveBones[i].position;
                ragdollBones[i].rotation = aliveBones[i].rotation;
            }
        }
    }

    private void SetRagdollState(bool active)
    {
        if (ragdollRoot != null)
        {
            ragdollRoot.SetActive(active);
        }

        if (ragdollRbs != null)
        {
            for (int i = 0; i < ragdollRbs.Length; i++)
            {
                if (ragdollRbs[i] != null)
                {
                    ragdollRbs[i].simulated = active;
                    if (active)
                    {
                        ragdollRbs[i].bodyType = RigidbodyType2D.Dynamic;
                    }
                }
            }
        }

        if (ragdollColliders != null)
        {
            for (int i = 0; i < ragdollColliders.Length; i++)
            {
                if (ragdollColliders[i] != null)
                {
                    ragdollColliders[i].enabled = active;
                }
            }
        }
    }
}