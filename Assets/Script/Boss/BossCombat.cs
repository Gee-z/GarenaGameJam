using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossCombat : MonoBehaviour
{
    public WeaponHitbox weapon;

    public int attack1Damage = 10;
    public int attack2Damage = 10;
    public int attack3Damage = 20;

    public float attackDuration = 0.4f;  // per hit duration
    public float comboResetTime = 0.6f;  // gap between hits

    [Header("Forward Movement Per Hit")]
    public float attack1Forward = 0.5f;
    public float attack2Forward = 0.7f;
    public float attack3Forward = 1f;

    public Transform player;

    private enum State { Idle, Attack1, Attack2, Attack3 }
    private State currentState = State.Idle;
    public void StartMeleeCombo()
    {
        if (currentState == State.Idle)
        {
            StartCoroutine(DoCombo());
        }
    }

    private IEnumerator DoCombo()
    {
        Vector3 originalPos = transform.position;

        // Attack 1
        currentState = State.Attack1;
        weapon.SetDamage(attack1Damage);
        weapon.EnableHit();
        MoveForward(attack1Forward);
        yield return new WaitForSeconds(attackDuration);
        weapon.DisableHit();
        transform.position = originalPos; // return
        yield return new WaitForSeconds(comboResetTime);

        // Attack 2
        currentState = State.Attack2;
        weapon.SetDamage(attack2Damage);
        weapon.EnableHit();
        MoveForward(attack2Forward);
        yield return new WaitForSeconds(attackDuration);
        weapon.DisableHit();
        transform.position = originalPos;
        yield return new WaitForSeconds(comboResetTime);

        // Attack 3
        currentState = State.Attack3;
        weapon.SetDamage(attack3Damage);
        weapon.EnableHit();
        MoveForward(attack3Forward);
        yield return new WaitForSeconds(attackDuration);
        weapon.DisableHit();
        transform.position = originalPos;

        currentState = State.Idle;
    }

    private void MoveForward(float distance)
    {
        if (player == null) return;
        Vector3 dir = (player.position - transform.position).normalized;
        transform.position += dir * distance;
    }
}
