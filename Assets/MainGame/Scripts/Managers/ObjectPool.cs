
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    //Them Dictionary<string, Pool> poolInfoDictionary
    public static ObjectPool Instance;
    public List<Pool> pools;
    public Dictionary<string, Queue<GameObject>> poolDictionary;
    public Dictionary<string, List<GameObject>> activeObjects;
    [System.Serializable]
    public class Pool
    {
        public string tag;
        public GameObject prefab;
        public int size;
        public Transform parent;
    }

    private void Awake()
    {
        this.Setup();
    }
    private void Setup()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Không hủy ObjectPool khi chuyển scene;
        }
        else
        {
            Destroy(gameObject);
            return;

        }
      //  this.InitalizePool();
        this.InitalizePoolQuynh();
    }

    private void Start()
    {
       Debug.Assert(CoinEvent.Instance != null, "CoinEvent.Instance is null in ObjectPool Start");
        CoinEvent.Instance.OnCoinCollected += coin => ReturnToPoolQuynh("Coin", coin);
      
    }

    private void OnDestroy()
    {
        CoinEvent.Instance.OnCoinCollected -= coin => ReturnToPoolQuynh("Coin", coin);
      
    }


    void InitalizePoolQuynh()
    {
        poolDictionary = new Dictionary<string, Queue<GameObject>>();
        activeObjects = new();
        foreach (Pool pool in pools)
        {
            Queue<GameObject> objectsPool = new Queue<GameObject>();
            activeObjects[pool.tag] = new();
            for (int i = 0; i < pool.size; i++)
            {
                GameObject obj = Instantiate(pool.prefab, pool.parent);
                obj.SetActive(false);
                objectsPool.Enqueue(obj);
            }
            poolDictionary.Add(pool.tag, objectsPool);
        }
    }

    public GameObject GetFromPoolQuynh(string tag)
    {
        if (!poolDictionary.ContainsKey(tag))
        {
            Debug.LogWarning("Pool với tag " + tag + " không tồn tại!");
            return null;
        }
        GameObject obj;
        if (poolDictionary[tag].Count > 0)
        {
            obj = poolDictionary[tag].Dequeue();
            obj.SetActive(true);
            activeObjects[tag].Add(obj);
            return obj;
        }
        else
        {
            return null;
        }
    }

    public void ReturnToPoolQuynh(string tag, GameObject obj)
    {
        if (!poolDictionary.ContainsKey(tag))
        {
            Debug.LogWarning("Pool với tag " + tag + " không tồn tại!");
            Destroy(obj);
            return;
        }

        obj.SetActive(false);
        poolDictionary[tag].Enqueue(obj);
        activeObjects[tag].Remove(obj);
    }

    public int GetActiveCount(string tag)
    {
        return activeObjects.ContainsKey(tag) ? activeObjects[tag].Count : 0;
    }

    public List<GameObject> GetActiveObjects(string tag)
    {
        return activeObjects.ContainsKey(tag) ? activeObjects[tag] : null;
    }

    void InitalizePool()
    {
        poolDictionary = new Dictionary<string, Queue<GameObject>>();

        foreach (Pool pool in pools)
        {
            Queue<GameObject> objectsPool = new Queue<GameObject>();
            for (int i = 0; i < pool.size; i++)
            {
                GameObject obj = Instantiate(pool.prefab, pool.parent);
                obj.SetActive(false);
                objectsPool.Enqueue(obj);
            }
            poolDictionary.Add(pool.tag, objectsPool);
        }
    }
    public GameObject GetFromPool(string tag)
    {
        if (!poolDictionary.ContainsKey(tag))
        {
            Debug.LogWarning("Pool với tag " + tag + " không tồn tại!");
            return null;
        }
        GameObject obj;
        if (poolDictionary[tag].Count > 0)
        {
            obj = poolDictionary[tag].Dequeue();
            obj.SetActive(true);
            return obj;
        }
        else
        {
            return null;
        }
    }

    public void ReturnToPool(string tag, GameObject obj)
    {
        if (!poolDictionary.ContainsKey(tag))
        {
            Debug.LogWarning("Pool với tag " + tag + " không tồn tại!");
            Destroy(obj);
            return;
        }

        obj.SetActive(false);
        poolDictionary[tag].Enqueue(obj);
    }

    public int GetCoinPoolSize()
    {
        Pool pool = pools.Find(p => p.tag == "Coin");
        return pool.size;
    }
}
