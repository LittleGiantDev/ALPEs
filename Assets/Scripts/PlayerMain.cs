using UnityEngine;
using System;

public class PlayerMain : MonoBehaviour
{
    [field: SerializeField] public Rigidbody2D Rb { get; private set; }
    [field: SerializeField] public Animator Anim { get; private set; }

    public event Action OnPlayerCrash;
    public event Action OnPlayerDeath;
    public event Action<int, int> OnAmmoChanged;

    public void TriggerCrash()
    {
        OnPlayerCrash?.Invoke();
        GameEvents.OnPlayerCrash?.Invoke();
    }

    public void TriggerDeath()
    {
        OnPlayerDeath?.Invoke();
        GameEvents.OnPlayerDeath?.Invoke();
    }

    public void TriggerAmmoChanged(int current, int max)
    {
        OnAmmoChanged?.Invoke(current, max);
        GameEvents.OnAmmoChanged?.Invoke(current, max);
    }
}