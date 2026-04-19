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

    private void Update()
    {
        float currentEmission = 0f;
        float currentSimSpeed = 1f;

        bool isGrounded = player.GetIsGrounded();
        bool isDead = player.GetIsDead();

        if (isGrounded)
        {
            if (isDead == false)
            {
                float speed = player.GetCurrentSpeed();
                float range = maxSpeed - minSpeed;
                float time = (speed - minSpeed) / range;

                if (time < 0f)
                {
                    time = 0f;
                }
                if (time > 1f)
                {
                    time = 1f;
                }

                currentEmission = minEmissionRate + (time * (maxEmissionRate - minEmissionRate));
                currentSimSpeed = 1f + (time * (maxSimulationSpeed - 1f));
            }
        }

        var em = speedParticles.emission;
        em.rateOverTime = currentEmission;

        var main = speedParticles.main;
        main.simulationSpeed = currentSimSpeed;
    }
}