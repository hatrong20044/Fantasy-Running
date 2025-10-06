using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.VFX;
using UnityEngine.XR;

public class ObstaclePooler : MonoBehaviour
{
    public List<ObstaclePool> obstaclePools;
    private Dictionary<string, Queue<GameObject>> poolDictionary;

    private void Awake()
    {
        this.obstaclePools = new List<ObstaclePool>();
        this.poolDictionary = new Dictionary<string, Queue<GameObject>>();
    }
    
    // create pool for obstaclePools
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

        GameObject obstacle = this.poolDictionary[type].Dequeue();
        obstacle.SetActive(true);
        this.poolDictionary[type].Enqueue(obstacle);
        return obstacle;
    }
}