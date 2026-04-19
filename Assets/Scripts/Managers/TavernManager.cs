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
        originalCamSize = mainCamera.orthographicSize;
        wallStartPos = fourthWall.position;
        
        shopUI.alpha = 0;
        shopUI.interactable = false;
        shopUI.blocksRaycasts = false;
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
        playerMove = player.GetComponent<PlayerMovement>();
        playerShoot = player.GetComponent<ShootingController>();
        playerRb = player.GetComponent<Rigidbody2D>();

        continueButton.transform.DOScale(1.05f, 0.6f).SetLoops(-1, LoopType.Yoyo).SetUpdate(true);
        
        playerMove.SetTavernState(true);

        playerMove.enabled = false;
        playerShoot.enabled = false;
        playerRb.linearVelocity = Vector2.zero;
        playerRb.angularVelocity = 0f;
        playerRb.bodyType = RigidbodyType2D.Kinematic;

        player.transform.DOMove(playerAnchor.position, 1f).SetEase(Ease.OutQuad);
        player.transform.DORotate(Vector3.zero, 1f).SetEase(Ease.OutQuad);

        fourthWall.DOMoveY(wallStartPos.y + wallMoveY, transitionTime);
        mainCamera.DOOrthoSize(zoomAmount, transitionTime).OnComplete(ShowShopUI);
    }

    private void ShowShopUI()
    {
        shopUI.DOFade(1, 0.5f);
        shopUI.interactable = true;
        shopUI.blocksRaycasts = true;
    }

    public void ResumeGame()
    {
        continueButton.transform.DOComplete();
        continueButton.transform.DOPunchScale(Vector3.one * 0.2f, 0.3f).SetUpdate(true);
        AudioManager.Instance.PlayBuySound();
        
        shopUI.DOFade(0, 0.3f).OnComplete(HideShopAndMoveWall);
    }

    // Bloquea los botones y devuelve la pared y la cámara a su sitio original
    private void HideShopAndMoveWall()
    {
        shopUI.interactable = false;
        shopUI.blocksRaycasts = false;

        fourthWall.DOMoveY(wallStartPos.y, transitionTime);
        mainCamera.DOOrthoSize(originalCamSize, transitionTime).OnComplete(RestorePlayerState);
    }

    // Devuelve el control al jugador
    private void RestorePlayerState()
    {
        playerRb.bodyType = RigidbodyType2D.Dynamic;
        
        playerMove.SetTavernState(false);
        
        playerMove.enabled = true;
        playerShoot.enabled = true;
        
        GameEvents.OnTavernExited.Invoke();
    }
}