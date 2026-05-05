using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class TavernManager : MonoBehaviour
{
    [Header("Visuals")]
    [SerializeField] private Transform fourthWall;
    [SerializeField] private Button continueButton;
    [SerializeField] private float wallMoveY = 10f;
    [SerializeField] private float transitionTime = 1.2f;
    [SerializeField] private CanvasGroup shopUI;
    [SerializeField] private float zoomAmount = 5f;

    [Header("Player Control")]
    [SerializeField] private Transform playerAnchor; 

    private Camera mainCamera;
    private float originalCamSize;
    private Vector3 wallStartPos;
    private PlayerMovement playerMove;
    private ShootingController playerShoot;
    private Rigidbody2D playerRb;

    private void Awake()
    {
        mainCamera = Camera.main;
        
        if (mainCamera != null)
        {
            originalCamSize = mainCamera.orthographicSize;
        }
        
        if (fourthWall != null)
        {
            wallStartPos = fourthWall.position;
        }
        
        if (shopUI != null)
        {
            shopUI.alpha = 0f;
            shopUI.interactable = false;
            shopUI.blocksRaycasts = false;
        }
    }

    private void Start()
    {
        GameEvents.OnTavernEntered += StartTavernSequence;
    }

    private void OnDestroy()
    {
        GameEvents.OnTavernEntered -= StartTavernSequence;
    }

    private void StartTavernSequence()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        
        if (player != null)
        {
            playerMove = player.GetComponent<PlayerMovement>();
            playerShoot = player.GetComponent<ShootingController>();
            playerRb = player.GetComponent<Rigidbody2D>();
        }

        if (continueButton != null)
        {
            continueButton.transform.DOScale(1.05f, 0.6f).SetLoops(-1, LoopType.Yoyo).SetUpdate(true);
        }
        
        if (playerMove != null)
        {
            playerMove.SetTavernState(true);
            playerMove.enabled = false;
        }

        if (playerShoot != null)
        {
            playerShoot.enabled = false;
        }

        if (playerRb != null)
        {
            playerRb.linearVelocity = Vector2.zero;
            playerRb.angularVelocity = 0f;
            playerRb.bodyType = RigidbodyType2D.Kinematic;
        }

        if (player != null && playerAnchor != null)
        {
            player.transform.DOMove(playerAnchor.position, 1f).SetEase(Ease.OutQuad);
            player.transform.DORotate(Vector3.zero, 1f).SetEase(Ease.OutQuad);
        }

        if (fourthWall != null)
        {
            fourthWall.DOMoveY(wallStartPos.y + wallMoveY, transitionTime);
        }

        if (mainCamera != null)
        {
            mainCamera.DOOrthoSize(zoomAmount, transitionTime).OnComplete(() => ShowShopUI());
        }
    }

    private void ShowShopUI()
    {
        if (shopUI != null)
        {
            shopUI.DOFade(1f, 0.5f);
            shopUI.interactable = true;
            shopUI.blocksRaycasts = true;
        }
    }

    public void ResumeGame()
    {
        if (continueButton != null)
        {
            continueButton.transform.DOComplete();
            continueButton.transform.DOPunchScale(Vector3.one * 0.2f, 0.3f).SetUpdate(true);
        }
        
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayBuySound();
        }
        
        if (shopUI != null)
        {
            shopUI.DOFade(0f, 0.3f).OnComplete(() => HideShopAndMoveWall());
        }
    }

    private void HideShopAndMoveWall()
    {
        if (shopUI != null)
        {
            shopUI.interactable = false;
            shopUI.blocksRaycasts = false;
        }

        if (fourthWall != null)
        {
            fourthWall.DOMoveY(wallStartPos.y, transitionTime);
        }

        if (mainCamera != null)
        {
            mainCamera.DOOrthoSize(originalCamSize, transitionTime).OnComplete(() => RestorePlayerState());
        }
    }

    private void RestorePlayerState()
    {
        if (playerRb != null)
        {
            playerRb.bodyType = RigidbodyType2D.Dynamic;
        }
        
        if (playerMove != null)
        {
            playerMove.SetTavernState(false);
            playerMove.enabled = true;
        }

        if (playerShoot != null)
        {
            playerShoot.enabled = true;
        }
        
        if (GameEvents.OnTavernExited != null)
        {
            GameEvents.OnTavernExited.Invoke();
        }
    }
}