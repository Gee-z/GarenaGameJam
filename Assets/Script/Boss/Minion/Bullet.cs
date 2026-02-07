using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float maxLifeTime = 6f;
    public int damage = 10;
    void Start()
    {
        Destroy(gameObject, maxLifeTime);
    }
    void OnTriggerEnter2D(Collider2D col)
    {
        if (!col.CompareTag("Player")) return;
        IDamageable dmg = col.GetComponent<IDamageable>();
        if (dmg != null)
        {
            dmg.TakeDamage(damage);
        }
        Destroy(gameObject);
    }
}
