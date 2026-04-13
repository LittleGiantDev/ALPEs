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
    public static Action OnGrindStarted;
    public static Action OnGrindEnded;
}