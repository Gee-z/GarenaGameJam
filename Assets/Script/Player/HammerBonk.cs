using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Rigidbody2D))]
public class HammerBonk : MonoBehaviour
{
    public float hitRange = 0.8f;
    public float hitOffset = 1f;
    public LayerMask hitMask;

    public void Bonk()
    {
        float dir = transform.root.localScale.x >= 0 ? 1f : -1f;
        Vector2 center = (Vector2)transform.position + Vector2.right * dir * hitOffset;

        Collider2D[] hits = Physics2D.OverlapCircleAll(center, hitRange, hitMask);

        foreach (var c in hits)
        {
            Fish fish = c.GetComponentInParent<Fish>();
            if (fish != null)
            {
                fish.OnBonk();
            }
        }
    }
}
