using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fish : MonoBehaviour
{
    public int hitsToBreak = 6;
    int hitCount;

    public Animator animator;
    public void OnBonk()
    {
        hitCount++;
        Debug.Log($"Fish hit {hitCount}/{hitsToBreak}");

        animator?.SetTrigger("FishHit");

        if (hitCount >= hitsToBreak)
        {
            Debug.Log("Fish reached 6 hits!");
            //change scene
            Destroy(gameObject);
        }
    }
}
