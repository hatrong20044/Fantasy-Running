using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class ObstaclePooler : MonoBehaviour
{
    public List<ObstaclePool> obstaclePools;
    private Dictionary<string, Queue<GameObject>> poolDictionary;

    private void Awake()
    {
        this.obstaclePools = new List<ObstaclePool>();
        this.poolDictionary = new Dictionary<string, Queue<GameObject>>();
    }
    
    public void generatePool()
    {
        foreach (ObstaclePool obstaclePool in this.obstaclePools)
        {
            Queue<GameObject> pool = new Queue<GameObject>();

            for (int i = 0; i < obstaclePool.size; i++)
            {
                GameObject obstacle = Instantiate(obstaclePool.obstaclePrefab);
                obstacle.SetActive(false);
                pool.Enqueue(obstacle);
            }

            this.poolDictionary.Add(obstaclePool.type, pool);
        }
    }

    //get obstacle from pool, make it active and set position and then, add it into pool again
    public GameObject getObstacle(string type)
    {
        if (!this.poolDictionary.ContainsKey(type)) return null;

        if (this.poolDictionary[type].Count > 0)
        {
            GameObject obstacle = this.poolDictionary[type].Dequeue();
            obstacle.SetActive(true);
            this.poolDictionary[type].Enqueue(obstacle);
            return obstacle;
        }
        else return null;
    }
}