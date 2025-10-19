using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoneManager : MonoBehaviour
{
    [SerializeField] private float zoneSize = 40f;
    [SerializeField] private Transform cameraTranf;
    [SerializeField] private float distanceThresold = 3.0f;

    private Dictionary<int, List<Vector3>> coinZones;
    private Dictionary<int, List<ObstaclePosition>> obstacleZones;

    public void Awake()
    {
        coinZones = new();
        obstacleZones = new();
    }
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

    public void RegisterObstacle(ObstaclePosition obsPos)
    {
        int zoneIndex = Mathf.FloorToInt(obsPos.Position.z / zoneSize);
        if (!obstacleZones.ContainsKey(zoneIndex))
        {
            obstacleZones[zoneIndex] = new List<ObstaclePosition>();
        }
        obstacleZones[zoneIndex].Add(obsPos);
    }

    public void RemoveOstacle(ObstaclePosition obsPos)
    {
        int zoneIndex = Mathf.FloorToInt(obsPos.Position.z / zoneSize);
        if (obstacleZones.ContainsKey(zoneIndex))
        {
            obstacleZones[zoneIndex].Remove(obsPos);
            if (obstacleZones[zoneIndex].Count == 0)
            {
                obstacleZones.Remove(zoneIndex);
            }
        }
    }


    public bool CanPlaceCoin(Vector3 coinPos)
    {
        int zoneIndex = Mathf.FloorToInt(coinPos.z / zoneSize);
        for(int i = zoneIndex - 1; i <= zoneIndex + 1; i++)
        {
            if (obstacleZones.ContainsKey(i))
            {
                foreach(ObstaclePosition obsPos in obstacleZones[i])
                {
                    if (obsPos.Type.subType == ObstacleType.ObstacleSubType.NonPassableStatic || 
                        obsPos.Type.subType == ObstacleType.ObstacleSubType.NonPassableDynamic)
                    {
                        float distance = Vector2.Distance(
                            new Vector2(coinPos.x, coinPos.z),
                            new Vector2(obsPos.Position.x, obsPos.Position.y)
                            );
                        if(distance < distanceThresold)
                        {
                            return false;
                        }
                    }
                    
                }
            }
            else
            {
                Debug.Log("chua co key nay");
            }
        }
        return true;
    }
}
