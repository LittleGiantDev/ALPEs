using UnityEngine;

public class ImpactParticles : MonoBehaviour
{
    public static ImpactParticles instance;

    [SerializeField] private ParticleSystem landingParticles;
    [SerializeField] private Transform playerTransform;

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
        GameEvents.OnPerfectLanding += SpawnLandingParticles;
    }

    private void OnDestroy()
    {
        GameEvents.OnPerfectLanding -= SpawnLandingParticles;
    }

    private void SpawnLandingParticles()
    {
        RaycastHit2D[] hits = Physics2D.RaycastAll(playerTransform.position, Vector2.down, 2f);

        for (int i = 0; i < hits.Length; i++)
        {
            if (hits[i].collider != null)
            {
                if (hits[i].collider.CompareTag("Ground"))
                {
                    if (hits[i].collider.CompareTag("Bridge") == false)
                    {
                        SpawnAtPoint(hits[i].point, hits[i].normal);
                        break;
                    }
                }
            }
        }
    }

    public static void SpawnAtPoint(Vector2 position, Vector2 normal)
    {
        if (instance != null)
        {
            Instantiate(instance.landingParticles, position, Quaternion.FromToRotation(Vector3.up, normal));
        }
    }
}