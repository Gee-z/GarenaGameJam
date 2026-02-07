using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    public float comboResetTime = 0.6f;
    public float attackDuration = 0.4f;
    public float comboWindow = 0.3f;
    public WeaponHitbox weapon;
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

    private State currentState = State.Idle;
    private float stateTimer = 0f;
    private bool attackBuffered = false;
    private bool attackHitDone = false;

    //private Animator anim;

    void Awake()
    {
        //anim = GetComponent<Animator>();
    }

    void Update()
    {
        HandleInput();
        UpdateState();
        weapon.PointTowardMouse();
    }

    void HandleInput()
    {
        if (Input.GetMouseButtonDown(0)) 
        {
            if (currentState == State.Idle)
            {
                StartAttack1();
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
        if (currentState == State.Attack1)
            StartAttack2();
        else if (currentState == State.Attack2)
            StartAttack3();

        // reset buffered input
        attackBuffered = false;
    }
    void StartAttack1()
    {
        currentState = State.Attack1;
        stateTimer = comboResetTime;
        attackBuffered = false;
        attackHitDone = false;
        Debug.Log("Attack1");
        //anim.Play("Attack1", 0, 0f);
    }

    void StartAttack2()
    {
        currentState = State.Attack2;
        stateTimer = comboResetTime;
        attackBuffered = false;
        attackHitDone = false;
        Debug.Log("Attack2");
       // anim.Play("Attack2", 0, 0f);
    }

    void StartAttack3()
    {
        currentState = State.Attack3;
        stateTimer = attackDuration;
        attackBuffered = false;
        attackHitDone = false;
        Debug.Log("Attack3");
        // anim.Play("Attack3", 0, 0f);
    }

    void ResetCombo()
    {
        currentState = State.Idle;
        stateTimer = 0f;
        attackBuffered = false;
        attackHitDone = false;
        if (weapon != null)
            weapon.DisableHit();
        Debug.Log("Combo Reset");
    }
    void TriggerAttackHit()
    {
        if (weapon == null)
        {
            Debug.LogWarning("Weapon not assigned!");
            return;
        }

        switch (currentState)
        {
            case State.Attack1: weapon.SetDamage(attack1Damage); break;
            case State.Attack2: weapon.SetDamage(attack2Damage); break;
            case State.Attack3: weapon.SetDamage(attack3Damage); break;
        }
        weapon.EnableHit();

        // Disable after half the attack duration
        StartCoroutine(DisableWeaponAfter(attackDuration / 2f));
    }
    IEnumerator DisableWeaponAfter(float time)
    {
        yield return new WaitForSeconds(time);
        if (weapon != null)
            weapon.DisableHit();
    }
}
