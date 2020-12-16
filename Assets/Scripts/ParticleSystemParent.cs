using UnityEngine;

[ExecuteInEditMode]
public class ParticleSystemParent : MonoBehaviour
{
    [SerializeField] Gradient myColorOverLifetime;
    [SerializeField] float mySpawnRate = 0.5f;
    [SerializeField] float myLifetime = 30.0f;
    [SerializeField] float mySpeed = 0.1f;
    [SerializeField] int myMaxParticles = 50;
    [SerializeField] float myDuration = 30.0f;

    public void ApplyChanges()
    {
        ParticleSystem onMe = GetComponent<ParticleSystem>();
        if (onMe != null)
        {
            ApplyTo(onMe);
        }
        foreach (var system in GetComponentsInChildren<ParticleSystem>())
        {
            ApplyTo(system);
        }
    }

    void ApplyTo(ParticleSystem system)
    {
        var colorOverLifetime = system.colorOverLifetime;
        var emission = system.emission;
        var main = system.main;

        colorOverLifetime.color = myColorOverLifetime;
        emission.rateOverTimeMultiplier = mySpawnRate;
        main.startLifetimeMultiplier = myLifetime;
        main.startSpeedMultiplier = mySpeed;
        main.maxParticles = myMaxParticles;
        main.duration = myDuration;
    }
}
