using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinEvent : MonoBehaviour
{
    public static CoinEvent Instance;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public event Action<GameObject> OnCoinCollected;

    public void CoinCollected(GameObject coin)
    {
        OnCoinCollected?.Invoke(coin);
    }
}
