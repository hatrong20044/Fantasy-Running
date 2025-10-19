using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MapInfo
{
    public string tag; // Tag của pool (vd: "winter_1", "winter_2")
}

[System.Serializable]
public class Theme
{
    public string name; // Tên chủ đề (vd: "Winter", "Sea")
    public List<MapInfo> maps;
    public int spawnPerTheme = 10; // Số lần spawn random trong chủ đề này trước khi chuyển
}

public class GroundSpawner : MonoBehaviour
{
    public List<Theme> themes;
    public GameObject startMap; // Prefab hoặc GameObject cho startMap
    private int currentThemeIndex = 0;
    private int spawnCounter = 0;
    private float nextSpawnZ = 0f; // Chỉ lưu giá trị Z cho điểm spawn tiếp theo
    private GameObject lastSpawnedTile; // Lưu map cuối cùng được spawn

    void Start()
    {
        // Đặt vị trí spawner tại (0, 0, 0)
        transform.position = Vector3.zero;

        // Spawn startMap
        if (startMap != null)
        {
            // Khởi tạo startMap tại (0, 0, 0)
            GameObject startTile = Instantiate(startMap, Vector3.zero, Quaternion.identity);

            // Tìm StartPoint và EndPoint trong startMap
            Transform startPoint = startTile.transform.Find("StartPoint");
            Transform endPoint = startTile.transform.Find("EndPoint");

            if (startPoint == null || endPoint == null)
            {
                Debug.LogError("startMap thiếu StartPoint hoặc EndPoint!");
                return;
            }

            // Đặt startMap sao cho StartPoint tại (0, 0, 0)
            float startPointZ = startPoint.localPosition.z;
            startTile.transform.position = new Vector3(0f, 0f, -startPointZ);

            // Cập nhật nextSpawnZ dựa trên EndPoint của startMap
            nextSpawnZ = endPoint.position.z;

            // Lưu startMap làm lastSpawnedTile
            lastSpawnedTile = startTile;

            // Xử lý EndTrigger cho startMap (nếu có)
            Transform endTrigger = startTile.transform.Find("EndTrigger");
            if (endTrigger != null)
            {
                Collider col = endTrigger.GetComponent<Collider>();
                if (col != null)
                {
                    col.isTrigger = true;
                    EndTriggerHandler handler = col.gameObject.AddComponent<EndTriggerHandler>();
                    handler.Init(this, "startMap", startTile);
                }
            }
        }
        else
        {
            Debug.LogError("startMap chưa được gán trong Inspector!");
        }

        // Spawn thêm vài map để test
        for (int i = 0; i < 4; i++) // Spawn 4 map vì startMap đã chiếm 1 slot
        {
            SpawnTile();
        }
    }

    public void SpawnTile()
    {
        if (themes.Count == 0 || themes[currentThemeIndex].maps.Count == 0) return;

        Theme currentTheme = themes[currentThemeIndex];
        int randomIndex = Random.Range(0, currentTheme.maps.Count);
        MapInfo currentMap = currentTheme.maps[randomIndex];

        // Lấy từ pool
        GameObject tile = ObjectPool.Instance.GetFromPool(currentMap.tag);
        if (tile == null) return;

        // Tìm StartPoint và EndPoint trong prefab map
        Transform startPoint = tile.transform.Find("StartPoint");
        Transform endPoint = tile.transform.Find("EndPoint");

        if (startPoint == null || endPoint == null)
        {
            Debug.LogError($"Map {currentMap.tag} thiếu StartPoint hoặc EndPoint!");
            return;
        }

        // Tính toán vị trí spawn dựa trên tọa độ Z
        float startPointZ = startPoint.localPosition.z; // Z của StartPoint so với gốc prefab
        float offsetZ = nextSpawnZ - startPointZ; // Khoảng cách Z cần dịch chuyển
        tile.transform.position = new Vector3(0f, 0f, offsetZ); // Chỉ thay đổi Z, X và Y = 0
        tile.transform.rotation = Quaternion.identity; // Xoay mặc định

        // Cập nhật nextSpawnZ dựa trên Z của EndPoint
        nextSpawnZ = endPoint.position.z;

        // Lưu map vừa spawn
        lastSpawnedTile = tile;

        // Xử lý EndTrigger
        Transform endTrigger = tile.transform.Find("EndTrigger");
        if (endTrigger != null)
        {
            Collider col = endTrigger.GetComponent<Collider>();
            if (col != null)
            {
                col.isTrigger = true;
                EndTriggerHandler handler = col.gameObject.AddComponent<EndTriggerHandler>();
                handler.Init(this, currentMap.tag, tile);
            }
        }

        // Đếm số lần spawn trong chủ đề hiện tại
        spawnCounter++;
        if (spawnCounter >= currentTheme.spawnPerTheme)
        {
            spawnCounter = 0;
            currentThemeIndex = (currentThemeIndex + 1) % themes.Count;
        }
    }
}

