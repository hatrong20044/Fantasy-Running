using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstaclePooler : MonoBehaviour
{
    //public int poolSize = 20;
    //public Queue<GameObject> obstaclePool;
    //public GameObject obstaclePrefab;

    //private void Awake()
    //{
    //    this.obstaclePool = new Queue<GameObject>();

    //    for (int i = 0; i < poolSize; i++)
    //    {
    //        GameObject obstacle  = Instantiate(obstaclePrefab);
    //        obstacle.SetActive(false);
    //        obstaclePool.Enqueue(obstacle);
    //    }    
    //}

    //public GameObject GetObstacle()
    //{
    //    if(this.obstaclePool.Count > 0)
    //    {
    //        GameObject obstacle = obstaclePool.Dequeue();
    //        obstacle.SetActive(true);
    //        return obstacle;
    //    }
    //    else
    //    {
    //        GameObject obstacle = Instantiate(obstaclePrefab);
    //        return obstacle;
    //    }
    //}

    //public void ReturnObstacle(GameObject obstacle)
    //{
    //    obstacle.SetActive(false);
    //    obstaclePool.Enqueue(obstacle);
    //}

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