
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinSpawner : MonoBehaviour
{
    [SerializeField] private Transform player; // Reference đến player
    [SerializeField] private Vector3 startPosition; // Vị trí bắt đầu spawn coin (lấy từ Hierarchy)
    [SerializeField] private float coinSpacing = 2f; // Khoảng cách giữa các coin (2m theo Z)
    [SerializeField] private float spawnDistance = 20f; // Khoảng cách giữa các hàng coin
    private float lastSpawnZ; // Vị trí Z của lần spawn trước
    public int dir = 0; // 1: hàng dọc, 2: hàng ngang

    private void Start()
    {
        lastSpawnZ = player.position.z;
        dir = Random.Range(1, 3); // Random ban đầu: 1 hoặc 2
        SpawnCoins(); // Spawn hàng coin đầu tiên
    }

    private void Update()
    {
        // Spawn hàng coin mới khi player di chuyển đủ khoảng cách
        if (player.position.z >= lastSpawnZ + spawnDistance)
        {
            SpawnCoins();
            lastSpawnZ = player.position.z;
            dir = Random.Range(1, 3); // Random lại dir sau mỗi dải
        }
    }

    private void SpawnCoins()
    {
        if (dir == 1)
        {
            // Spawn hàng dọc: 5 coin cách nhau 2m theo Z
            for (int i = 0; i < 5; i++)
            {
                Vector3 coinPosition = new Vector3(
                    startPosition.x,
                    startPosition.y,
                    startPosition.z + player.position.z + (i * coinSpacing)
                );
                GameObject coin = ObjectPool.Instance.GetObjectFromPools("Coin");
                if (coin != null)
                {
                    coin.transform.SetPositionAndRotation(coinPosition, Quaternion.identity);
                }
            }
        }
        else if (dir == 2)
        {
            // Spawn hàng ngang: 3 coin trên các làn (x = -2, 0, 2) tại cùng Z
            float[] lanes = { -2f, 0f, 2f }; // Vị trí x cho các làn: trái, giữa, phải
            foreach (float laneX in lanes)
            {
                Vector3 coinPosition = new Vector3(
                    laneX,
                    startPosition.y,
                    startPosition.z + player.position.z
                );
                GameObject coin = ObjectPool.Instance.GetObjectFromPools("Coin");
                if (coin != null)
                {
                    coin.transform.SetPositionAndRotation(coinPosition, Quaternion.identity);
                }
            }
        }
    }
}