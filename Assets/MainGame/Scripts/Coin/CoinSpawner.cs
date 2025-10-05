using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinSpawner : MonoBehaviour
{
    public GameObject coinPrefab; // Tham chiếu đến Prefab coin
    private Vector3 targetScale = new Vector3(0.5f, 0.04f, 0.5f); // Kích thước của cylinder

    void Start()
    {
        // Tạo instance của Prefab coin
        GameObject coinInstance = Instantiate(coinPrefab, transform.position, Quaternion.identity);

        // Đặt scale của instance giống với cylinder
        coinInstance.transform.localScale = targetScale;
    }
}
