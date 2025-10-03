using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    public int poolSize = 20;
    public Queue<GameObject> obstaclePool;
    public GameObject obstaclePrefab;

    private void Awake()
    {
        this.obstaclePool = new Queue<GameObject>();

        for (int i = 0; i < poolSize; i++)
        {
            GameObject obstacle  = Instantiate(obstaclePrefab);
            obstacle.SetActive(false);
            obstaclePool.Enqueue(obstacle);
        }    
    }

    public GameObject GetObstacle()
    {
        if(this.obstaclePool.Count > 0)
        {
            GameObject obstacle = obstaclePool.Dequeue();
            obstacle.SetActive(true);
            return obstacle;
        }
        else
        {
            GameObject obstacle = Instantiate(obstaclePrefab);
            return obstacle;
        }
    }

    public void ReturnObstacle(GameObject obstacle)
    {
        obstacle.SetActive(false);
        obstaclePool.Enqueue(obstacle);
    }
}