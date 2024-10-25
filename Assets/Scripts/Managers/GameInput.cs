using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameInput : MonoBehaviour {

    public static GameInput Instance { get; private set; }

    public event EventHandler OnJumpAction;
    public event EventHandler OnJumpCanceled;
    public event EventHandler OnInteractAction;
    public event EventHandler OnContinueAction;

    private PlayerInputActions playerInputActions;

    private void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(gameObject);
        } else {
            Instance = this;
        }

        playerInputActions = new PlayerInputActions();
    }

    private void OnEnable() {
        playerInputActions.Enable();
        playerInputActions.Player.Jump.performed += Jump_performed;
        playerInputActions.Player.Jump.canceled += Jump_canceled;
        playerInputActions.Player.Interact.performed += Interact_performed;
        playerInputActions.Player.Continue.performed += Continue_performed;
    }


    public Vector2 GetMovementNormalize() {
        Vector2 inputVector = playerInputActions.Player.Movement.ReadValue<Vector2>();
        inputVector.Normalize();
        return inputVector;
    }

    private void Jump_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj) {
        OnJumpAction?.Invoke(this, EventArgs.Empty);
    }

    private void Jump_canceled(UnityEngine.InputSystem.InputAction.CallbackContext obj) {
        OnJumpCanceled?.Invoke(this, EventArgs.Empty);
    }

    private void Interact_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj) {
        OnInteractAction?.Invoke(this, EventArgs.Empty);
    }

    private void Continue_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj) {
        OnContinueAction?.Invoke(this, EventArgs.Empty);
    }
}
