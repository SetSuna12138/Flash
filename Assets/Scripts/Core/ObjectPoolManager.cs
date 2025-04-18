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

                // ���PooledObject��������Զ�����
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
            // ��̬���� - ������ӿ��ˣ��½�һ������
            Pool pool = pools.Find(p => p.tag == tag);
            if (pool != null)
            {
                objectToSpawn = Instantiate(pool.prefab);

                // ���PooledObject���
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

        // ���ö����OnSpawn������������ڣ�
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

        // ���ö����OnReturn������������ڣ�
        IPoolable[] poolables = objectToReturn.GetComponentsInChildren<IPoolable>();
        foreach (IPoolable poolable in poolables)
        {
            poolable.OnReturn();
        }

        objectToReturn.SetActive(false);
        objectToReturn.transform.SetParent(transform); // �Żس��Ӹ��ڵ�
        poolDictionary[tag].Enqueue(objectToReturn);
    }

    // Ԥ���ظ�����󵽳���
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

// ���ڱ�ǳض�������
public class PooledObject : MonoBehaviour
{
    public string PoolTag { get; set; }

    private void OnDisable()
    {
        // �Զ�����
        if (ObjectPoolManager.Instance != null)
        {
            ObjectPoolManager.Instance.ReturnToPool(gameObject);
        }
    }
}

// ��ѡ�Ľӿڣ����ڶ���ȡ��/����ʱִ���Զ����߼�
public interface IPoolable
{
    void OnSpawn();
    void OnReturn();
}