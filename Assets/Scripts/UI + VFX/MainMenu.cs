using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using DG.Tweening;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI highScoreText;
    [SerializeField] private CanvasGroup menuGroup;
    
    private bool isStarting;

    private void Start()
    {
        highScoreText.text = " " + SaveManager.GetRecord();
        
        AudioManager.Instance.PlayMenuMusic();
        InputManager.Instance.OnJumpInitiated += TryStartGame;
    }

    private void OnDestroy()
    { 
        InputManager.Instance.OnJumpInitiated -= TryStartGame;
    }

    private void TryStartGame()
    {
        if (!isStarting)
        {
            StartGame();
        }
    }

    private void StartGame()
    {
        isStarting = true;
        
        AudioManager.Instance.TransitionToGameMusic(0.5f);

        if (menuGroup != null)
        {
            menuGroup.DOFade(0, 0.5f).OnComplete(() => {
                SceneManager.LoadScene("Game");
            });
        }
        else
        {
            SceneManager.LoadScene("Game");
        }
    }
}