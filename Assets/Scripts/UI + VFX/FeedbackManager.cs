using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using Unity.Cinemachine;

public class FeedbackManager : MonoBehaviour
{
    [Header("Camera Shake")]
    [SerializeField] private CinemachineImpulseSource impulseSource;
    [SerializeField] private float shootShakeForce = 0.5f;
    [SerializeField] private float landingShakeForce = 1f;
    [SerializeField] private float crashShakeForce = 2.5f;

    [Header("Visuals")]
    [SerializeField] private Transform playerVisuals;

    [Header("Post Processing")]
    [SerializeField] private Volume globalVolume;
    [SerializeField] private float slowMoAberration = 0.6f;
    [SerializeField] private float slowMoDistortion = -0.3f;
    [SerializeField] private float transitionDuration = 0.25f;

    private ChromaticAberration chromaticAberration;
    private LensDistortion lensDistortion;

    private float targetChromaticAberration;
    private float startCA;
    private float progressCA = 1f;
    private float durationCA;

    private float targetLensDistorsion;
    private float startLD;
    private float progressLD = 1f;
    private float durationLD;

    private Vector3 targetScale = Vector3.one;
    private Vector3 startScale = Vector3.one;
    private float progressScale = 1f;
    private float durationScale;

    private void Start()
    {
        globalVolume.profile.TryGet(out chromaticAberration);
        globalVolume.profile.TryGet(out lensDistortion);

        GameEvents.OnShoot += HandleShootFeedback;
        GameEvents.OnPerfectLanding += HandleLandingFeedback;
        GameEvents.OnPlayerCrash += HandleCrashFeedback;
        GameEvents.OnSlowMotionStarted += HandleSlowMoStart;
        GameEvents.OnSlowMotionEnded += HandleSlowMoEnd;
    }

    private void OnDestroy()
    {
        GameEvents.OnShoot -= HandleShootFeedback;
        GameEvents.OnPerfectLanding -= HandleLandingFeedback;
        GameEvents.OnPlayerCrash -= HandleCrashFeedback;
        GameEvents.OnSlowMotionStarted -= HandleSlowMoStart;
        GameEvents.OnSlowMotionEnded -= HandleSlowMoEnd;
    }

    private void Update()
    {
        if (progressCA < 1f)
        {
            progressCA += Time.unscaledDeltaTime / durationCA;
            if (progressCA > 1f) progressCA = 1f;
            chromaticAberration.intensity.value = Mathf.Lerp(startCA, targetChromaticAberration, progressCA);
        }

        if (progressLD < 1f)
        {
            progressLD += Time.unscaledDeltaTime / durationLD;
            if (progressLD > 1f) progressLD = 1f;
            lensDistortion.intensity.value = Mathf.Lerp(startLD, targetLensDistorsion, progressLD);
        }

        if (progressScale < 1f)
        {
            progressScale += Time.unscaledDeltaTime / durationScale;
            if (progressScale > 1f) progressScale = 1f;
            playerVisuals.localScale = Vector3.Lerp(startScale, targetScale, progressScale);
        }
    }

    private void HandleShootFeedback()
    {
        impulseSource.GenerateImpulseWithForce(shootShakeForce);

        startCA = 1f;
        targetChromaticAberration = 0f;
        durationCA = 0.2f;
        progressCA = 0f;
    }

    private void HandleLandingFeedback()
    {
        impulseSource.GenerateImpulseWithForce(landingShakeForce);

        startScale = new Vector3(1.5f, 0.4f, 1f);
        targetScale = Vector3.one;
        durationScale = 0.3f;
        progressScale = 0f;
        playerVisuals.localScale = startScale;
    }

    private void HandleCrashFeedback()
    {
        impulseSource.GenerateImpulseWithForce(crashShakeForce);

        startLD = -0.8f;
        targetLensDistorsion = 0f;
        durationLD = 1f;
        progressLD = 0f;
    }

    private void HandleSlowMoStart()
    {
        startCA = chromaticAberration.intensity.value;
        targetChromaticAberration = slowMoAberration;
        durationCA = transitionDuration;
        progressCA = 0f;

        startLD = lensDistortion.intensity.value;
        targetLensDistorsion = slowMoDistortion;
        durationLD = transitionDuration;
        progressLD = 0f;
    }

    private void HandleSlowMoEnd()
    {
        startCA = chromaticAberration.intensity.value;
        targetChromaticAberration = 0f;
        durationCA = transitionDuration;
        progressCA = 0f;

        startLD = lensDistortion.intensity.value;
        targetLensDistorsion = 0f;
        durationLD = transitionDuration;
        progressLD = 0f;
    }
}