using System.Collections.Generic;
using System.Linq;
using Palmmedia.ReportGenerator.Core.Parser.Analysis;
using UnityEngine;
using UnityEngine.Pool;

public class ObjectPool : MonoBehaviour
{
    public static ObjectPool Instance;

    //对象池
    public List<Pool> pools;

    //对象字典
    public Dictionary<string, Queue<GameObject>> poolDictionary;

    public GameObject currentObj;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
       InitPools();
    }

    /// <summary>
    /// 初始化对象池
    /// </summary>
    private void InitPools()
    {
        poolDictionary = new Dictionary<string, Queue<GameObject>>();
        foreach (var pool in pools)
        {
            Queue<GameObject> objectPool = new Queue<GameObject>();
            
            //初始化指定数量的对象
            for (int i = 0; i < pool.Size; i++)
            {
                var obj = Instantiate(pool.Prefab);
                obj.SetActive(false); //初始状态不可见
                objectPool.Enqueue(obj);
            }
            
            //对象池加入字典
            poolDictionary.Add(pool.Tag,objectPool);
        }
    }

    /// <summary>
    /// 替换创建功能
    /// </summary>
    /// <param name="tag"></param>
    /// <param name="position"></param>
    /// <param name="rotation"></param>
    /// <returns></returns>
    public GameObject SpawnFromPool(string tag, Vector3 position, Quaternion rotation)
    {
        currentObj = null;
        if (!poolDictionary.ContainsKey(tag))
        {
            Debug.LogError($"对象池{tag}不存在！");
            return null;
        }

        if (poolDictionary[tag].FirstOrDefault(t => t.activeInHierarchy == false) is null)
        {
            //需要扩充对象池的size
            var pool = pools.Find(t => t.Tag == tag);
            if (pool.Tag is null) return null;  //对象不存在
            currentObj = Instantiate(pool.Prefab);
        }
        currentObj ??= poolDictionary[tag].Dequeue();
        currentObj.transform.position = position;
        currentObj.transform.rotation = rotation;
        currentObj.SetActive(true);
        
        // 调用对象的初始化方法
        if (currentObj.TryGetComponent<IPooledObject>(out var pooledObj))
        {
            pooledObj.OnObjectSpawn();
        }
        
        // 将对象重新加入队列（先入先出）
        var newObj = currentObj;
        poolDictionary[tag].Enqueue(newObj);
        return newObj;
    }
    
}

[System.Serializable]
public class Pool : System.Object
{
    public string Tag; //对象标识
    public GameObject Prefab; //对象预制体
    public int Size; //池大小
}
    
/// <summary>
/// 可池化对象接口
/// </summary>
public interface IPooledObject
{
    /// <summary>
    /// 对象生成时调用
    /// </summary>
    void OnObjectSpawn();
}


