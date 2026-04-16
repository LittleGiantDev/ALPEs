using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;

public class TavernManager : MonoBehaviour
{
    [Header("Visuals")]
    [SerializeField] private Transform fourthWall;
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

        playerMove.enabled = false;
        playerShoot.enabled = false;
        playerRb.linearVelocity = Vector2.zero;
        playerRb.angularVelocity = 0f;
        playerRb.bodyType = RigidbodyType2D.Kinematic;

        player.transform.DOMove(playerAnchor.position, 1f).SetEase(Ease.OutQuad);

        Sequence tavernSeq = DOTween.Sequence();
        tavernSeq.Join(fourthWall.DOMoveY(wallStartPos.y + wallMoveY, transitionTime));
        tavernSeq.Join(mainCamera.DOOrthoSize(zoomAmount, transitionTime));
        tavernSeq.OnComplete(() => {
            shopUI.DOFade(1, 0.5f);
            shopUI.interactable = true;
            shopUI.blocksRaycasts = true;
        });
    }

    public void ResumeGame()
    {
        shopUI.DOFade(0, 0.3f).OnComplete(() => {
            shopUI.interactable = false;
            shopUI.blocksRaycasts = false;

            Sequence exitSeq = DOTween.Sequence();
            exitSeq.Join(fourthWall.DOMoveY(wallStartPos.y, transitionTime));
            exitSeq.Join(mainCamera.DOOrthoSize(originalCamSize, transitionTime));
            exitSeq.OnComplete(() => {
                playerRb.bodyType = RigidbodyType2D.Dynamic;
                playerMove.enabled = true;
                playerShoot.enabled = true;
                GameEvents.OnTavernExited?.Invoke();
            });
        });
    }
}