using UnityEngine;

public class SpeedVFX : MonoBehaviour
{
    [SerializeField] private ParticleSystem speedParticles;
    [SerializeField] private PlayerMovement player;
    [SerializeField] private float minSpeed = 8f;
    [SerializeField] private float maxSpeed = 30f;
    
    [Header("Dynamic Settings")]
    [SerializeField] private float minEmissionRate = 20f;
    [SerializeField] private float maxEmissionRate = 120f;
    [SerializeField] private float maxSimulationSpeed = 3f;

    private ParticleSystem.EmissionModule emissionModule;
    private ParticleSystem.MainModule mainModule;

    private void Awake()
    {
        emissionModule = speedParticles.emission;
        mainModule = speedParticles.main;
    }

    private void Update()
    {
        float currentEmission = 0f;

        if (player.GetIsGrounded() && !player.GetIsDead())
        {
            float speedRatio = Mathf.InverseLerp(minSpeed, maxSpeed, player.GetCurrentSpeed());
            
            currentEmission = Mathf.Lerp(minEmissionRate, maxEmissionRate, speedRatio);
            mainModule.simulationSpeed = Mathf.Lerp(1f, maxSimulationSpeed, speedRatio);
        }

        emissionModule.rateOverTime = currentEmission;
    }
}