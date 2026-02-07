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
    public string targetTag;
    private Dictionary<Collider2D, bool> originalIsTrigger = new Dictionary<Collider2D, bool>();
    private void Awake()
    {
        foreach (var col in hitCollider)
            if (col != null)
            {
                // remember original trigger state then disable collider
                originalIsTrigger[col] = col.isTrigger;
                col.enabled = false;
            }
    }    

    // Centralized processing for applying damage to a collider target
    private void ProcessHitOnCollider(Collider2D col)
    {
        if (!canHit) return;
        if (alreadyHit.Contains(col)) return;

        // TAG FILTER (if set)
        if (!string.IsNullOrEmpty(targetTag) && !col.CompareTag(targetTag))
            return;

        IDamageable damageable = col.GetComponent<IDamageable>()
                                ?? col.GetComponentInParent<IDamageable>()
                                ?? col.attachedRigidbody?.GetComponent<IDamageable>();
        if (damageable != null)
        {
            damageable.TakeDamage(damage);
            alreadyHit.Add(col);
            Debug.Log($"Hit {col.name} for {damage} damage (via ProcessHitOnCollider)");
        }
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
            if (col != null)
            {
                // ensure hitbox acts as a trigger for overlap events
                if (!originalIsTrigger.ContainsKey(col)) originalIsTrigger[col] = col.isTrigger;
                col.isTrigger = true;
                col.enabled = true;
                Debug.Log($"WeaponHitbox EnableHit: enabled {col.name} isTrigger={col.isTrigger} attachedRigidbody={(col.attachedRigidbody!=null)}");

                // Immediately check for overlapping colliders so targets already inside hitbox get hit
                ContactFilter2D filter = new ContactFilter2D();
                filter.NoFilter();
                List<Collider2D> results = new List<Collider2D>();
                int count = col.OverlapCollider(filter, results);
                if (count > 0)
                {
                    Debug.Log($"WeaponHitbox EnableHit: {col.name} overlapping {count} colliders");
                    foreach (var r in results)
                        if (r != null)
                            ProcessHitOnCollider(r);
                }
            }

        lockRotation = lockRot;
        if (lockRotation) fixedDirection = direction.normalized;
    }

    public void DisableHit()
    {
        canHit = false;
        alreadyHit.Clear();
        foreach (var col in hitCollider)
            if (col != null)
            {
                // restore original trigger state when disabling
                if (originalIsTrigger.ContainsKey(col))
                    col.isTrigger = originalIsTrigger[col];
                col.enabled = false;
                Debug.Log($"WeaponHitbox DisableHit: disabled {col.name} isTrigger={col.isTrigger}");
            }

        lockRotation = false;
    }

    void Update()
    {
        if (pivot == null) return;

        Vector3 dir;

        if (lockRotation)
        {
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
        Debug.Log($"WeaponHitbox OnTriggerEnter2D: col={col.name} canHit={canHit} targetTag={targetTag} colTag={col.tag} isTrigger={col.isTrigger} attachedRigidbody={(col.attachedRigidbody!=null)}");
        if (!canHit) return;
        if (alreadyHit.Contains(col)) return;

        // TAG FILTER (if set)
        if (!string.IsNullOrEmpty(targetTag) && !col.CompareTag(targetTag))
            return;

        // Try to find an IDamageable on the collider, its parent, or the attached Rigidbody's GameObject
        IDamageable damageable = col.GetComponent<IDamageable>()
                                ?? col.GetComponentInParent<IDamageable>()
                                ?? col.attachedRigidbody?.GetComponent<IDamageable>();
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
    public void SetTarget(string tag)
    {
        targetTag = tag;
    }
}
