using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleAsset
{
    public string name;
    public GameObject prefab;
}

public class AssetCollector : MonoBehaviour
{
    public static AssetCollector instance;
    public List<ObstacleAsset> currentPassable; //Store Obstacles's Mesh of current season (map's season)
    public List<ObstacleAsset> currentNonPassable; //Store Obstacles's Mesh Renderer of current season (map's season)

    void Awake()
    {
        AssetCollector.instance = this;
        this.currentPassable = new List<ObstacleAsset>();
        this.currentNonPassable = new List<ObstacleAsset>();
    }

    public void LoadSeason(string season)
    {
        this.currentPassable.Clear();
        this.currentNonPassable.Clear();

        Transform seasonRoot = transform.Find(season);
        if (seasonRoot == null) return;

        Transform passable = seasonRoot.Find("Passable");
        Transform nonPassable = seasonRoot.Find("NonPassable");

        CollectMeshes(passable, this.currentPassable);
        CollectMeshes(nonPassable, this.currentNonPassable);
    }

    void CollectMeshes(Transform parent, List<ObstacleAsset> list)
    {
        if (parent == null) return;
        foreach (Transform child in parent)
        {
            MeshFilter filter = child.GetComponent<MeshFilter>();
            MeshRenderer renderer = child.GetComponent<MeshRenderer>();

            if (filter && renderer)
            {
                var asset = new ObstacleAsset
                {
                    name = child.name,
                    prefab = child.gameObject
                };
                list.Add(asset);
            }
        }
    }
}
