using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class Pool
{
    public string tag;
    public GameObject prefab;
    public int size;
}

public class ObjectPoolManager : MonoBehaviour
{
    public static ObjectPoolManager Instance;

    [SerializeField] private List<Pool> pools;
    private Dictionary<string, Queue<GameObject>> poolDictionary;
    private Dictionary<GameObject, string> prefabToTagMap;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        InitializePools();
    }

    private void InitializePools()
    {
        poolDictionary = new Dictionary<string, Queue<GameObject>>();
        prefabToTagMap = new Dictionary<GameObject, string>();

        foreach (Pool pool in pools)
        {
            Queue<GameObject> objectPool = new Queue<GameObject>();

            for (int i = 0; i < pool.size; i++)
            {
                GameObject obj = Instantiate(pool.prefab);
                obj.SetActive(false);
                objectPool.Enqueue(obj);

                // 添加PooledObject组件用于自动回收
                if (!obj.TryGetComponent<PooledObject>(out var pooledObj))
                {
                    pooledObj = obj.AddComponent<PooledObject>();
                }
                pooledObj.PoolTag = pool.tag;
            }

            poolDictionary.Add(pool.tag, objectPool);
            prefabToTagMap.Add(pool.prefab, pool.tag);
        }
    }

    public GameObject SpawnFromPool(string tag, Vector3 position, Quaternion rotation, Transform parent = null)
    {
        if (!poolDictionary.ContainsKey(tag))
        {
            Debug.LogWarning($"Pool with tag {tag} doesn't exist.");
            return null;
        }

        GameObject objectToSpawn;
        if (poolDictionary[tag].Count > 0)
        {
            objectToSpawn = poolDictionary[tag].Dequeue();
        }
        else
        {
            // 动态扩容 - 如果池子空了，新建一个对象
            Pool pool = pools.Find(p => p.tag == tag);
            if (pool != null)
            {
                objectToSpawn = Instantiate(pool.prefab);

                // 添加PooledObject组件
                if (!objectToSpawn.TryGetComponent<PooledObject>(out var pooledObj))
                {
                    pooledObj = objectToSpawn.AddComponent<PooledObject>();
                    pooledObj.PoolTag = tag;
                }
            }
            else
            {
                Debug.LogWarning($"No pool found with tag {tag}");
                return null;
            }
        }

        objectToSpawn.SetActive(true);
        objectToSpawn.transform.SetPositionAndRotation(position, rotation);
        objectToSpawn.transform.SetParent(parent, false);

        // 调用对象的OnSpawn方法（如果存在）
        IPoolable[] poolables = objectToSpawn.GetComponentsInChildren<IPoolable>();
        foreach (IPoolable poolable in poolables)
        {
            poolable.OnSpawn();
        }

        return objectToSpawn;
    }

    public void ReturnToPool(GameObject objectToReturn)
    {
        if (objectToReturn == null) return;

        if (!objectToReturn.TryGetComponent<PooledObject>(out var pooledObj))
        {
            Debug.LogWarning("Object doesn't belong to any pool - destroying it");
            Destroy(objectToReturn);
            return;
        }

        string tag = pooledObj.PoolTag;
        if (!poolDictionary.ContainsKey(tag))
        {
            Debug.LogWarning($"Pool with tag {tag} doesn't exist - destroying object");
            Destroy(objectToReturn);
            return;
        }

        // 调用对象的OnReturn方法（如果存在）
        IPoolable[] poolables = objectToReturn.GetComponentsInChildren<IPoolable>();
        foreach (IPoolable poolable in poolables)
        {
            poolable.OnReturn();
        }

        objectToReturn.SetActive(false);
        objectToReturn.transform.SetParent(transform); // 放回池子根节点
        poolDictionary[tag].Enqueue(objectToReturn);
    }

    // 预加载更多对象到池中
    public void PreloadMore(string tag, int count)
    {
        if (!poolDictionary.ContainsKey(tag))
        {
            Debug.LogWarning($"Pool with tag {tag} doesn't exist.");
            return;
        }

        Pool pool = pools.Find(p => p.tag == tag);
        if (pool == null) return;

        for (int i = 0; i < count; i++)
        {
            GameObject obj = Instantiate(pool.prefab);
            obj.SetActive(false);

            if (!obj.TryGetComponent<PooledObject>(out var pooledObj))
            {
                pooledObj = obj.AddComponent<PooledObject>();
                pooledObj.PoolTag = tag;
            }

            poolDictionary[tag].Enqueue(obj);
        }
    }
}

// 用于标记池对象的组件
public class PooledObject : MonoBehaviour
{
    public string PoolTag { get; set; }

    private void OnDisable()
    {
        // 自动回收
        if (ObjectPoolManager.Instance != null)
        {
            ObjectPoolManager.Instance.ReturnToPool(gameObject);
        }
    }
}

// 可选的接口，用于对象被取出/回收时执行自定义逻辑
public interface IPoolable
{
    void OnSpawn();
    void OnReturn();
}