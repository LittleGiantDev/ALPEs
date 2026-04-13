using UnityEngine;

public class ImpactVFX : MonoBehaviour
{
    [SerializeField] private ParticleSystem landingParticlesPrefab;
    [SerializeField] private Transform playerTransform;
    [SerializeField] private LayerMask groundLayer;

    private ContactFilter2D filter;
    private RaycastHit2D[] hits = new RaycastHit2D[1];

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
            ParticleSystem vfx = Instantiate(landingParticlesPrefab, hits[0].point, Quaternion.FromToRotation(Vector3.up, hits[0].normal));
            Destroy(vfx.gameObject, 2f);
        }
    }
}