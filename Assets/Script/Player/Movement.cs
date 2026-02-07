using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float acceleration = 10f;
    public float deceleration = 15f;

    public float dashDistance = 3f;      
    public float dashDuration = 0.2f;    
    public float dashCooldown = 1f;       
    private bool canDash = true;
    private Rigidbody2D rb;
    private Vector2 input;
    private Vector2 currentVelocity;
    private bool isDashing = false;

    // I-frame
    private bool invincible = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        input = new Vector2(
            Input.GetAxisRaw("Horizontal"),
            Input.GetAxisRaw("Vertical")
        ).normalized;

        if (Input.GetKeyDown(KeyCode.LeftShift) && canDash)
        {
            StartCoroutine(Dash());
        }
    }

    void FixedUpdate()
    {
        Vector2 targetVelocity = input * moveSpeed;

        float smooth = input.magnitude > 0 ? acceleration : deceleration;

        currentVelocity = Vector2.Lerp(
            rb.velocity,
            targetVelocity,
            smooth * Time.fixedDeltaTime
        );

        rb.velocity = currentVelocity;
    }
    IEnumerator Dash()
    {
        canDash = false;
        isDashing = true;
        invincible = true; // I-frame

        Vector2 dashDirection = input;
        if (dashDirection == Vector2.zero)
            dashDirection = Vector2.up; // default forward if no input

        Vector2 startPos = rb.position;
        Vector2 targetPos = startPos + dashDirection.normalized * dashDistance;

        float elapsed = 0f;

        while (elapsed < dashDuration)
        {
            rb.position = Vector2.Lerp(startPos, targetPos, elapsed / dashDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        rb.position = targetPos;

        isDashing = false;
        invincible = false;

        // Start cooldown
        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }
    public bool IsInvincible()
    {
        return invincible;
    }
}
