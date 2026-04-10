using UnityEngine;

public class TimeManager : MonoBehaviour
{
    [SerializeField] [Range(0.05f, 0.5f)] private float slowmotionTime = 0.2f;

    private float normalTimeScale = 1f;
    private float normalFixedDeltaTime;

    private void Awake()
    {
        normalFixedDeltaTime = Time.fixedDeltaTime;
    }

    private void OnEnable()
    {
            InputManager.Instance.OnRightClickInitiated += StartSlowMotion;
            InputManager.Instance.OnRightClickCanceled += StopSlowMotion;
        
    }

    private void OnDisable()
    {
            InputManager.Instance.OnRightClickInitiated -= StartSlowMotion;
            InputManager.Instance.OnRightClickCanceled -= StopSlowMotion;
        
    }

    private void StartSlowMotion()
    {
        Time.timeScale = slowmotionTime;
        Time.fixedDeltaTime = normalFixedDeltaTime * slowmotionTime;
    }

    private void StopSlowMotion()
    {
        Time.timeScale = normalTimeScale;
        Time.fixedDeltaTime = normalFixedDeltaTime;
    }
}