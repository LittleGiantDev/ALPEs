using System;

public static class GameEvents
{
    public static Action OnPlayerCrash;
    public static Action OnEnemyKilled;
    public static Action OnBackflipCompleted;
    public static Action OnPerfectLanding;
    public static Action OnShoot;
    public static Action OnSlowMotionStarted;
    public static Action OnSlowMotionEnded;
    public static Action<int> OnCoinCollected;
    public static Action<int, int> OnAmmoChanged;
    public static Action<float> OnReloadStarted; 
    public static Action OnReloadFinished;
    public static Action OnPlayerDeath;
    public static Action OnTavernEntered;
    public static Action OnTavernExited;
}