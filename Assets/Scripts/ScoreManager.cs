using UnityEngine;
using TMPro;
using DG.Tweening;

public class ScoreManager : MonoBehaviour
{
    [Header("Dependencies")]
    [SerializeField] private PlayerMovement player;
    [SerializeField] private ShootingController weapon;
    [SerializeField] private TrickDetector trickDetector;
    
    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI distanceText;
    [SerializeField] private TextMeshProUGUI trickScoreText;

    [Header("Settings")]
    [SerializeField] private int pointsPerBackflip = 250;
    [SerializeField] private int pointsPerKill = 100;

    private float startX;
    private int distancePoints;
    private int trickPoints;
    private int displayedTrickPoints;

    private void Start()
    {
        startX = player.transform.position.x;
        GameEvents.OnBackflipCompleted += AddBackflipPoints;
        GameEvents.OnEnemyKilled += AddKillPoints;
    }

    private void OnDestroy()
    {
        GameEvents.OnBackflipCompleted -= AddBackflipPoints;
        GameEvents.OnEnemyKilled -= AddKillPoints;
    }

    private void Update()
    {
        float traveled = player.transform.position.x - startX;
        if (traveled > distancePoints)
        {
            distancePoints = Mathf.FloorToInt(traveled);
            distanceText.text = distancePoints.ToString() + "m";
        }
    }

    private void AddBackflipPoints() => UpdateTrickScore(pointsPerBackflip);
    private void AddKillPoints() => UpdateTrickScore(pointsPerKill);

    private void UpdateTrickScore(int amount)
    {
        trickPoints += amount;
        DOTween.To(() => displayedTrickPoints, x => displayedTrickPoints = x, trickPoints, 0.4f)
            .OnUpdate(() => {
                trickScoreText.text = displayedTrickPoints.ToString();
            });
    }
}