using UnityEngine;
using System.Collections;
using DG.Tweening;

public class TimeManager : MonoBehaviour
{
    [SerializeField] private float slowMotionTimeScale = 0.2f;
    [SerializeField] private float transitionDuration = 0.25f;
    [SerializeField] private float hitStopDuration = 0.05f;

    private float normalTimeScale = 1f;
    private float normalFixedDeltaTime;
    private Tweener timeTween;
    private bool isHitStopping;

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
        if (InputManager.Instance != null)
        {
            InputManager.Instance.OnRightClickInitiated -= StartSlowMotion;
            InputManager.Instance.OnRightClickCanceled -= StopSlowMotion;
        }
        GameEvents.OnEnemyKilled -= TriggerHitStop;
    }

    private void StartSlowMotion()
    {
        if (isHitStopping) return;
        
        timeTween?.Kill();
        timeTween = DOTween.To(() => Time.timeScale, x => 
        {
            Time.timeScale = x;
            Time.fixedDeltaTime = normalFixedDeltaTime * x;
        }, slowMotionTimeScale, transitionDuration).SetUpdate(true);
        
        GameEvents.OnSlowMotionStarted?.Invoke();
    }

    private void StopSlowMotion()
    {
        if (isHitStopping) return;

        timeTween?.Kill();
        timeTween = DOTween.To(() => Time.timeScale, x => 
        {
            Time.timeScale = x;
            Time.fixedDeltaTime = normalFixedDeltaTime * x;
        }, normalTimeScale, transitionDuration).SetUpdate(true);
        
        GameEvents.OnSlowMotionEnded?.Invoke();
    }

    private void TriggerHitStop()
    {
        if (isHitStopping) return; 
        StartCoroutine(HitStopCoroutine());
    }

    private IEnumerator HitStopCoroutine()
    {
        isHitStopping = true;
        timeTween?.Pause();
        
        float previousTimeScale = Time.timeScale;
        Time.timeScale = 0.001f; 
        
        yield return new WaitForSecondsRealtime(hitStopDuration);
        
        Time.timeScale = previousTimeScale;
        timeTween?.Play();
        isHitStopping = false;
    }
    public void UpgradeSlowMo(float newScale)
    {
        slowMotionTimeScale = newScale;
    }
}