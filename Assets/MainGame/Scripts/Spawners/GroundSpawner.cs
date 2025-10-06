using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MapInfo
{
    public string tag;        // Tag của pool (vd: "Map1", "Map2")
    public float length;      // Chiều dài của map
}

public class GroundSpawner : MonoBehaviour
{
    public List<MapInfo> maps;
    private int currentMapIndex = 0;
    private int spawnCounter = 0;

    public int spawnPerMap = 10; // cố định 10 lần mỗi map
    private Vector3 nextSpawnPoint = Vector3.zero;

    void Start()
    {
        // Spawn thử vài tile đầu tiên để test
        for (int i = 0; i < 5; i++)
        {
            SpawnTile();
        }
    }

    public void SpawnTile()
    {
        if (maps.Count == 0) return;

        MapInfo currentMap = maps[currentMapIndex];

        // Lấy từ pool
        GameObject tile = ObjectPool.Instance.GetFromPool(currentMap.tag);
        if (tile == null) return;

        // Đặt vị trí
        tile.transform.position = nextSpawnPoint;
        tile.transform.rotation = Quaternion.identity;

        // Tìm EndTrigger trong prefab map
        Transform endTrigger = tile.transform.Find("EndTrigger");
        if (endTrigger != null)
        {
            Collider col = endTrigger.GetComponent<Collider>();
            if (col != null)
            {
                col.isTrigger = true;

                // Gắn handler tạm thời
                EndTriggerHandler handler = col.gameObject.AddComponent<EndTriggerHandler>();
                handler.Init(this, currentMap.tag, tile);
            }
        }

        // Cập nhật điểm spawn tiếp theo dựa vào chiều dài map
        nextSpawnPoint += Vector3.forward * currentMap.length;

        // Đếm số lần spawn map hiện tại
        spawnCounter++;

        // Nếu đủ 10 lần thì chuyển map
        if (spawnCounter >= spawnPerMap)
        {
            spawnCounter = 0;
            currentMapIndex++;

            // Nếu hết map thì quay lại map đầu
            if (currentMapIndex >= maps.Count)
                currentMapIndex = 0;
        }
    }
}
