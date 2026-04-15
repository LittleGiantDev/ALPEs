using UnityEngine;

public class ImpactVFX : MonoBehaviour
{
    public static ImpactVFX instance;

    [SerializeField] private ParticleSystem landingParticlesPrefab;
    [SerializeField] private Transform playerTransform;
    [SerializeField] private LayerMask groundLayer;

    private ContactFilter2D filter;
    private RaycastHit2D[] hits = new RaycastHit2D[1];

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        filter = new ContactFilter2D();
        filter.SetLayerMask(groundLayer);
        filter.useTriggers = false;

        GameEvents.OnPerfectLanding += SpawnLandingParticles;
    }

    private void OnDestroy()
    {
        GameEvents.OnPerfectLanding -= SpawnLandingParticles;
    }

    private void SpawnLandingParticles()
    {
        if (landingParticlesPrefab == null) return;

        if (Physics2D.Raycast(playerTransform.position, Vector2.down, filter, hits, 2f) > 0)
        {
            if (!hits[0].collider.CompareTag("GrindRail"))
            {
                SpawnAtPoint(hits[0].point, hits[0].normal);
            }
        }
    }

    public static void SpawnAtPoint(Vector2 position, Vector2 normal)
    {
        if (instance == null || instance.landingParticlesPrefab == null) return;
        
        ParticleSystem vfx = Instantiate(instance.landingParticlesPrefab, position, Quaternion.FromToRotation(Vector3.up, normal));
        Destroy(vfx.gameObject, 2f);
    }
}