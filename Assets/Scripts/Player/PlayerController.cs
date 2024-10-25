using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    public static PlayerController Instance { get; private set; }

    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private float jumpForce = 10f;
    [SerializeField] private LayerMask groundLayerMask;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.2f;

    private Rigidbody2D rb;
    private Vector2 movement;
    private bool isGrounded;
    private bool isJumping;

    private void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(gameObject);
        } else {
            Instance = this;
        }

        rb = GetComponent<Rigidbody2D>();
    }

    private void Start() {
        GameInput.Instance.OnJumpAction += GameInput_OnJumpAction;
        GameInput.Instance.OnJumpCanceled += GameInput_OnJumpCanceled;
    }

    // EVENT Listeners
    private void GameInput_OnJumpAction(object sender, System.EventArgs e) {
        isJumping = true;
    }

    private void GameInput_OnJumpCanceled(object sender, System.EventArgs e) {
        isJumping = false;
    }

    private void Update() {
        HandleMovementInput();
        CheckGround();
    }

    private void FixedUpdate() {
        if (!DialogueManager.Instance.IsDialoguePlaying()) {
            HandleMovement();

            if (isJumping && isGrounded) {
                HandleJump();
            }
        }
    }

    private void HandleMovementInput() {
        movement = GameInput.Instance.GetMovementNormalize();
    }

    private void HandleMovement() {
        if (isGrounded || movement.x != 0) {
            rb.velocity = new Vector2(movement.x * moveSpeed, rb.velocity.y);
        }
    }

    private void HandleJump() {
        if (isGrounded) {
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }
    }

    private void CheckGround() {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayerMask);
    }
}
