using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float acceleration = 10f;
    public float deceleration = 15f;
    public Animator anim;
    public GameObject visualObject;

    public float dashDistance = 3f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 1f;

    private bool _canDash = true;
    private Rigidbody2D _rb;
    private Vector2 _input;
    private Vector2 _currentVelocity;
    private bool _isMoving;
    private bool _isDashing;
    private bool _invincible;
    public PlayerCombat combat;
    private bool _movementLocked = false;

    void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (_movementLocked || combat.isAttacking) return;
        _input = new Vector2(
            Input.GetAxisRaw("Horizontal"),
            Input.GetAxisRaw("Vertical")
        ).normalized;

        if (_input.x > 0.01f) SetVisualFacing(-1f);
        else if (_input.x < -0.01f) SetVisualFacing(1f);

        if (Input.GetKeyDown(KeyCode.LeftShift) && _canDash)
        {
            StartCoroutine(Dash());
        }

        UpdateAnimatorState();
    }

    void FixedUpdate()
    {
        if (_isDashing || _movementLocked || combat.isAttacking) return;

        Vector2 targetVelocity = _input * moveSpeed;
        float smooth = _input.magnitude > 0 ? acceleration : deceleration;

        _currentVelocity = Vector2.Lerp(
            _rb.velocity,
            targetVelocity,
            smooth * Time.fixedDeltaTime
        );

        _rb.velocity = _currentVelocity;
    }

    void UpdateAnimatorState()
    {
        bool movingNow = _input.sqrMagnitude > 0.001f;

        if (_isDashing)
        {
            if (_isMoving)
            {
                _isMoving = false;
                if (anim != null) anim.SetTrigger("Idle");
            }
            return;
        }

        if (movingNow != _isMoving)
        {
            _isMoving = movingNow;
            if (anim != null) anim.SetTrigger(_isMoving ? "Move" : "Idle");
        }
    }

    void SetVisualFacing(float xScale)
    {
        if (visualObject == null) return;

        Vector3 s = visualObject.transform.localScale;
        s.x = xScale;
        visualObject.transform.localScale = s;
    }

    IEnumerator Dash()
    {
        _canDash = false;
        _isDashing = true;
        _invincible = true;

        if (anim != null) anim.SetTrigger("Idle");

        Vector2 dashDirection = _input;
        if (dashDirection == Vector2.zero) dashDirection = Vector2.up;

        dashDirection.Normalize();

        // Calculate dash speed from distance + duration
        float dashSpeed = dashDistance / dashDuration;

        // Apply velocity
        _rb.velocity = dashDirection * dashSpeed;

        float elapsed = 0f;

        while (elapsed < dashDuration)
        {
            elapsed += Time.deltaTime;
            yield return null;
        }

        // Stop movement
        _rb.velocity = Vector2.zero;

        _isDashing = false;
        _invincible = false;


        UpdateAnimatorState();

        yield return new WaitForSeconds(dashCooldown);
        _canDash = true;
    }

    public bool IsInvincible()
    {
        return _invincible;
    }
    public void LockMovement()
    {
        _movementLocked = true;
        _rb.velocity = Vector2.zero;
        _input = Vector2.zero;
    }

    public void UnlockMovement()
    {
        _movementLocked = false;
    }
}
