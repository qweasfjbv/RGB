using UnityEngine;

public class ParticleSizeOverLifetime : MonoBehaviour
{
    private ParticleSystem ps;
    private ParticleSystem.SizeOverLifetimeModule sizeModule;

    void Start()
    {
        ps = GetComponent<ParticleSystem>();
        sizeModule = ps.sizeOverLifetime;
        sizeModule.enabled = true;

        AnimationCurve curve = new AnimationCurve();
        curve.AddKey(0.0f, 1.0f); // 시작점
        curve.AddKey(1.0f, 0.0f); // 끝점

        ParticleSystem.MinMaxCurve sizeCurve = new ParticleSystem.MinMaxCurve(1.0f, curve);
        sizeModule.size = sizeCurve;
    }
}