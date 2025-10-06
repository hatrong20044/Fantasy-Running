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

    private void Start()
    {
        lastSpawnZ = player.position.z;
        SpawnCoins(); // Spawn hàng coin đầu tiên
    }

    private void Update()
    {
        // Spawn hàng coin mới khi player di chuyển đủ khoảng cách
        if (player.position.z >= lastSpawnZ + spawnDistance)
        {
            SpawnCoins();
            lastSpawnZ = player.position.z;
        }
    }

    private void SpawnCoins()
    {
        // Spawn 5 coin trong một hàng, cách nhau 2m
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
}
