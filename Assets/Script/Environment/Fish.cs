using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Fish : MonoBehaviour
{
    public int hitsToBreak = 6;
    int hitCount;
    public UnityEvent onFishDestroyed;

    public Animator animator;
    public void OnBonk()
    {
        hitCount++;
        Debug.Log($"Fish hit {hitCount}/{hitsToBreak}");

        animator?.SetTrigger("Damaged");

        if (hitCount >= hitsToBreak)
        {
            Debug.Log("Fish reached 6 hits!");
            //change scene
            onFishDestroyed?.Invoke();
            Destroy(gameObject);
        }
    }
}
