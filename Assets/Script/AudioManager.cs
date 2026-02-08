using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Audio Sources")]
    [SerializeField] private AudioSource bgmSource;
    [SerializeField] private AudioSource sfxSource;

    [Header("BGM Clips")]
    public AudioClip[] bgmClips;

    [Header("SFX Clips")]
    public AudioClip[] sfxClips;

    [Header("Settings")]
    public float fadeDuration = 0.3f;

    private Coroutine bgmCoroutine;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        bgmSource.loop = true;
        bgmSource.volume = 1f;
        sfxSource.volume = 1f;
    }

    public void PlayBGM(int index)
    {
        if (index < 0 || index >= bgmClips.Length)
            return;

        if (bgmCoroutine != null)
            StopCoroutine(bgmCoroutine);

        bgmCoroutine = StartCoroutine(SwitchBGM(bgmClips[index]));
    }

    public void StopBGM()
    {
        if (bgmCoroutine != null)
            StopCoroutine(bgmCoroutine);

        bgmCoroutine = StartCoroutine(FadeOutBGM());
    }

    private IEnumerator SwitchBGM(AudioClip newClip)
    {
        yield return FadeOutBGM();

        bgmSource.clip = newClip;
        bgmSource.Play();

        yield return FadeInBGM();
    }

    private IEnumerator FadeOutBGM()
    {
        float startVolume = bgmSource.volume;

        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            bgmSource.volume = Mathf.Lerp(startVolume, 0f, t / fadeDuration);
            yield return null;
        }

        bgmSource.volume = 0f;
        bgmSource.Stop();
    }

    private IEnumerator FadeInBGM()
    {
        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            bgmSource.volume = Mathf.Lerp(0f, 1f, t / fadeDuration);
            yield return null;
        }

        bgmSource.volume = 1f;
    }

    public void PlaySFX(int index)
    {
        if (index < 0 || index >= sfxClips.Length)
            return;

        sfxSource.PlayOneShot(sfxClips[index]);
    }
}
