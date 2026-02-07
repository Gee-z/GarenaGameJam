using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float acceleration = 10f;
    public float deceleration = 15f;

    private Rigidbody2D rb;
    private Vector2 input;
    private Vector2 currentVelocity;

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
}
