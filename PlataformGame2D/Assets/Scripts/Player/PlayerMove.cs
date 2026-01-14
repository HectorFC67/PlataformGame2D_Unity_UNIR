using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class PlayerMove : MonoBehaviour
{
    [Header("Input (Input System)")]
    public InputActionReference moveAction;
    public InputActionReference dashAction;

    [Header("Movement")]
    public float moveSpeed = 8f;
    public float jumpForce = 14f;

    [Header("Crouch")]
    public float crouchSpeedMultiplier = 0.5f;

    [Header("Ground Check (BoxCast)")]
    [Tooltip("Capas que cuentan como suelo (por ejemplo, 'Ground' donde está Midground).")]
    public LayerMask groundLayer;

    [Tooltip("Distancia extra hacia abajo para detectar suelo debajo del collider.")]
    public float groundCheckDistance = 0.08f;

    [Tooltip("Encoge un poco el ancho del cast para evitar detectar paredes laterales como suelo.")]
    [Range(0.5f, 1f)] public float groundCheckWidthFactor = 0.9f;

    [Header("Jump Feel")]
    [Tooltip("Permite saltar un poco después de haber dejado el suelo.")]
    public float coyoteTime = 0.1f;

    [Tooltip("Si pulsas salto un poco antes de tocar suelo, lo ejecuta al aterrizar.")]
    public float jumpBufferTime = 0.1f;

    [Header("Dash")]
    public float dashSpeed = 18f;
    public float dashDuration = 0.15f;
    public float dashCooldown = 5f;

    private Rigidbody2D rb;
    private Collider2D col;

    private Vector2 moveInput;
    private bool isCrouching;

    private bool isDashing;
    private float nextDashTime;

    // Timers para jump feel
    private float coyoteTimer;
    private float jumpBufferTimer;

    // Para evitar salto continuo si mantienes W dentro del Move
    private bool jumpLatch;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
    }

    private void OnEnable()
    {
        if (moveAction != null) moveAction.action.Enable();
        if (dashAction != null)
        {
            dashAction.action.Enable();
            dashAction.action.performed += OnDashPerformed;
        }
    }

    private void OnDisable()
    {
        if (dashAction != null)
            dashAction.action.performed -= OnDashPerformed;

        if (moveAction != null) moveAction.action.Disable();
        if (dashAction != null) dashAction.action.Disable();
    }

    private void Update()
    {
        moveInput = moveAction != null ? moveAction.action.ReadValue<Vector2>() : Vector2.zero;

        isCrouching = moveInput.y < -0.5f;

        bool groundedNow = IsGrounded_BoxCast();

        // Coyote time
        if (groundedNow) coyoteTimer = coyoteTime;
        else coyoteTimer -= Time.deltaTime;

        // Jump input (W en tu Move)
        bool wantsJump = moveInput.y > 0.5f;

        // latch para que mantener W no meta inputs infinitos
        if (!wantsJump) jumpLatch = false;

        if (wantsJump && !jumpLatch)
        {
            jumpBufferTimer = jumpBufferTime; // registramos “intentó saltar”
            jumpLatch = true;
        }
        else
        {
            jumpBufferTimer -= Time.deltaTime;
        }

        // Ejecutar salto si:
        // - no estás dashing
        // - no estás agachado
        // - hay jump buffered
        // - estás dentro de coyote time (suelo reciente)
        if (!isDashing && !isCrouching && jumpBufferTimer > 0f && coyoteTimer > 0f)
        {
            Jump();
            jumpBufferTimer = 0f;
            coyoteTimer = 0f; // consume coyote
        }
    }

    private void FixedUpdate()
    {
        if (isDashing) return;

        float speed = moveSpeed * (isCrouching ? crouchSpeedMultiplier : 1f);
        rb.linearVelocity = new Vector2(moveInput.x * speed, rb.linearVelocity.y);
    }

    private void Jump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
    }

    private bool IsGrounded_BoxCast()
    {
        // Bounds del collider del jugador
        Bounds b = col.bounds;

        // Caja un poco más estrecha que el collider, y muy bajita
        float castWidth = b.size.x * groundCheckWidthFactor;
        Vector2 castSize = new Vector2(castWidth, 0.02f);

        // Origen: justo debajo del collider
        Vector2 castOrigin = new Vector2(b.center.x, b.min.y);

        RaycastHit2D hit = Physics2D.BoxCast(
            castOrigin,
            castSize,
            0f,
            Vector2.down,
            groundCheckDistance,
            groundLayer
        );

        return hit.collider != null;
    }

    private void OnDashPerformed(InputAction.CallbackContext ctx)
    {
        if (Time.time < nextDashTime) return;

        float dir = Mathf.Sign(moveInput.x);
        if (Mathf.Abs(moveInput.x) < 0.01f)
        {
            dir = Mathf.Sign(rb.linearVelocity.x);
            if (Mathf.Abs(rb.linearVelocity.x) < 0.01f) dir = 1f;
        }

        StartCoroutine(DashCoroutine(dir));
    }

    private IEnumerator DashCoroutine(float dir)
    {
        isDashing = true;
        nextDashTime = Time.time + dashCooldown;

        float startTime = Time.time;
        while (Time.time < startTime + dashDuration)
        {
            rb.linearVelocity = new Vector2(dir * dashSpeed, rb.linearVelocity.y);
            yield return null;
        }

        isDashing = false;
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        // Visualizar el BoxCast de suelo en editor
        var c = GetComponent<Collider2D>();
        if (c == null) return;

        Bounds b = c.bounds;
        float castWidth = b.size.x * groundCheckWidthFactor;
        Vector2 castSize = new Vector2(castWidth, 0.02f);
        Vector2 castOrigin = new Vector2(b.center.x, b.min.y);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(castOrigin + Vector2.down * groundCheckDistance, castSize);
    }
#endif
}
