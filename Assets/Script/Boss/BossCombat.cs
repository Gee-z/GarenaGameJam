using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossCombat : MonoBehaviour
{
    public WeaponHitbox weapon;       // For Attack1 & Attack2
    public WeaponHitbox lungeWeapon;  // For Attack3
    public Transform player;          // Player to target
    public float attackDuration = 0.4f;
    public float attack1Move = 0.3f;
    public float attack2Move = 0.4f;
    public float attack3Lunge = 1f;

    public int attack1Damage = 10;
    public int attack2Damage = 10;
    public int attack3Damage = 20;

    private enum State { Idle, Attack1, Attack2, Attack3 }
    private State currentState = State.Idle;

    private Coroutine moveCoroutine;

    public void StartCombo()
    {
        if (currentState == State.Idle)
            StartCoroutine(ComboRoutine());
    }

    private IEnumerator ComboRoutine()
    {
        // --- ATTACK 1 ---
        currentState = State.Attack1;
        Vector3 dir1 = (player.position - transform.position).normalized;
        ExecuteAttack(weapon, attack1Damage, attack1Move, dir1);
        yield return new WaitForSeconds(attackDuration); // pause between attacks

        // --- ATTACK 2 ---
        currentState = State.Attack2;
        Vector3 dir2 = (player.position - transform.position).normalized;
        ExecuteAttack(weapon, attack2Damage, attack2Move, dir2);
        yield return new WaitForSeconds(attackDuration);

        // --- LUNGE ATTACK ---
        currentState = State.Attack3;
        Vector3 dir3 = (player.position - transform.position).normalized;
        ExecuteAttack(lungeWeapon, attack3Damage, attack3Lunge, dir3, true); // lock rotation
        yield return new WaitForSeconds(attackDuration);

        currentState = State.Idle;
    }

    private void ExecuteAttack(WeaponHitbox activeWeapon, int damage, float moveDistance, Vector3 direction, bool lockRotation = false)
    {
        if (activeWeapon != null)
        {
            // Enable collider and optionally lock rotation
            activeWeapon.EnableHit(lockRotation, direction);
            StartCoroutine(DisableWeaponAfter(activeWeapon, attackDuration));
        }

        MoveForward(moveDistance, direction);
    }

    private IEnumerator DisableWeaponAfter(WeaponHitbox w, float time)
    {
        yield return new WaitForSeconds(time);
        if (w != null) w.DisableHit();
    }

    private void MoveForward(float distance, Vector3 direction)
    {
        if (moveCoroutine != null) StopCoroutine(moveCoroutine);
        moveCoroutine = StartCoroutine(MoveForwardCoroutine(distance, direction));
    }

    private IEnumerator MoveForwardCoroutine(float distance, Vector3 direction)
    {
        Vector3 startPos = transform.position;
        Vector3 targetPos = startPos + direction * distance;
        float elapsed = 0f;
        float duration = attackDuration / 2f;

        while (elapsed < duration)
        {
            transform.position = Vector3.Lerp(startPos, targetPos, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPos;
    }
}
