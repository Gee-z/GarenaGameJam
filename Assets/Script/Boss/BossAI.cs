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

    public int minSummon = 3;
    public int maxSummon = 5;

    private BossState currentState = BossState.Idle;
    private bool actionInProgress = false;
    void Update()
    {
        if (currentState == BossState.ChasePlayer || currentState == BossState.MeleeAttack)
        {
            ChasePlayer();
        }
        if (!actionInProgress)
        {
            StartCoroutine(ChooseAction());
        }
    }
    void ChasePlayer()
    {
        if (player == null) return;
        Vector3 dir = (player.position - transform.position).normalized;
        transform.position += dir * moveSpeed * Time.deltaTime;
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
        while (Vector3.Distance(transform.position, player.position) > 1f)
        {
            ChasePlayer();
            yield return null;
        }

        combat.StartCombo();
        yield return new WaitForSeconds(combat.attackDuration * 3f);
    }

    IEnumerator DoBulletHell()
    {
        Vector3 toPlayer = (player.position - bulletSpawn.position).normalized;
        float baseAngle = Mathf.Atan2(toPlayer.y, toPlayer.x) * Mathf.Rad2Deg;

        float angleStep = bulletAngle / (bulletCount - 1);
        float startAngle = baseAngle - bulletAngle / 2f;

        for (int i = 0; i < bulletCount; i++)
        {
            float angle = startAngle + angleStep * i;
            Vector3 dir = Quaternion.Euler(0, 0, angle) * Vector3.right;

            GameObject bullet = Instantiate(bulletPrefab, bulletSpawn.position, Quaternion.identity);
            bullet.GetComponent<Rigidbody2D>().velocity = dir.normalized * bulletSpeed;
        }

        yield return new WaitForSeconds(0.5f);
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
