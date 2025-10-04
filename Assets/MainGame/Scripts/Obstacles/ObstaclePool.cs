using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstaclePool
{
    public string type;
    public GameObject obstaclePrefab;
    public int size = 20;

    public ObstaclePool(string type, GameObject obstaclePrefab, int size) 
    {
        this.type = type;
        this.obstaclePrefab = obstaclePrefab;
        this.size = size;
    }

    public ObstaclePool() { }
}
