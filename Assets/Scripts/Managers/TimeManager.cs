using UnityEngine;

public class TimeManager : MonoBehaviour
{
    [SerializeField] private float slowMotionTimeScale = 0.2f;
    [SerializeField] private float transitionDuration = 0.25f;
    [SerializeField] private float hitStopDuration = 0.05f;
    private float normalTimeScale = 1f;
    private float normalFixedDeltaTime;
    private bool    isHitStopping;
    private float targetTimeScale = 1f;
    private float startTransitionScale = 1f;
    private float transitionProgress = 1f;
    private float hitStopTimer;
    private float HitStopTimeScaleOg;

    private void Awake()
    {
        normalFixedDeltaTime = Time.fixedDeltaTime;
    }

    private void Start()
    {
        InputManager.Instance.OnRightClickInitiated += StartSlowMotion;
        InputManager.Instance.OnRightClickCanceled += StopSlowMotion;
        GameEvents.OnEnemyKilled += TriggerHitStop;
    }

    private void OnDestroy()
    {
        InputManager.Instance.OnRightClickInitiated -= StartSlowMotion;
        InputManager.Instance.OnRightClickCanceled -= StopSlowMotion;
        GameEvents.OnEnemyKilled -= TriggerHitStop;
    }

    private void Update()
    {
        if (isHitStopping)
        {
            hitStopTimer += Time.unscaledDeltaTime;
            if (hitStopTimer >= hitStopDuration)
            {
                isHitStopping = false;
                Time.timeScale = HitStopTimeScaleOg;
            }
            return;
        }

        if (transitionProgress < 1f)
        {
            transitionProgress += Time.unscaledDeltaTime / transitionDuration;
            
            if (transitionProgress >= 1f)
            {
                transitionProgress = 1f;
            }

            float currentScale = Mathf.Lerp(startTransitionScale, targetTimeScale, transitionProgress);
            Time.timeScale = currentScale;
            Time.fixedDeltaTime = normalFixedDeltaTime * currentScale;
        }
    }

    private void StartSlowMotion()
    {
        if (isHitStopping) return;
        
        startTransitionScale = Time.timeScale;
        targetTimeScale = slowMotionTimeScale;
        transitionProgress = 0f;
        
        if (GameEvents.OnSlowMotionStarted != null)
        {
            GameEvents.OnSlowMotionStarted.Invoke();
        }
    }

    private void StopSlowMotion()
    {
        if (isHitStopping) return;

        startTransitionScale = Time.timeScale;
        targetTimeScale = normalTimeScale;
        transitionProgress = 0f;
        
        if (GameEvents.OnSlowMotionEnded != null)
        {
            GameEvents.OnSlowMotionEnded.Invoke();
        }
    }

    private void TriggerHitStop()
    {
        if (isHitStopping) return; 
        
        isHitStopping = true;
        HitStopTimeScaleOg = Time.timeScale;
        Time.timeScale = 0.001f;
        hitStopTimer = 0f;
    }

    public void UpgradeSlowMo(float newScale)
    {
        slowMotionTimeScale = newScale;
    }
}