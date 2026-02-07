using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    public float comboResetTime = 0.6f;
    public float attackDuration = 0.4f;
    public float comboWindow = 0.3f;
    public WeaponHitbox weapon;
    public WeaponHitbox lungeWeapon;
    public int attack1Damage = 10;
    public int attack2Damage = 10;
    public int attack3Damage = 20;
    private enum State
    {
        Idle,
        Attack1,
        Attack2,
        Attack3
    }

    public float attack1Move = 0.3f;
    public float attack2Move = 0.4f;
    public float attack3Lunge = 1f;
    private State currentState = State.Idle;
    private float stateTimer = 0f;
    private bool attackBuffered = false;
    private bool attackHitDone = false;
    private Coroutine moveCoroutine;
    //private Animator anim;

    void Awake()
    {
        //anim = GetComponent<Animator>();
    }

    void Update()
    {
        HandleInput();
        UpdateState();
        PointWeaponsTowardMouse();
    }

    void HandleInput()
    {
        if (Input.GetMouseButtonDown(0)) 
        {
            if (currentState == State.Idle)
            {
                StartAttack(State.Attack1);
            }
            else
            {
                // buffer input for next attack
                attackBuffered = true;
            }
        }
    }

    void UpdateState()
    {
        if (currentState == State.Idle)
            return;

        stateTimer -= Time.deltaTime;
        if (!attackHitDone && stateTimer <= attackDuration / 2f)
        {
            TriggerAttackHit();
            attackHitDone = true;
        }

        // If we are within the combo window, allow input
        if (stateTimer <= attackDuration - comboWindow && attackBuffered)
        {
            BufferCombo();
        }

        // If attack finished and no buffered input, reset combo
        if (stateTimer <= 0f)
        {
            ResetCombo();
        }
    }

    void BufferCombo()
    {
        switch (currentState)
        {
            case State.Attack1: StartAttack(State.Attack2); break;
            case State.Attack2: StartAttack(State.Attack3); break;
        }
        attackBuffered = false;
    }
    void StartAttack(State newState)
    {
        currentState = newState;
        stateTimer = comboResetTime;
        attackBuffered = false;
        attackHitDone = false;
        WeaponHitbox activeWeapon = null;
        float moveDistance = 0f;
        // Move forward based on attack type
        switch (currentState)
        {
            case State.Attack1:
                activeWeapon = weapon;
                moveDistance = attack1Move;
                activeWeapon.SetDamage(attack1Damage);
                break;
            case State.Attack2:
                activeWeapon = weapon;
                moveDistance = attack2Move;
                activeWeapon.SetDamage(attack2Damage);
                break;
            case State.Attack3:
                activeWeapon = lungeWeapon;
                moveDistance = attack3Lunge;
                activeWeapon.SetDamage(attack3Damage);
                break;
        }

        // Move first (starts dash immediately)
        MoveForward(moveDistance);

        // Enable collider throughout the dash
        if (activeWeapon != null)
        {
            activeWeapon.EnableHit();
            StartCoroutine(DisableWeaponAfter(activeWeapon, attackDuration));
        }

        Debug.Log(currentState.ToString());
    }

    void ResetCombo()
    {
        currentState = State.Idle;
        stateTimer = 0f;
        attackBuffered = false;
        attackHitDone = false;

        if (weapon != null) weapon.DisableHit();
        if (lungeWeapon != null) lungeWeapon.DisableHit();
    }

    void TriggerAttackHit()
    {
        switch (currentState)
        {
            case State.Attack1:
                if (weapon != null)
                {
                    weapon.SetDamage(attack1Damage);
                    weapon.EnableHit();
                    StartCoroutine(DisableWeaponAfter(weapon, attackDuration / 2f));
                }
                break;
            case State.Attack2:
                if (weapon != null)
                {
                    weapon.SetDamage(attack2Damage);
                    weapon.EnableHit();
                    StartCoroutine(DisableWeaponAfter(weapon, attackDuration / 2f));
                }
                break;
            case State.Attack3:
                if (lungeWeapon != null)
                {
                    lungeWeapon.SetDamage(attack3Damage);
                    lungeWeapon.EnableHit();
                    StartCoroutine(DisableWeaponAfter(lungeWeapon, attackDuration / 2f));
                }
                break;
        }
    }

    IEnumerator DisableWeaponAfter(WeaponHitbox w, float time)
    {
        yield return new WaitForSeconds(time);
        if (w != null) w.DisableHit();
    }

    void PointWeaponsTowardMouse()
    {
        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorld.z = 0;

        if (weapon != null)
            weapon.PointTowardTarget(mouseWorld, 0.5f);

        if (lungeWeapon != null)
            lungeWeapon.PointTowardTarget(mouseWorld, 0.5f);
    }

    void MoveForward(float distance)
    {
        if (moveCoroutine != null) StopCoroutine(moveCoroutine);
        moveCoroutine = StartCoroutine(MoveForwardCoroutine(distance));
    }

    IEnumerator MoveForwardCoroutine(float distance)
    {
        Vector3 startPos = transform.position;
        Vector3 targetPos;

        // Choose direction: forward along current mouse direction
        Vector3 direction = Vector3.zero;

        if ((currentState == State.Attack1 || currentState == State.Attack2) && weapon != null)
            direction = (weapon.transform.position - transform.position).normalized;
        else if (currentState == State.Attack3 && lungeWeapon != null)
            direction = (lungeWeapon.transform.position - transform.position).normalized;

        targetPos = startPos + direction * distance;

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
