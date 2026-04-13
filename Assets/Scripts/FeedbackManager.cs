using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using Unity.Cinemachine;
using DG.Tweening;

public class FeedbackManager : MonoBehaviour
{
    [Header("Camera Shake (Cinemachine)")]
    [SerializeField] private CinemachineImpulseSource impulseSource;
    [SerializeField] private float shootShakeForce = 0.5f;
    [SerializeField] private float landingShakeForce = 1f;
    [SerializeField] private float crashShakeForce = 2.5f;

    [Header("Player Visuals")]
    [SerializeField] private Transform playerVisuals;

    [Header("Post Processing (URP Volume)")]
    [SerializeField] private Volume globalVolume;
    [SerializeField] private float slowMoAberration = 0.6f;
    [SerializeField] private float slowMoDistortion = -0.3f;
    [SerializeField] private float transitionDuration = 0.25f;

    private ChromaticAberration chromaticAberration;
    private LensDistortion lensDistortion;
    
    private Tweener caTween;
    private Tweener ldTween;

    private void Start()
    {
        if (globalVolume != null && globalVolume.profile != null)
        {
            globalVolume.profile.TryGet(out chromaticAberration);
            globalVolume.profile.TryGet(out lensDistortion);
        }

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

    private void HandleShootFeedback()
    {
        impulseSource?.GenerateImpulseWithForce(shootShakeForce);

        if (chromaticAberration != null)
        {
            caTween?.Kill();
            chromaticAberration.intensity.value = 1f;
            caTween = DOTween.To(() => chromaticAberration.intensity.value, x => chromaticAberration.intensity.value = x, 0f, 0.2f).SetUpdate(true);
        }
    }

    private void HandleLandingFeedback()
    {
        impulseSource?.GenerateImpulseWithForce(landingShakeForce);

        if (playerVisuals != null)
        {
            playerVisuals.DOComplete();
            
            Sequence squashSequence = DOTween.Sequence();
            squashSequence.Append(playerVisuals.DOScale(new Vector3(1.5f, 0.4f, 1f), 0.1f).SetEase(Ease.OutQuad));
            squashSequence.Append(playerVisuals.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutElastic));
            squashSequence.SetUpdate(true);
        }
    }

    private void HandleCrashFeedback()
    {
        impulseSource?.GenerateImpulseWithForce(crashShakeForce);

        if (lensDistortion != null)
        {
            ldTween?.Kill();
            lensDistortion.intensity.value = -0.8f;
            ldTween = DOTween.To(() => lensDistortion.intensity.value, x => lensDistortion.intensity.value = x, 0f, 1f).SetUpdate(true);
        }
    }

    private void HandleSlowMoStart()
    {
        if (chromaticAberration != null)
        {
            caTween?.Kill();
            caTween = DOTween.To(() => chromaticAberration.intensity.value, x => chromaticAberration.intensity.value = x, slowMoAberration, transitionDuration).SetUpdate(true);
        }

        if (lensDistortion != null)
        {
            ldTween?.Kill();
            ldTween = DOTween.To(() => lensDistortion.intensity.value, x => lensDistortion.intensity.value = x, slowMoDistortion, transitionDuration).SetUpdate(true);
        }
    }

    private void HandleSlowMoEnd()
    {
        if (chromaticAberration != null)
        {
            caTween?.Kill();
            caTween = DOTween.To(() => chromaticAberration.intensity.value, x => chromaticAberration.intensity.value = x, 0f, transitionDuration).SetUpdate(true);
        }

        if (lensDistortion != null)
        {
            ldTween?.Kill();
            ldTween = DOTween.To(() => lensDistortion.intensity.value, x => lensDistortion.intensity.value = x, 0f, transitionDuration).SetUpdate(true);
        }
    }
}