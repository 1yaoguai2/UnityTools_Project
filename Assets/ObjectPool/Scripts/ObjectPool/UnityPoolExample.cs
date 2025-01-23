using UnityEngine;
using UnityEngine.Pool;

// This example spans a random number of ParticleSystems using a pool so that old systems can be reused.
public class UnityPoolExample : MonoBehaviour
{
    public enum PoolType
    {
        Stack,
        LinkedList
    }

    public PoolType poolType;

    // Collection checks will throw errors if we try to release an item that is already in the pool.
    public bool collectionChecks = true;
    public int maxPoolSize = 10;

    IObjectPool<ParticleSystem> m_Pool;

    public IObjectPool<ParticleSystem> Pool
    {
        get
        {
            if (m_Pool == null)
            {
                if (poolType == PoolType.Stack)
                    m_Pool = new ObjectPool<ParticleSystem>(CreatePooledItem, OnTakeFromPool, OnReturnedToPool, OnDestroyPoolObject, collectionChecks, 10, maxPoolSize);
                else
                    m_Pool = new LinkedPool<ParticleSystem>(CreatePooledItem, OnTakeFromPool, OnReturnedToPool, OnDestroyPoolObject, collectionChecks, maxPoolSize);
            }
            return m_Pool;
        }
    }

    //粒子特效池，创建粒子特效
    ParticleSystem CreatePooledItem()
    {
        var go = new GameObject("Pooled Particle System");
        var ps = go.AddComponent<ParticleSystem>();
        ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);

        var main = ps.main;
        main.duration = 1;
        main.startLifetime = 1;
        main.loop = false;

        // This is used to return ParticleSystems to the pool when they have stopped.
        var returnToPool = go.AddComponent<ReturnToPool>();
        returnToPool.pool = Pool;

        return ps;
    }

    // Called when an item is returned to the pool using Release
    //粒子特效收回
    void OnReturnedToPool(ParticleSystem system)
    {
        system.gameObject.SetActive(false);
    }

    // Called when an item is taken from the pool using Get
    //粒子特效出现
    void OnTakeFromPool(ParticleSystem system)
    {
        system.gameObject.SetActive(true);
    }

    // If the pool capacity is reached then any items returned will be destroyed.
    // We can control what the destroy behavior does, here we destroy the GameObject.
    //粒子特效池大于最大数量的粒子特效删除
    void OnDestroyPoolObject(ParticleSystem system)
    {
        Destroy(system.gameObject);
    }

    private void OnGUI()
    {
        int count = Pool.CountInactive;
        GUI.Box(new Rect(100,100,150,50),$"Pool size: {count}");
        if (GUI.Button(new Rect(100,200,150,50),"Create Particles"))
        {
            var amount = Random.Range(1, 10);
            for (int i = 0; i < amount; ++i)
            {
                var ps = Pool.Get();
                ps.transform.position = Random.insideUnitSphere * 10;
                ps.Play();
            }
        }
    }
}