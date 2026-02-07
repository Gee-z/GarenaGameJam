using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponHitbox : MonoBehaviour
{
    public Collider2D coneCollider;
    private int damage = 10;
    private bool canHit = false;
    private HashSet<Collider2D> alreadyHit = new HashSet<Collider2D>();
    private Transform player;

    private void Awake()
    {
        player = transform.parent;
        if (coneCollider != null)
            coneCollider.enabled = false; 
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
        coneCollider.enabled = true;
    }

    public void DisableHit()
    {
        canHit = false;
        alreadyHit.Clear();
        coneCollider.enabled = false;
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
    public void PointTowardMouse()
    {
        if (player == null) return;

        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorld.z = 0;

        // Rotate the weapon so the base points toward the mouse
        Vector3 direction = (mouseWorld - player.position).normalized;

        float weaponLength = 0.5f;
        transform.position = player.position + direction * weaponLength;;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }
}
