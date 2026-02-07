using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HardScriptMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float acceleration = 10f;
    public float deceleration = 15f;
    public Animator anim;
    public SpriteRenderer spriteRenderer;

    private Rigidbody2D _rb;
    private Vector2 _input;
    private Vector2 _currentVelocity;
    private bool _isMoving;
    private bool _isDashing;

    void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        _input = new Vector2(
            Input.GetAxisRaw("Horizontal"),
            Input.GetAxisRaw("Vertical")
        ).normalized;

        if (_input.x < 0.01f) FlipSprite(true);
        else if (_input.x > -0.01f) FlipSprite(false);

        UpdateAnimatorState();
    }

    void FixedUpdate()
    {
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

    void FlipSprite(bool flipX)
    {
        if (spriteRenderer == null) return;
        spriteRenderer.flipX = flipX;
    }
}
