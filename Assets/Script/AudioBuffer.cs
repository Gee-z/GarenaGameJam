using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioBuffer : MonoBehaviour
{
    public void PlaySFX(int index)
    {
        AudioManager.Instance.PlaySFX(index);
    }
    public void PlayBGM(int index)
    {
        AudioManager.Instance.PlayBGM(index);
    }
}
