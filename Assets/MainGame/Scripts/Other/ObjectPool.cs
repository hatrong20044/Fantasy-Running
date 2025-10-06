using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PoolInfo
{
    public string poolName; // "Coin", "Winter", "Summer", "Obstacle",....
    public GameObject prefab;
    public int poolSize;
    public int maxPoolSize;
    public Transform parent;

}
public class ObjectPool : MonoBehaviour
{
    [SerializeField] private static ObjectPool instance;
    public static ObjectPool Instance => instance;
    [SerializeField] private List<PoolInfo> poolInfos = new();
    [SerializeField] private Dictionary<string, Queue<GameObject>> poolDictionary = new();
    [SerializeField] private Dictionary<string, PoolInfo> poolInfoDictionary = new();

    private void Awake()
    {
        this.Setup();
    }

    private void Setup()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // Không hủy ObjectPool khi chuyển scene;
        }
        else
        {
            Destroy(gameObject);
            return;

        }
        this.InitializePools();
    }

    private void InitializePools()
    {
        foreach(var poolInfo in poolInfos)
        {
            Queue<GameObject> pool = new();
            poolInfoDictionary.Add(poolInfo.poolName, poolInfo);
            for(int i = 0; i < poolInfo.poolSize; i++)
            {
                GameObject obj = Instantiate(poolInfo.prefab, poolInfo.parent);
                obj.SetActive(false);
                pool.Enqueue(obj);
            }
            poolDictionary.Add(poolInfo.poolName, pool);
        }

    }

    public GameObject GetObjectFromPools(string poolName)
    {
        if (!poolDictionary.ContainsKey(poolName))
        {
            Debug.Log($"Pool with name {poolName} doesn't exist.");
            return null;
        }
        Queue<GameObject> pool = poolDictionary[poolName];
        GameObject obj;
        
        if(pool.Count > 0)
        {
            obj = pool.Dequeue();
        }
        else
        {
            PoolInfo info = poolInfoDictionary[poolName];
            obj = Instantiate (info.prefab, info.parent);
            poolInfoDictionary[poolName].poolSize++;
        }
        obj.SetActive(true);
        obj.GetComponent<IPooledObject>()?.OnSpawn();
        return obj;
    }

    public void ReturnToPool(string poolName, GameObject obj)
    {
        if (!poolDictionary.ContainsKey(poolName))
        {
            Debug.LogError($"Pool with name {poolName} doesn't exist.");
            return;
        }

        obj.GetComponent<IPooledObject>()?.OnReturn();
        obj.SetActive(false);
        poolDictionary[poolName].Enqueue(obj);
    }

}
