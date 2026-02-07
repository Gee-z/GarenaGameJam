using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
public class Health : MonoBehaviour, IDamageable
{
    public enum EntityRole { None, Player, Boss }
    public EntityRole role = EntityRole.None;
    public int maxHP = 50;
    private int currentHP;
    public UnityEvent onDeath;
    public UnityEvent<int, int> onHealthChanged;
    public float iFrameDuration = 0.5f; // seconds of invulnerability after getting hit
    private bool invulnerable = false;
    private Coroutine iFrameCoroutine = null;

    void Awake()
    {
        currentHP = maxHP;
        onHealthChanged?.Invoke(currentHP, maxHP);
    }

    public void TakeDamage(int damage)
    {
        if (invulnerable) return;

        currentHP -= damage;
        currentHP = Mathf.Clamp(currentHP, 0, maxHP);

        onHealthChanged?.Invoke(currentHP, maxHP);

        if (currentHP <= 0)
            Die();
        else
        {
            // Start invulnerability frames
            if (iFrameCoroutine != null) StopCoroutine(iFrameCoroutine);
            iFrameCoroutine = StartCoroutine(InvulnerabilityCoroutine());
        }
    }

    IEnumerator InvulnerabilityCoroutine()
    {
        invulnerable = true;
        // simple visual feedback: flash sprite renderers if present
        SpriteRenderer[] rends = GetComponentsInChildren<SpriteRenderer>(true);
        Color[] originals = new Color[rends.Length];
        for (int i = 0; i < rends.Length; i++) originals[i] = rends[i].color;

        float elapsed = 0f;
        float flashInterval = 0.1f;
        while (elapsed < iFrameDuration)
        {
            // tint red
            for (int i = 0; i < rends.Length; i++) if (rends[i] != null) rends[i].color = Color.red;
            yield return new WaitForSeconds(flashInterval);
            elapsed += flashInterval;
            // restore
            for (int i = 0; i < rends.Length; i++) if (rends[i] != null) rends[i].color = originals[i];
            yield return new WaitForSeconds(flashInterval);
            elapsed += flashInterval;
        }

        // ensure original colors restored
        for (int i = 0; i < rends.Length; i++) if (rends[i] != null) rends[i].color = originals[i];
        invulnerable = false;
        iFrameCoroutine = null;
    }

    void Die()
    {
        onDeath?.Invoke();
        // Trigger global win/lose if this health belongs to player or boss
        if (role == EntityRole.Player)
        {
            if (WinManager.Instance != null) WinManager.Instance.Lose();
        }
        else if (role == EntityRole.Boss)
        {
            if (WinManager.Instance != null) WinManager.Instance.Win();
        }
        Destroy(gameObject);
    }
}
