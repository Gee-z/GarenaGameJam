using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponHitbox : MonoBehaviour
{
    public Collider2D[] hitCollider;
    private int damage = 10;
    private bool canHit = false;
    private HashSet<Collider2D> alreadyHit = new HashSet<Collider2D>();
    private bool lockRotation = false;
    private Vector3 fixedDirection;
    public Transform pivot;
    public Transform target;
    private void Awake()
    {
        foreach (var col in hitCollider)
            if (col != null) col.enabled = false;
    }    
    public void SetDamage(int dmg)
    {
        damage = dmg;
        alreadyHit.Clear();
    }
    public void EnableHit(bool lockRot = false, Vector3 direction = default)
    {
        canHit = true;
        alreadyHit.Clear();
        foreach (var col in hitCollider)
            if (col != null) col.enabled = true;

        // lockRotation = lockRot;
        if (lockRotation) fixedDirection = direction.normalized;
    }

    public void DisableHit()
    {
        canHit = false;
        alreadyHit.Clear();
        foreach (var col in hitCollider)
            if (col != null) col.enabled = false;

        lockRotation = false;
    }

    void Update()
    {
        if (pivot == null) return;

        Vector3 dir;

        if (lockRotation)
        {
            Debug.Log("lock");
            // Keep the attack locked in a fixed direction
            dir = fixedDirection;
        }
        else if (target != null)
        {
            // Normal pointing toward target (mouse or player)
            dir = (target.position - pivot.position).normalized;
        }
        else
        {
            return; // nothing to do
        }

        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        pivot.rotation = Quaternion.Euler(0f, 0f, angle);   
    }
    void OnTriggerEnter2D(Collider2D col)
    {
        if (!canHit) return;
        if (alreadyHit.Contains(col)) return;

        IDamageable damageable = col.GetComponent<IDamageable>();
        if (damageable != null)
        {
            damageable.TakeDamage(damage);
            alreadyHit.Add(col);
            Debug.Log($"Hit {col.name} for {damage} damage");
        }
    }
    public void UpdateRotation(Vector3 mouseWorld)
    {
        if (pivot == null) return;

        if (lockRotation)
        {
            float angle = Mathf.Atan2(fixedDirection.y, fixedDirection.x) * Mathf.Rad2Deg;
            pivot.rotation = Quaternion.Euler(0f, 0f, angle );
        }
        else
        {
            Vector3 dir = (mouseWorld - pivot.position).normalized;
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            pivot.rotation = Quaternion.Euler(0f, 0f, angle );
        }
    }
    public void lockRotationFunc()
    {
        lockRotation = true;
    }

    public void UnlockRotation()
    {
        lockRotation = false;
    }
}
