using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSideScroll : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 6f;
    public float acceleration = 50f;
    public Rigidbody2D rb;

    [Header("Combat")]
    public HammerBonk hammer;
    public Animator animator;

    float moveInput;
    bool facingRight = true;
    int attackIndex = 0;

    void Awake()
    {
        if (!rb) rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // Movement
        moveInput = Input.GetAxisRaw("Horizontal");

        if (moveInput > 0 && !facingRight) Flip();
        else if (moveInput < 0 && facingRight) Flip();

        // Attack
        if (Input.GetMouseButtonDown(0))
        {
            if (attackIndex == 0)
                animator.SetTrigger("Attack1");
            else
                animator.SetTrigger("Attack2");

            attackIndex = 1 - attackIndex;

            hammer.Bonk();
        }
    }

    void FixedUpdate()
    {
        float targetVelX = moveInput * moveSpeed;
        rb.velocity = new Vector2(
            Mathf.MoveTowards(rb.velocity.x, targetVelX, acceleration * Time.fixedDeltaTime),
            rb.velocity.y
        );
    }

    void Flip()
    {
        facingRight = !facingRight;
        Vector3 s = transform.localScale;
        s.x *= -1;
        transform.localScale = s;
    }
}
