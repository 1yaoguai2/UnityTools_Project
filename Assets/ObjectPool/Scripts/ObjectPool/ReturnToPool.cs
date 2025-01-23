using System.Text;
using UnityEngine;
using UnityEngine.Pool;

// 当粒子系统stop时，对象池回收，直接release
[RequireComponent(typeof(ParticleSystem))]
public class ReturnToPool : MonoBehaviour
{
    private ParticleSystem _system;
    public IObjectPool<ParticleSystem> pool;

    void Start()
    {
        _system = GetComponent<ParticleSystem>();
        var main = _system.main;
        main.stopAction = ParticleSystemStopAction.Callback;
    }

    void OnParticleSystemStopped()
    {
        // Return to the pool
        pool.Release(_system);
    }
}


