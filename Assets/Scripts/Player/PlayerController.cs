using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    public static PlayerController Instance { get; private set; }

    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private float jumpForce = 10f;

    private Rigidbody2D rb;
    private Vector2 movement;

    private void Awake() {
        Instance = this;
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start() {
        GameInput.Instance.OnJumpAction += GameInput_OnJumpAction;
    }

    // EVENT Listeners
    private void GameInput_OnJumpAction(object sender, System.EventArgs e) {
        HandleJump();
    }

    private void Update() {
        HandleMovementInput();
    }

    private void FixedUpdate() {
        HandleMovement();
    }

    private void HandleMovementInput() {
        movement = GameInput.Instance.GetMovementNormalize();
    }

    private void HandleMovement() {
        rb.velocity = new Vector2(movement.x * moveSpeed, rb.velocity.y);
    }

    private void HandleJump() {
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
    }
}
