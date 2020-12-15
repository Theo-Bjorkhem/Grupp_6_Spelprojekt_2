using UnityEngine;

[ExecuteInEditMode]
public class ParticleSystemParent : MonoBehaviour
{
    [SerializeField] Gradient myColorOverLifetime;
    [SerializeField] float mySpawnRate = 2.0f;
    [SerializeField] float myLifetime = 30.0f;
    [SerializeField] float mySpeed = 0.1f;

    public void ApplyChanges()
    {
        foreach (var system in GetComponentsInChildren<ParticleSystem>())
        {
            var colorOverLifetime = system.colorOverLifetime;
            var emission = system.emission;
            var main = system.main;

            colorOverLifetime.color = myColorOverLifetime;
            emission.rateOverTimeMultiplier = mySpawnRate;
            main.startLifetimeMultiplier = myLifetime;
            main.startSpeedMultiplier = mySpeed;
        }
    }
}
