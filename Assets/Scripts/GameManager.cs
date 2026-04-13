using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject gameOverUI;
    [SerializeField] private float restartDelay = 2f;

    private void Start()
    {
        if (gameOverUI != null) gameOverUI.SetActive(false);
        GameEvents.OnPlayerCrash += InitiateGameOver;
    }

    private void OnDestroy()
    {
        GameEvents.OnPlayerCrash -= InitiateGameOver;
    }

    private void InitiateGameOver()
    {
        StartCoroutine(GameOverCoroutine());
    }

    private IEnumerator GameOverCoroutine()
    {
        yield return new WaitForSeconds(restartDelay);
        
        if (gameOverUI != null)
        {
            gameOverUI.SetActive(true);
        }
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}