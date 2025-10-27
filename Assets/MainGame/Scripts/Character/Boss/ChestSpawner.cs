using System.Collections;
using UnityEngine;

public class ChestSpawner : MonoBehaviour
{
    [Header("Pool Settings")]
    public string chestPoolTag = "Chest";

    [Header("Lane Settings")]
    public float laneSpacing = 2.5f;          // Khoảng cách giữa các lane (trái-giữa-phải)
    public float spawnDistance = 15f;         // Khoảng cách spawn phía trước player
    public float chestHeight = 0.5f;          // Độ cao spawn chest

    [Header("References")]
    public Transform player;

    private void Start()
    {
        // Tự động tìm player
        if (player == null)
        {
            Player playerComp = FindObjectOfType<Player>();
            if (playerComp != null)
            {
                player = playerComp.transform;
            }
        }

        if (player == null)
        {
            Debug.LogError("❌ Không tìm thấy Player!");
        }
    }

    /// <summary>
    /// Spawn 3 rương ngang hàng ở 3 lane
    /// GỌI TỪ ANIMATION EVENT khi Boss vung gậy
    /// </summary>
    public void SpawnChestWave()
    {
        if (player == null)
        {
            Debug.LogError("❌ Player reference null!");
            return;
        }

        if (ObjectPool.Instance == null)
        {
            Debug.LogError("❌ ObjectPool chưa khởi tạo!");
            return;
        }

        // Vị trí base: phía trước player
        Vector3 forwardPos = player.position + player.forward * spawnDistance;
        forwardPos.y = chestHeight;

        int spawnCount = 0;

        // Spawn 3 chest: Trái, Giữa, Phải
        // Lane Trái (offset -2.5)
        if (SpawnChest(forwardPos + player.right * -laneSpacing))
            spawnCount++;

        // Lane Giữa (offset 0)
        if (SpawnChest(forwardPos))
            spawnCount++;

        // Lane Phải (offset +2.5)
        if (SpawnChest(forwardPos + player.right * laneSpacing))
            spawnCount++;

        Debug.Log($"✅ Boss vung gậy! Spawn {spawnCount}/3 chest ở 3 lane");
    }

    private bool SpawnChest(Vector3 position)
    {
        GameObject chest = ObjectPool.Instance.GetFromPoolQuynh(chestPoolTag);

        if (chest != null)
        {
            chest.transform.position = position;
            chest.transform.rotation = Quaternion.identity;

            // Reset chest nếu có script
            Chest chestScript = chest.GetComponent<Chest>();
            if (chestScript != null)
            {
                chestScript.ResetChest();
            }

            return true;
        }
        else
        {
            Debug.LogWarning($"⚠️ Pool '{chestPoolTag}' hết chest!");
            return false;
        }
    }

    // Debug: Test spawn bằng phím G
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            Debug.Log("🧪 TEST: Spawn 3 chest!");
            SpawnChestWave();
        }
    }
}