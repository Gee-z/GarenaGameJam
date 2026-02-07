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
    public Animator anim;
    private State currentState = State.Idle;
    private float stateTimer = 0f;
    private bool attackBuffered = false;
    private bool attackHitDone = false;
    private Coroutine moveCoroutine;
    //private Animator anim;
    public bool isAttacking = false;
    private Vector3 attackDirection;
    public float attackDuration1 = 0.4f;
    public float attackDuration2 = 0.4f;
    public float attackDuration3 = 0.7f;

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

        float attackDur = GetCurrentAttackDuration(); // get duration for this attack
        stateTimer -= Time.deltaTime;

        // Trigger attack hit halfway through this attack
        if (!attackHitDone && stateTimer <= attackDur / 2f)
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
        isAttacking = true;
        WeaponHitbox activeWeapon = null;
        float moveDistance = 0f;
        if (GetComponent<Movement>() != null)
        {
            GetComponent<Movement>().LockMovement();
        }
        weapon.lockRotationFunc();
        lungeWeapon.lockRotationFunc();
        // Move forward based on attack type
        switch (currentState)
        {
            case State.Attack1:
                activeWeapon = weapon;
                moveDistance = attack1Move;
                activeWeapon.SetDamage(attack1Damage);
                anim.SetTrigger("Attack1");
                break;
            case State.Attack2:
                activeWeapon = weapon;
                moveDistance = attack2Move;
                activeWeapon.SetDamage(attack2Damage);
                anim.SetTrigger("Attack2");
                break;
            case State.Attack3:
                activeWeapon = lungeWeapon;
                moveDistance = attack3Lunge;
                activeWeapon.SetDamage(attack3Damage);
                anim.SetTrigger("Attack3"); 
                break;
        }
        float attackDur = GetCurrentAttackDuration();
        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorld.z = 0;
        attackDirection = (mouseWorld - transform.position).normalized; // lock this direction

        MoveForward(moveDistance, attackDirection, attackDur);

        // Enable weapon collider and lock rotation
        if (activeWeapon != null)
        {
            activeWeapon.EnableHit(true, attackDirection); // lock rotation to this direction
            StartCoroutine(DisableWeaponAfter(activeWeapon, attackDuration));
        }

        // Lock movement
        isAttacking = true;
    }

    private float GetCurrentAttackDuration()
    {
        switch (currentState)
        {
            case State.Attack1: return attackDuration1;
            case State.Attack2: return attackDuration2;
            case State.Attack3: return attackDuration3;
            default: return 0.4f; // fallback
        }
    }
    void ResetCombo()
    {
        currentState = State.Idle;
        stateTimer = 0f;
        attackBuffered = false;
        attackHitDone = false;
        isAttacking = false;
        if (weapon != null) weapon.DisableHit();
        if (lungeWeapon != null) lungeWeapon.DisableHit();
        if (GetComponent<Movement>() != null)
            GetComponent<Movement>().UnlockMovement();
        weapon.UnlockRotation();
        lungeWeapon.UnlockRotation();
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
                    //anim.SetTrigger("Attack1");
                    StartCoroutine(DisableWeaponAfter(weapon, attackDuration / 2f));
                }
                break;
            case State.Attack2:
                if (weapon != null)
                {
                    weapon.SetDamage(attack2Damage);
                    weapon.EnableHit();
                    //anim.SetTrigger("Attack2");
                    StartCoroutine(DisableWeaponAfter(weapon, attackDuration / 2f));
                }
                break;
            case State.Attack3:
                if (lungeWeapon != null)
                {
                    lungeWeapon.SetDamage(attack3Damage);
                    lungeWeapon.EnableHit();
                    //anim.SetTrigger("Attack3");
                    StartCoroutine(DisableWeaponAfter(lungeWeapon, attackDuration));
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
            weapon.UpdateRotation(mouseWorld: Camera.main.ScreenToWorldPoint(Input.mousePosition));

        if (lungeWeapon != null)
            lungeWeapon.UpdateRotation(mouseWorld: Camera.main.ScreenToWorldPoint(Input.mousePosition));
    }

    void MoveForward(float distance, Vector3 direction, float duration)
    {
        if (moveCoroutine != null) StopCoroutine(moveCoroutine);
        moveCoroutine = StartCoroutine(MoveForwardCoroutine(distance, direction, duration));
    }

    IEnumerator MoveForwardCoroutine(float distance, Vector3 direction, float duration)
    {
        yield return new WaitForSeconds(0.2f);
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb == null) yield break;

        Vector2 startPos = rb.position;
        Vector2 targetPos = startPos + (Vector2)(direction.normalized * distance);
        float elapsed = 0f;

        // Calculate the velocity needed to reach the target in "duration / 2"
        Vector2 velocity = (targetPos - startPos) / (duration / 2f);

        // Temporarily override Rigidbody velocity
        while (elapsed < duration / 2f)
        {
            rb.velocity = velocity;
            elapsed += Time.deltaTime;
            yield return null;
        }

        // Snap to the final position and stop velocity
        rb.position = targetPos;
        rb.velocity = Vector2.zero;
    }
}
