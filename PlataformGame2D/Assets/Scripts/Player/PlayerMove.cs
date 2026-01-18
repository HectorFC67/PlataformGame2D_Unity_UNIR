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
    public InputActionReference attackAction;

    [Header("Movement")]
    public float moveSpeed = 8f;
    public float jumpForce = 14f;

    [Header("Ground Check (BoxCast)")]
    public LayerMask groundLayer;
    public float groundCheckDistance = 0.08f;
    [Range(0.5f, 1f)] public float groundCheckWidthFactor = 0.9f;

    [Header("Jump Feel")]
    public float coyoteTime = 0.1f;
    public float jumpBufferTime = 0.1f;

    [Header("Dash")]
    public float dashSpeed = 18f;
    public float dashDuration = 0.15f;
    public float dashCooldown = 5f;

    [Header("Attack")]
    public float attackDuration = 0.35f;

    [Header("Animator")]
    public Animator animator;

    private Rigidbody2D rb;
    private BoxCollider2D boxCol;

    private Vector2 moveInput;

    private bool isDashing;
    private float nextDashTime;

    private bool isAttacking;

    private float coyoteTimer;
    private float jumpBufferTimer;

    private bool jumpLatch;

    private bool groundedNow;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        boxCol = GetComponent<BoxCollider2D>();

        if (animator == null)
            animator = GetComponentInChildren<Animator>();
    }

    private void OnEnable()
    {
        if (moveAction != null) moveAction.action.Enable();

        if (dashAction != null)
        {
            dashAction.action.Enable();
            dashAction.action.performed += OnDashPerformed;
        }

        if (attackAction != null)
        {
            attackAction.action.Enable();
            attackAction.action.performed += OnAttackPerformed;
        }
    }

    private void OnDisable()
    {
        if (dashAction != null)
            dashAction.action.performed -= OnDashPerformed;

        if (attackAction != null)
        attackAction.action.performed -= OnAttackPerformed;

        if (moveAction != null) moveAction.action.Disable();
        if (dashAction != null) dashAction.action.Disable();
        if (attackAction != null) attackAction.action.Disable();
    }

    private void Update()
    {
        moveInput = moveAction != null ? moveAction.action.ReadValue<Vector2>() : Vector2.zero;

        groundedNow = IsGrounded_BoxCast();

        if (groundedNow) coyoteTimer = coyoteTime;
        else coyoteTimer -= Time.deltaTime;

        bool wantsJump = moveInput.y > 0.5f;

        if (!wantsJump) jumpLatch = false;

        if (wantsJump && !jumpLatch)
        {
            jumpBufferTimer = jumpBufferTime;
            jumpLatch = true;
        }
        else
        {
            jumpBufferTimer -= Time.deltaTime;
        }

        if (!isDashing && !isAttacking && jumpBufferTimer > 0f && coyoteTimer > 0f)
        {
            Jump();
            jumpBufferTimer = 0f;
            coyoteTimer = 0f;
        }

        if (attackAction == null && Keyboard.current != null)
        {
            TryAttack();
        }

        UpdateAnimator();
        HandleFlip(moveInput.x);
    }

    private void FixedUpdate()
    {
        if (isDashing) return;
        if (isAttacking) return;

        rb.linearVelocity = new Vector2(moveInput.x * moveSpeed, rb.linearVelocity.y);
    }

    private void Jump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
    }

    private bool IsGrounded_BoxCast()
    {
        Bounds b = boxCol.bounds;

        float castWidth = b.size.x * groundCheckWidthFactor;
        Vector2 castSize = new Vector2(castWidth, 0.02f);

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
        if (isAttacking) return;

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

    private void OnAttackPerformed(InputAction.CallbackContext ctx)
    {
        TryAttack();
    }

    private void TryAttack()
    {
        if (isAttacking) return;
        if (isDashing) return;
        if (!groundedNow) return;

        StartCoroutine(AttackCoroutine());
    }

    private IEnumerator AttackCoroutine()
    {
        isAttacking = true;

        rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);

        if (animator != null)
            animator.SetBool("Atacando", true);

        yield return new WaitForSeconds(attackDuration);

        if (animator != null)
            animator.SetBool("Atacando", false);

        isAttacking = false;
    }

    private void UpdateAnimator()
    {
        if (animator == null) return;

        float moveAmount = Mathf.Abs(moveInput.x);

        animator.SetFloat("movement", moveAmount);
        animator.SetBool("ensuelo", groundedNow);
        animator.SetBool("Atacando", isAttacking);
    }

    private void HandleFlip(float xInput)
    {
        if (Mathf.Abs(xInput) < 0.01f) return;

        Vector3 s = transform.localScale;
        s.x = xInput > 0 ? Mathf.Abs(s.x) : -Mathf.Abs(s.x);
        transform.localScale = s;
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        var c = GetComponent<BoxCollider2D>();
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