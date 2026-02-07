using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponHitbox : MonoBehaviour
{
    public Collider2D hitCollider;
    private int damage = 10;
    private bool canHit = false;
    private HashSet<Collider2D> alreadyHit = new HashSet<Collider2D>();
    private Transform player;

    private void Awake()
    {
        player = transform.parent;
        if (hitCollider != null)
            hitCollider.enabled = false; 
    }    
    public void SetDamage(int dmg)
    {
        damage = dmg;
        alreadyHit.Clear();
    }
    public void EnableHit()
    {
        canHit = true;
        alreadyHit.Clear();
        hitCollider.enabled = true;
    }

    public void DisableHit()
    {
        canHit = false;
        alreadyHit.Clear();
        hitCollider.enabled = false;
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
   public void PointTowardTarget(Vector3 targetPosition, float weaponLength = 0.5f)
{
    if (player == null) return;

    Vector3 direction = (targetPosition - player.position).normalized;

    transform.position = player.position + direction * weaponLength;

    float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

    transform.rotation = Quaternion.Euler(0f, 0f, angle + 180f);
}

}
