
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    public static ObjectPool Instance;
    public List<Pool> pools;
    public Dictionary<string, Queue<GameObject>> poolDictionary;
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
        this.InitalizePool();
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
            Pool pool = pools.Find(p => p.tag == tag);
            if (pool == null)
            {
                Debug.LogWarning("Không tìm thấy pool với tag " + tag);
                return null;
            }
            obj = Instantiate(pool.prefab, pool.parent);
            obj.SetActive(true);
            return obj;
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

}
