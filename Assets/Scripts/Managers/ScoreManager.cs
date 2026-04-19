using UnityEngine;
using TMPro;
using System.Collections;

public class ScoreManager : MonoBehaviour
{
    [SerializeField] private PlayerMovement player;
    [SerializeField] private TextMeshProUGUI distanceText;
    [SerializeField] private TextMeshProUGUI trickScoreText;

    [SerializeField] private int pointsPerBackflip = 25;
    [SerializeField] private int pointsPerKill = 10;

    private float startX;
    private int distancePoints;
    private int trickPoints;
    private int displayedTrickPoints;
    private bool isDead;
    private Coroutine scoreCoroutine;

    private void Start()
    {
        startX = player.transform.position.x;
        GameEvents.OnBackflipCompleted += AddBackflipPoints;
        GameEvents.OnEnemyKilled += AddKillPoints;
        GameEvents.OnPlayerDeath += HandlePlayerDeath;
    }

    private void OnDestroy()
    {
        GameEvents.OnBackflipCompleted -= AddBackflipPoints;
        GameEvents.OnEnemyKilled -= AddKillPoints;
        GameEvents.OnPlayerDeath -= HandlePlayerDeath;
    }

    private void Update()
    {
        if (isDead) return;

        float traveled = player.transform.position.x - startX;
        if (traveled > distancePoints)
        {
            distancePoints = Mathf.FloorToInt(traveled);
            distanceText.text = distancePoints.ToString() + "m";
        }
    }

    private void HandlePlayerDeath()
    {
        isDead = true;
    }

    private void AddBackflipPoints()
    {
        UpdateTrickScore(pointsPerBackflip);
    }

    private void AddKillPoints()
    {
        UpdateTrickScore(pointsPerKill);
    }

    private void UpdateTrickScore(int amount)
    {
        if (isDead) return;
        
        trickPoints += amount;

        if (scoreCoroutine != null)
        {
            StopCoroutine(scoreCoroutine);
        }
        
        scoreCoroutine = StartCoroutine(AnimateScoreText());
    }

    private IEnumerator AnimateScoreText()
    {
        float elapsedTime = 0f;
        int startScore = displayedTrickPoints;
        float duration = 0.4f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;
            
            displayedTrickPoints = Mathf.RoundToInt(Mathf.Lerp(startScore, trickPoints, t));
            trickScoreText.text = displayedTrickPoints.ToString();
            
            yield return null;
        }

        displayedTrickPoints = trickPoints;
        trickScoreText.text = displayedTrickPoints.ToString();
    }

    public int GetDistanceScore()
    {
        return distancePoints;
    }

    public int GetTrickScore()
    {
        return trickPoints;
    }

    public int GetTotalScore()
    {
        return GetDistanceScore() + GetTrickScore();
    }
}