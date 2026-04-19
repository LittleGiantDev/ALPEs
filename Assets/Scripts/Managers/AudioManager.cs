using UnityEngine;
using DG.Tweening;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Sources")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;

    [Header("Clips")]
    [SerializeField] private AudioClip menuMusic;
    [SerializeField] private AudioClip gameMusic;
    [SerializeField] private AudioClip shootClip;
    [SerializeField] private AudioClip aimClip;
    [SerializeField] private AudioClip reloadStartClip;
    [SerializeField] private AudioClip jumpClip;
    [SerializeField] private AudioClip bloodClip;
    [SerializeField] private AudioClip deathClip;
    [SerializeField] private AudioClip explosionClip;
    [SerializeField] private AudioClip buyClip;
    [SerializeField] private AudioClip coinClip;
    [SerializeField] private AudioClip auraClip;
    [SerializeField] private AudioClip landClip;
    [SerializeField] private AudioClip[] trickClips;
    [SerializeField] private AudioClip headshotClip;

    private int coinStreak = 0;
    private float lastCoinTime;
    private float baseMusicPitch = 1f;
    private int trickSoundIndex = 0;
    private float defaultMusicVolume;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        defaultMusicVolume = musicSource.volume;
        
        if (musicSource != null) baseMusicPitch = musicSource.pitch;
    }

    private void Start()
    {
        GameEvents.OnShoot += HandleShoot;
        GameEvents.OnReloadStarted += HandleReloadStarted;
        GameEvents.OnPlayerDeath += HandlePlayerDeath;
        GameEvents.OnEnemyKilled += HandleEnemyKilled;
        GameEvents.OnCoinCollected += HandleCoinCollected;
        GameEvents.OnBackflipCompleted += PlayTrickSequence;
        
        GameEvents.OnSlowMotionStarted += HandleSlowMotionStarted;
        GameEvents.OnSlowMotionEnded += HandleSlowMotionEnded;
    }

    private void OnDestroy()
    {
        GameEvents.OnShoot -= HandleShoot;
        GameEvents.OnReloadStarted -= HandleReloadStarted;
        GameEvents.OnPlayerDeath -= HandlePlayerDeath;
        GameEvents.OnEnemyKilled -= HandleEnemyKilled;
        GameEvents.OnCoinCollected -= HandleCoinCollected;
        GameEvents.OnBackflipCompleted -= PlayTrickSequence;
        
        GameEvents.OnSlowMotionStarted -= HandleSlowMotionStarted;
        GameEvents.OnSlowMotionEnded -= HandleSlowMotionEnded;
    }
    private void HandleShoot()
    {
        PlaySFX(shootClip);
    }

    private void HandleReloadStarted(float t)
    {
        PlaySFX(reloadStartClip);
    }

    private void HandlePlayerDeath()
    {
        PlaySFX(deathClip);
    }

    private void HandleEnemyKilled()
    {
        PlaySFX(explosionClip);
    }

    private void HandleCoinCollected(int v)
    {
        PlayCoinSound();
    }

    private void HandleSlowMotionStarted()
    {
        FadeMusicPitch(0.6f);
    }

    private void HandleSlowMotionEnded()
    {
        FadeMusicPitch(baseMusicPitch);
    }
    public void PlayMenuMusic()
    {
        if (musicSource.clip == menuMusic && musicSource.isPlaying) return;
        musicSource.volume = defaultMusicVolume;
        musicSource.clip = menuMusic;
        musicSource.Play();
    }

    public void PlayGameMusic()
    {
        if (musicSource.clip == gameMusic && musicSource.isPlaying) return;
        musicSource.volume = defaultMusicVolume;
        musicSource.clip = gameMusic;
        musicSource.Play();
    }

    public void TransitionToGameMusic(float duration)
    {
        musicSource.DOFade(0f, duration / 2f).SetUpdate(true).OnComplete(() =>
        {
            musicSource.clip = gameMusic;
            musicSource.Play();
            musicSource.DOFade(defaultMusicVolume, duration / 2f).SetUpdate(true);
        });
    }

    public void TransitionToMenuMusic(float duration)
    {
        musicSource.DOFade(0f, duration / 2f).SetUpdate(true).OnComplete(() =>
        {
            musicSource.clip = menuMusic;
            musicSource.Play();
            musicSource.DOFade(defaultMusicVolume, duration / 2f).SetUpdate(true);
        });
    }

    private void FadeMusicPitch(float targetPitch)
    {
        musicSource.DOComplete();
        musicSource.DOPitch(targetPitch, 0.4f).SetUpdate(true);
    }
    public void PlaySFX(AudioClip clip, float pitch = 1f, float volumeScale = 1f)
    {
        sfxSource.pitch = pitch;
        sfxSource.PlayOneShot(clip, volumeScale);
    }

    public void PlayCoinSound()
    {
        if (Time.unscaledTime - lastCoinTime > 0.8f)
        {
            coinStreak = 0;
        }

        float pitch = 1f + (coinStreak * 0.1f);
        pitch = Mathf.Min(pitch, 2.5f);
        
        PlaySFX(coinClip, pitch);
        
        coinStreak++;
        lastCoinTime = Time.unscaledTime;
    }

    public void PlayTrickSequence()
    {
        PlaySFX(trickClips[trickSoundIndex], 1f, 2.5f);
    
        trickSoundIndex++;
        if (trickSoundIndex >= trickClips.Length)
        {
            trickSoundIndex = 0;
        }
    }

    public void ResetTrickSequence()
    {
        trickSoundIndex = 0;
    }

    public void PlayBloodSound()
    {
        PlaySFX(bloodClip, Random.Range(0.8f, 1.2f));
    }

    public void PlayJumpSound()
    {
        PlaySFX(jumpClip);
    }

    public void PlayAimSound()
    {
        PlaySFX(aimClip);
    }
    public void PlayBuySound()
    {
        PlaySFX(buyClip);
    }

    public void PlayHeadshotSound()
    {
        PlaySFX(headshotClip);
    }

    public void PlayLandSound()
    {
        PlaySFX(landClip);
    }

    public void PlayAuraSound()
    {
        PlaySFX(auraClip);
    }
}