using System;
using UnityEngine;

public class TrickDetector : MonoBehaviour
{
    [SerializeField] private PlayerMovement player;
    [SerializeField] private float trickThreshold = 280f;


    private float currentAirRotation;

    private void FixedUpdate()
    {
        if (!player.GetIsGrounded())
        {
            float deltaRotation = Mathf.Abs(player.GetAngularVelocity()) * Time.fixedDeltaTime;
            currentAirRotation += deltaRotation;

            if (currentAirRotation >= trickThreshold)
            {
                GameEvents.OnBackflipCompleted?.Invoke();
                currentAirRotation -= 360f;
            }
        }
        else
        {
            currentAirRotation = 0f;
        }
    }
}