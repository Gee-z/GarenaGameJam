using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAI : MonoBehaviour
{
    public Transform player;
    public float moveSpeed = 3f;
    public BossCombat combat;   

    public GameObject bulletPrefab;
    public Transform bulletSpawn;

    public GameObject minionPrefab;
    public Transform[] minionSpawnPoints; // assign 10 positions

    public float actionPauseTime = 1.5f;

    public int minSummon = 3;
    public int maxSummon = 5;

    private BossState currentState = BossState.Idle;
    void Update()
    {
        switch (currentState)
        {
            case BossState.Idle: ChooseNextAction(); break;
            case BossState.ChasePlayer: ChasePlayer(); break;
        }
    }

    void ChooseNextAction()
    {
        int action = Random.Range(0, 3);
        switch(action)
        {
            case 0: StartCoroutine(DoMeleeAttack()); break;
            case 1: StartCoroutine(DoBulletHell()); break;
            case 2: StartCoroutine(DoSummonMinions()); break;
        }
    }

    void ChasePlayer()
    {
        if (player == null) return;
        Vector3 dir = (player.position - transform.position).normalized;
        transform.position += dir * moveSpeed * Time.deltaTime;
    }

    IEnumerator DoMeleeAttack()
    {
        currentState = BossState.MeleeAttack;
        if (combat != null)
            combat.StartMeleeCombo();
        float totalTime = (combat.attackDuration + combat.comboResetTime) * 3f;
        yield return new WaitForSeconds(totalTime);

        currentState = BossState.Tired;
        yield return new WaitForSeconds(actionPauseTime);
        currentState = BossState.Idle;
    }

    IEnumerator DoBulletHell()
    {
        currentState = BossState.BulletHell;

        for (int i = -2; i <= 2; i++)
        {
            GameObject b = Instantiate(bulletPrefab, bulletSpawn.position, Quaternion.identity);
            b.GetComponent<Rigidbody2D>().velocity = new Vector2(i, 5f);
        }

        currentState = BossState.Tired;
        yield return new WaitForSeconds(actionPauseTime);
        currentState = BossState.Idle;
    }

    IEnumerator DoSummonMinions()
    {
        currentState = BossState.SummonMinions;

        if (minionSpawnPoints.Length == 0) yield break;

        int spawnCount = Random.Range(minSummon, maxSummon + 1);
        List<int> availableIndices = new List<int>();
        for (int i = 0; i < minionSpawnPoints.Length; i++) availableIndices.Add(i);

        for (int i = 0; i < spawnCount; i++)
        {
            if (availableIndices.Count == 0) break;

            int randIndex = Random.Range(0, availableIndices.Count);
            int spawnIndex = availableIndices[randIndex];
            availableIndices.RemoveAt(randIndex);

            Instantiate(minionPrefab, minionSpawnPoints[spawnIndex].position, Quaternion.identity);
        }

        currentState = BossState.Tired;
        yield return new WaitForSeconds(actionPauseTime);
        currentState = BossState.Idle;
    }
}
