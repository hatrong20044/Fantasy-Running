using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MapInfo
{
    public string tag;        // Tag của pool (vd: "winter_1", "winter_2")
    public float length;      // Chiều dài của map
}

[System.Serializable]
public class Theme
{
    public string name;       // Tên chủ đề (vd: "Winter", "Sea")
    public List<MapInfo> maps;
    public int spawnPerTheme = 10; // Số lần spawn random trong chủ đề này trước khi chuyển
}

public class GroundSpawner : MonoBehaviour
{
    public List<Theme> themes;
    private int currentThemeIndex = 0;
    private int spawnCounter = 0;
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
        if (themes.Count == 0) return;

        Theme currentTheme = themes[currentThemeIndex];

        if (currentTheme.maps.Count == 0) return;

        // Random chọn một map từ list maps của chủ đề hiện tại
        int randomIndex = Random.Range(0, currentTheme.maps.Count);
        MapInfo currentMap = currentTheme.maps[randomIndex];

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

        // Đếm số lần spawn trong chủ đề hiện tại
        spawnCounter++;

        // Nếu đủ số lần thì chuyển chủ đề
        if (spawnCounter >= currentTheme.spawnPerTheme)
        {
            spawnCounter = 0;
            currentThemeIndex = (currentThemeIndex + 1) % themes.Count;
        }
    }
}