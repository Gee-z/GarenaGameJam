using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAI : MonoBehaviour
{
    public Transform player;
    public float moveSpeed = 1f;
    public BossCombat combat;   

    public GameObject bulletPrefab;
    public Transform bulletSpawn;
    public float bulletSpeed = 5f;
    public int bulletCount = 12;                
    public float bulletAngle = 60f;

    public GameObject minionPrefab;
    public Transform[] minionSpawnPoints; // assign 10 positions

    public float actionCooldown = 1.5f;
    public float meleeApproachTime = 0.4f;

    public int minSummon = 3;
    public int maxSummon = 5;

    private BossState currentState = BossState.Idle;
    private bool actionInProgress = false;
    private Rigidbody2D rb;
    public float introDelay = 2f;
    private bool canAct = false;
    IEnumerator Start()
    {
        yield return new WaitForSeconds(introDelay);
        canAct = true;
    }
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.freezeRotation = true;
    }
    void Update()
    {
        if (!player || !canAct) return;

        float dirX = player.position.x - transform.position.x;
        if (Mathf.Abs(dirX) > 0.01f)
            transform.localScale = new Vector3(Mathf.Sign(dirX), 1, 1);

        if (currentState == BossState.ChasePlayer || currentState == BossState.MeleeAttack)
        ChasePlayer();

        if (!actionInProgress && currentState == BossState.Idle)
            StartCoroutine(ChooseAction());
        }
    void ChasePlayer()
    {
        if (!player) return;

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (!rb) return;

        Vector2 dir = (player.position - transform.position).normalized;
        rb.velocity = dir * moveSpeed;
    }
    void StopMovement()
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb) rb.velocity = Vector2.zero;
    }
    IEnumerator ChooseAction()
    {
        actionInProgress = true;

        // Pick random action: 0=melee, 1=bullet, 2=summon
        int choice = Random.Range(0, 3);

        switch (choice)
        {
            case 0: 
                currentState = BossState.MeleeAttack;
                yield return StartCoroutine(MeleeAttack());
                break;

            case 1: 
                currentState = BossState.BulletHell;
                yield return StartCoroutine(DoBulletHell());
                break;

            case 2: 
                currentState = BossState.SummonMinions;
                yield return StartCoroutine(DoSummonMinions());
                break;
        }

        currentState = BossState.Idle;
        yield return new WaitForSeconds(actionCooldown);
        actionInProgress = false;
    }

    IEnumerator MeleeAttack()
    {
        float timer = 0f;

        while (timer < meleeApproachTime)
        {
            Vector2 dir = (player.position - transform.position).normalized;
            rb.velocity = dir * moveSpeed;
            timer += Time.deltaTime;
            yield return null;
        }
        rb.velocity = Vector2.zero;
        combat.StartCombo();
        while (combat.isAttacking)
            yield return null;
    }
    IEnumerator DoBulletHell()
    {
        int[] possibleCounts = { 1, 3, 5 };
        int count = possibleCounts[Random.Range(0, possibleCounts.Length)];

        Vector2 toPlayer = (player.position - bulletSpawn.position).normalized;
        float baseAngle = Mathf.Atan2(toPlayer.y, toPlayer.x) * Mathf.Rad2Deg;

        float spread = 40f;
        float angleStep = count > 1 ? spread / (count - 1) : 0f;
        float startAngle = baseAngle - spread / 2f;

        for (int i = 0; i < count; i++)
        {
            float angle = startAngle + angleStep * i;
            Vector2 dir = Quaternion.Euler(0, 0, angle) * Vector2.right;

            GameObject bullet = Instantiate(bulletPrefab, bulletSpawn.position, Quaternion.identity);
            bullet.GetComponent<Rigidbody2D>().velocity = dir * bulletSpeed;
        }

        yield return new WaitForSeconds(0.6f);
    }

    IEnumerator DoSummonMinions()
    {
        int count = Random.Range(minSummon, maxSummon + 1);
        List<Transform> spawnPool = new List<Transform>(minionSpawnPoints);

        for (int i = 0; i < count; i++)
        {
            if (spawnPool.Count == 0) break;
            int index = Random.Range(0, spawnPool.Count);
            Transform spawnPoint = spawnPool[index];
            spawnPool.RemoveAt(index);

            Instantiate(minionPrefab, spawnPoint.position, Quaternion.identity);
        }

        yield return new WaitForSeconds(0.5f);
    }
}
