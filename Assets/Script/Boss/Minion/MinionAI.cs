using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinionAI : MonoBehaviour
{
    public float moveSpeed = 1.5f;
    public float changeDirTime = 2f;

    public GameObject bulletPrefab;
    public Transform firePoint;
    public float bulletSpeed = 4f;
    public float shootCooldown = 2f;
    public Animator animator;
    private Vector2 moveDir;
    private Transform player;
    private Rigidbody2D rb;
    private bool touchingPlayer;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.freezeRotation = true;
    }

    void Start()
    {
        rb.velocity = Vector2.zero;
        rb.simulated = false;   // temporarily disable physics
        StartCoroutine(EnablePhysicsNextFrame());
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        StartCoroutine(ChangeDirectionRoutine());
        StartCoroutine(ShootRoutine());
    }

    void Update()
    {
        UpdateAnimatorState();
    }

    IEnumerator ChangeDirectionRoutine()
    {
        while (true)
        {
            moveDir = Random.insideUnitCircle.normalized;
            yield return new WaitForSeconds(changeDirTime);
        }
    }

    IEnumerator ShootRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(shootCooldown);

            if (!player) continue;

            Vector2 dir = (player.position - firePoint.position).normalized;
            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
            bullet.GetComponent<Rigidbody2D>().velocity = dir * bulletSpeed;
        }
    }
    void FixedUpdate()
    {
        if (touchingPlayer)
        {
            rb.velocity = Vector2.zero;
            return;
        }

        animator.SetTrigger("Move");
        rb.velocity = moveDir * moveSpeed;

        if (rb.velocity.x != 0)
            transform.localScale = new Vector3(Mathf.Sign(rb.velocity.x), 1, 1);
    }
    void UpdateAnimatorState()
    {
        bool movingNow = rb.velocity.sqrMagnitude > 0.001f;

        if (movingNow)
        {
            animator.SetTrigger("Move");
        }
        else
        {
            animator.SetTrigger("Idle");
        }
    }
    IEnumerator EnablePhysicsNextFrame()
    {
        yield return new WaitForFixedUpdate();
        rb.simulated = true;
    }
}
