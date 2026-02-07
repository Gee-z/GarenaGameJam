using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destructible : MonoBehaviour, IDamageable
{
    public int maxHP = 30;
    private int currentHP;

    void Awake()
    {
        currentHP = maxHP;
    }

    public void TakeDamage(int amount)
    {
        currentHP -= amount;
        Debug.Log($"{gameObject.name} took {amount} damage. HP left: {currentHP}");

        if (currentHP <= 0)
        {
            Break();
        }
    }

    void Break()
    {
        Debug.Log($"{gameObject.name} broke!");
        Destroy(gameObject);
    }
}
