using UnityEngine;

public class PlayerDeathHandler : PlayerSystem
{
    [Header("Referencias Visuales")]
    [SerializeField] private GameObject aliveGraphics;
    [SerializeField] private GameObject weapon;
    
    [Header("Referencias Ragdoll")]
    [SerializeField] private GameObject ragdollMain;
    [SerializeField] private Transform[] aliveBones;
    [SerializeField] private Transform[] ragdollBones;
    [SerializeField] private Rigidbody2D[] ragdollRb;
    [SerializeField] private Collider2D[] ragdollColliders;
    
    [Header("Objetos Clavables")]
    [SerializeField] private Rigidbody2D leftSki;
    [SerializeField] private Rigidbody2D rightSki;
    [SerializeField] private Rigidbody2D rifleRb; 
    [SerializeField] private Collider2D rifleCollider;
    [SerializeField] private float stickVelocity = 15f;

    protected override void Awake()
    {
        base.Awake();
        SetRagdollState(false);
    }

    private void Start()
    {
        main.OnPlayerDeath += HandleDeath;
    }

    private void OnDestroy()
    {
        main.OnPlayerDeath -= HandleDeath;
    }

    private void HandleDeath()
    {
        Vector2 impactVelocity = main.Rb.linearVelocity;

        main.Rb.simulated = false;
        Collider2D mainCollider = main.GetComponent<Collider2D>();
        mainCollider.enabled = false;
        
        LaunchRifle(impactVelocity);

        aliveGraphics.SetActive(false);
        weapon.SetActive(false);

        MatchBones();
        SetRagdollState(true);

        foreach (Rigidbody2D rb in ragdollRb)
        {
            rb.linearVelocity = impactVelocity;
        }

        leftSki.gameObject.AddComponent<SkiSticker>().Setup(stickVelocity);
        rightSki.gameObject.AddComponent<SkiSticker>().Setup(stickVelocity);
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
        ragdollMain.SetActive(active);

        foreach (Rigidbody2D rb in ragdollRb)
        {
            if (rb.simulated == true)
            {
                rb.bodyType = RigidbodyType2D.Dynamic;
            }
        }

        foreach (Collider2D collider in ragdollColliders)
        {
            collider.enabled = active;
        }
    }

    private void LaunchRifle(Vector2 impactVelocity)
    {
        if (rifleRb == null || rifleCollider == null) return;

        rifleRb.transform.SetParent(null);
        rifleRb.gameObject.SetActive(true);

        rifleRb.bodyType = RigidbodyType2D.Dynamic;
        rifleCollider.enabled = true;

        rifleRb.linearVelocity = impactVelocity + new Vector2(Random.Range(-5f, 5f), Random.Range(5f, 10f));
        rifleRb.AddTorque(Random.Range(-300f, 300f));

        rifleRb.gameObject.AddComponent<SkiSticker>().Setup(stickVelocity * 0.5f);
    }
}