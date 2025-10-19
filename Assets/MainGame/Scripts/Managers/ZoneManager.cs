using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoneManager : MonoBehaviour
{
    [SerializeField] private float zoneSize = 40f;
    [SerializeField] private Transform cameraTranf;
    [SerializeField] private float positionLimit = 1.0f;

    private Dictionary<int, List<Vector3>> coinZones;
    public void RemoveCoin(Vector3 coinPos)
    {
        int zoneIndex = Mathf.FloorToInt(coinPos.z / zoneSize);
        if (coinZones.ContainsKey(zoneIndex))
        {
            coinZones[zoneIndex].Remove(coinPos);
            if(coinZones[zoneIndex].Count == 0)
            {
                coinZones.Remove(zoneIndex);
            }
        }
    }

    public void RegisterCoin(Vector3 coinPos)
    {
        int zoneIndex = Mathf.FloorToInt(coinPos.z / zoneSize);
        if (!coinZones.ContainsKey(zoneIndex))
        {
            coinZones[zoneIndex] = new List<Vector3>();
        }
        coinZones[zoneIndex].Add(coinPos);
    }

    public void RegisterObstacle (ObstaclePosition obsPos)
    {

    }

    public void RemoveOstacle(ObstaclePosition obsPos)
    {

    }
}
