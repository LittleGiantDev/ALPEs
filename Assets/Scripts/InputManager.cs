using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }

    private PlayerInput playerInput;

    public event Action OnJumpInitiated;
    public event Action OnJumpCanceled;
    public event Action OnRightClickInitiated;
    public event Action OnRightClickCanceled;
    public event Action OnLeftClickInitiated;
    public event Action OnLeftClickCanceled;
    public event Action OnReloadInitiated;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        playerInput = new PlayerInput();

        playerInput.Player.Jump.started += ctx => OnJumpInitiated?.Invoke();
        playerInput.Player.Jump.canceled += ctx => OnJumpCanceled?.Invoke();
        
        playerInput.Player.RightClick.started += ctx => OnRightClickInitiated?.Invoke();
        playerInput.Player.RightClick.canceled += ctx => OnRightClickCanceled?.Invoke();
        
        playerInput.Player.LeftClick.started += ctx => OnLeftClickInitiated?.Invoke();
        playerInput.Player.LeftClick.canceled += ctx => OnLeftClickCanceled?.Invoke();
        
        playerInput.Player.Reload.started += ctx => OnReloadInitiated?.Invoke();
    }

    private void OnEnable() => playerInput?.Player.Enable();
    private void OnDisable() => playerInput?.Player.Disable();
}