using UnityEngine;

public class ParticleController : MonoBehaviour
{
    public ParticleSystem particle;

    public void Play()
    {
        if (particle.isPlaying) return;
        particle.Play();
    }

    public void Stop()
    {
        if (!particle.isPlaying) return;
        particle.Stop();
    }
}
