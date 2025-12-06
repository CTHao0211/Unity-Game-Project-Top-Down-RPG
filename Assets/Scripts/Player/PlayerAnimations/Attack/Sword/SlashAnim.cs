using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlashAnim : MonoBehaviour
{
    private ParticleSystem slashParticleSystem;
    private void Awake() {
        slashParticleSystem = GetComponent<ParticleSystem>();
    }
    private void Update()
    {
        if (slashParticleSystem && slashParticleSystem.IsAlive())
        {
            DestroySelf();
        }
    }
    public void DestroySelf() {
        Destroy(gameObject);
    }
}
