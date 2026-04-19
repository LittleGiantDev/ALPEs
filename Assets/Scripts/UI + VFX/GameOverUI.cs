using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class GameOverUI : MonoBehaviour
{
    [SerializeField] private ScoreManager scoreManager;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private RectTransform rectPanel;
    
    [SerializeField] private TextMeshProUGUI distanceText;
    [SerializeField] private TextMeshProUGUI styleText;
    [SerializeField] private TextMeshProUGUI totalScoreText;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button mainMenuButton;
    
    [SerializeField] private float delay = 2f;

    private int tempDistance;
    private int tempStyle;
    private float lastSoundTime;

    private bool isGameOverActive;
    private float stateTimer;
    private int currentPhase;

    private int finalDistance;
    private int finalStyle;
    private int finalTotal;

    private void Start()
    {
        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
        rectPanel.localScale = Vector3.zero;

        distanceText.text = "0";
        styleText.text = "0";
        totalScoreText.text = "0";

        GameEvents.OnPlayerDeath += InitiateGameOver;
    }

    private void OnDestroy()
    {
        GameEvents.OnPlayerDeath -= InitiateGameOver;
    }

    private void Update()
    {
        if (isGameOverActive == false)
        {
            return;
        }

        stateTimer += Time.unscaledDeltaTime;

        if (currentPhase == 0)
        {
            if (stateTimer >= delay)
            {
                ShowPanel();
                stateTimer = 0f;
                currentPhase = 1;
            }
        }
        else if (currentPhase == 1)
        {
            if (stateTimer >= 0.8f)
            {
                finalDistance = scoreManager.GetDistanceScore();
                stateTimer = 0f;
                currentPhase = 2;
            }
        }
        else if (currentPhase == 2)
        {
            float timer = stateTimer / 0.8f;
            if (timer > 1f) timer = 1f;

            int nextVal = Mathf.RoundToInt(Mathf.Lerp(0, finalDistance, timer));
            if (nextVal != tempDistance)
            {
                tempDistance = nextVal;
                distanceText.text = tempDistance.ToString();
                PlayTickSound(distanceText.transform);
            }

            if (timer >= 1f)
            {
                stateTimer = 0f;
                currentPhase = 3;
            }
        }
        else if (currentPhase == 3)
        {
            if (stateTimer >= 0.2f)
            {
                finalStyle = scoreManager.GetTrickScore();
                stateTimer = 0f;
                currentPhase = 4;
            }
        }
        else if (currentPhase == 4)
        {
            float timer = stateTimer / 0.8f;
            if (timer > 1f) timer = 1f;

            int nextVal = Mathf.RoundToInt(Mathf.Lerp(0, finalStyle, timer));
            if (nextVal != tempStyle)
            {
                tempStyle = nextVal;
                styleText.text = tempStyle.ToString();
                PlayTickSound(styleText.transform);
            }

            if (timer >= 1f)
            {
                stateTimer = 0f;
                currentPhase = 5;
            }
        }
        else if (currentPhase == 5)
        {
            if (stateTimer >= 0.4f)
            {
                FinalizeScore();
                isGameOverActive = false;
            }
        }
    }

    private void InitiateGameOver()
    {
        isGameOverActive = true;
        stateTimer = 0f;
        currentPhase = 0;
    }

    private void ShowPanel()
    {
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
        canvasGroup.DOFade(1f, 0.3f).SetUpdate(true);
        rectPanel.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack).SetUpdate(true);
    }

    private void FinalizeScore()
    {
        finalTotal = scoreManager.GetTotalScore();
        int previousRecord = SaveManager.GetRecord(); 
        SaveManager.SaveRecord(finalTotal);

        totalScoreText.text = finalTotal.ToString();
        totalScoreText.transform.DOPunchScale(Vector3.one * 0.5f, 0.5f, 10, 1f).SetUpdate(true);
        
        AudioManager.Instance.PlayBuySound();

        if (finalTotal > previousRecord)
        {
            totalScoreText.transform.DOPunchRotation(new Vector3(0, 0, 10), 0.5f).SetUpdate(true);
        }
        
        restartButton.transform.DOScale(1.1f, 0.5f).SetLoops(-1, LoopType.Yoyo).SetUpdate(true);
        mainMenuButton.transform.DOScale(1.1f, 0.5f).SetLoops(-1, LoopType.Yoyo).SetUpdate(true);
    }

    public void RestartGame()
    {
        restartButton.interactable = false;
        mainMenuButton.interactable = false;

        AudioManager.Instance.PlayGameMusic();
        SceneManager.LoadScene("Game");
    }

    public void ReturnToMainMenu()
    {
        restartButton.interactable = false;
        mainMenuButton.interactable = false;
        AudioManager.Instance.TransitionToMenuMusic(0.5f);

        canvasGroup.DOFade(0f, 0.5f).SetUpdate(true).OnComplete(LoadMainMenu);
    }

    private void LoadMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
    
    private void PlayTickSound(Transform textTransform)
    {
        if (Time.unscaledTime - lastSoundTime > 0.08f)
        {
            textTransform.DOPunchScale(Vector3.one * 0.05f, 0.08f).SetUpdate(true);
           
            AudioManager.Instance.PlayCoinSound();
            lastSoundTime = Time.unscaledTime;
        }
    }
}