
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ZoneManager : MonoBehaviour
{
    [SerializeField] private float zoneSize = 40f;
    [SerializeField] private float distanceThresold = 5.0f;
    private Dictionary<int, List<Vector3>> coinZones;
    private Dictionary<int, List<ObstaclePosition>> obstacleZones;

    [SerializeField] private float recycleInterval = 0.5f;
    [SerializeField] private float recycleTimer = 0f;
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

    public void RemoveObstacle(ObstaclePosition obsPos)
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
                            new Vector2(obsPos.Position.x, obsPos.Position.z)
                            );
                        if(distance < distanceThresold)
                        {
                         
                            return false;
                        }
                    }
                    
                }
            }
            
        }
        return true;
    }

    public void UpdateWithCameraPosition(float cameraZ)
    {
        this.RemoveZone(cameraZ);
        this.RecycleObjectsByTime(cameraZ);
    }

    protected void RemoveZone(float cameraZ)
    {
        int zoneIndex = Mathf.FloorToInt(cameraZ / zoneSize);
        if(coinZones != null)
        {
            coinZones = coinZones.Where(pair => pair.Key >= zoneIndex).ToDictionary(pair => pair.Key, pair => pair.Value);
        }
        if(obstacleZones != null)
        {
            obstacleZones = obstacleZones.Where(pair => pair.Key >= zoneIndex).ToDictionary(pair => pair.Key, pair => pair.Value);
        }
       // Debug.Log("coin zone: " + coinZones.Count + " ob zone: " +  obstacleZones.Count);
    }

    protected void RecycleObjectsByTime(float cameraZ)
    {
        recycleTimer += Time.deltaTime;
        if (recycleTimer >= recycleInterval)
        {
            RecycleObject("Coin",cameraZ);
            for (int i = 0; i < GameSetting.Instance.ActiveObstacles.Count; i++)
            {
                RecycleObject(GameSetting.Instance.ActiveObstacles[i], cameraZ);
            }
            RecycleObject("Bullet", cameraZ);
            RecycleObject("Warning", cameraZ);
            recycleTimer = 0f;
        }
    }
    protected void RecycleObject(string tag, float cameraZ)
    {
        List<GameObject> activeObjects = ObjectPool.Instance.GetActiveObjects(tag);
        for (int i = activeObjects.Count - 1; i >= 0; i--)
        {
            Movement obstacleMovement = activeObjects[i].GetComponent<Movement>();
            if (activeObjects[i] != null && activeObjects[i].transform.position.z < cameraZ)
            {
                if (obstacleMovement != null) obstacleMovement.ResetMoving(); // khi thu về pool thì đặt lại trạng thái chuyển động
                ObjectPool.Instance.ReturnToPoolQuynh(tag, activeObjects[i]);
            }
        }
    }
}
