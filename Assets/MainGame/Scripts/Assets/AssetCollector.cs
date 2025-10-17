using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleAsset
{
    public string name;
    public GameObject prefab;
    public ObstacleType.ObstacleSubType subType;
}

public class AssetCollector : MonoBehaviour
{
    public static AssetCollector instance;
    private Dictionary<ObstacleType.ObstacleSubType, List<ObstacleAsset>> passableObstacles = new Dictionary<ObstacleType.ObstacleSubType, List<ObstacleAsset>>();
    private Dictionary<ObstacleType.ObstacleSubType, List<ObstacleAsset>> nonPassableObstacles = new Dictionary<ObstacleType.ObstacleSubType, List<ObstacleAsset>>();

    void Awake()
    {
        AssetCollector.instance = this;
    }

    public void LoadSeason(string season)
    {
        this.passableObstacles.Clear();
        this.nonPassableObstacles.Clear();

        // Khởi tạo các key trong Dictionary để tránh lỗi null
        foreach (ObstacleType.ObstacleSubType subType in System.Enum.GetValues(typeof(ObstacleType.ObstacleSubType)))
        {
            passableObstacles[subType] = new List<ObstacleAsset>();
            nonPassableObstacles[subType] = new List<ObstacleAsset>();
        }

        Transform seasonRoot = transform.Find(season);
        if (seasonRoot == null) return;

        Transform passable = seasonRoot.Find("Passable");
        Transform nonPassable = seasonRoot.Find("NonPassable");

        CollectMeshes(passable, true);
        CollectMeshes(nonPassable, false);
    }

    void CollectMeshes(Transform parent, bool isPassable)
    {
        if (parent == null) return;
        foreach (Transform child in parent)
        {
            MeshFilter filter = child.GetComponent<MeshFilter>();
            MeshRenderer renderer = child.GetComponent<MeshRenderer>();
            ObstacleType obstacleType = child.GetComponent<ObstacleType>();

            if (filter && renderer && obstacleType)
            {
                var asset = new ObstacleAsset
                {
                    name = child.name,
                    prefab = child.gameObject,
                    subType = obstacleType.subType
                };
                if (isPassable) 
                    this.passableObstacles[asset.subType].Add(asset);
                else 
                    this.nonPassableObstacles[asset.subType].Add(asset);
            }
        }
    }

    public List<ObstacleAsset> GetAssetsBySubType(bool isPassable, ObstacleType.ObstacleSubType subType)
    {
        return isPassable ? this.passableObstacles[subType] : this.nonPassableObstacles[subType];
    }
}
