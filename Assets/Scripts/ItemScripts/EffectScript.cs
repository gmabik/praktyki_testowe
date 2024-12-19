using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectScript : MonoBehaviour
{
    private ParticleSystem particle;
    public ClickManager clickManager;
    void Start()
    {
        particle = GetComponent<ParticleSystem>();
        clickManager.ClickCount.OnValueChanged += PlayParticle;
    }

    private void PlayParticle(int _prev, int _new)
        => particle.Play();
}
