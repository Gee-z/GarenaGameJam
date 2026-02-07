using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossCombat : MonoBehaviour
{
    public WeaponHitbox weapon;       // For Attack1 & Attack2
    public WeaponHitbox lungeWeapon;  // For Attack3
    public Transform combatPivot;
    public Transform player;          // Player to target
    public float comboResetTime = 0.6f;
    public float attackDuration1 = 0.4f;
    public float attackDuration2 = 0.4f;
    public float attackDuration3 = 0.7f;
    public float comboWindow = 0.3f;

    public int attack1Damage = 10;
    public int attack2Damage = 10;
    public int attack3Damage = 20;

    public float attack1Move = 0.3f;
    public float attack2Move = 0.4f;
    public float attack3Lunge = 1f;

    public Animator anim;

    private enum State { Idle, Attack1, Attack2, Attack3 }
    private State currentState = State.Idle;
     private Vector2 lockedAttackDirection;
    private Rigidbody2D rb;
    private Coroutine moveRoutine;

    public bool isAttacking { get; private set; }

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.freezeRotation = true;
    }

    void Update()
    {
        if (currentState != State.Idle)
        {
            RotateCombatPivot(lockedAttackDirection);
        }
    }


    public void StartCombo()
    {
        if (currentState != State.Idle) return;

 
        lockedAttackDirection = (player.position - transform.position).normalized;

        if (lockedAttackDirection.x >= 0)
            transform.localScale = new Vector3(1, 1, 1);
        else
            transform.localScale = new Vector3(-1, 1, 1);

        StartCoroutine(ComboRoutine());
    }


    IEnumerator ComboRoutine()
    {
        isAttacking = true;

        yield return DoAttack(State.Attack1);
        yield return DoAttack(State.Attack2);
        yield return DoAttack(State.Attack3);

        ResetCombo();
    }

    IEnumerator DoAttack(State attackState)
    {
        currentState = attackState;

        WeaponHitbox activeWeapon;
        float duration;
        float moveDist;
        int damage;

        switch (attackState)
        {
            case State.Attack1:
                activeWeapon = weapon;
                duration = attackDuration1;
                moveDist = attack1Move;
                damage = attack1Damage;
                anim.SetTrigger("Attack1");
                break;

            case State.Attack2:
                activeWeapon = weapon;
                duration = attackDuration2;
                moveDist = attack2Move;
                damage = attack2Damage;
                anim.SetTrigger("Attack2");
                break;

            case State.Attack3:
                activeWeapon = lungeWeapon;
                duration = attackDuration3;
                moveDist = attack3Lunge;
                damage = attack3Damage;
                anim.SetTrigger("Attack3");
                break;

            default:
                yield break;
        }

        activeWeapon.SetDamage(damage);
        activeWeapon.EnableHit();
        MoveForward(moveDist, lockedAttackDirection, duration);

        yield return new WaitForSeconds(duration * 0.5f);
        activeWeapon.DisableHit();

        yield return new WaitForSeconds(duration * 0.5f);
    }


    void MoveForward(float distance, Vector2 direction, float duration)
    {
        if (moveRoutine != null)
            StopCoroutine(moveRoutine);

        moveRoutine = StartCoroutine(MoveRoutine(distance, direction, duration));
    }

    IEnumerator MoveRoutine(float distance, Vector2 direction, float duration)
    {
        Vector2 start = rb.position;
        Vector2 target = start + direction * distance;

        float elapsed = 0f;

        while (elapsed < duration)
        {
            rb.position = Vector2.Lerp(start, target, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        rb.position = target;
    }

    void RotateCombatPivot(Vector2 dir)
    {
        if (!combatPivot) return;

        float sign = Mathf.Sign(transform.lossyScale.x);
        Vector2 correctedDir = new Vector2(dir.x * sign, dir.y);

        float angle = Mathf.Atan2(correctedDir.y, correctedDir.x) * Mathf.Rad2Deg;
        combatPivot.rotation = Quaternion.Euler(0, 0, angle);
    }

    void ResetCombo()
    {
        currentState = State.Idle;
        isAttacking = false;

        weapon.DisableHit();
        lungeWeapon.DisableHit();
    }
}
