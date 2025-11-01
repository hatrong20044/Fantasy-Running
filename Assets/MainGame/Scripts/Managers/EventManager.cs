using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    public static EventManager Instance;
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

    public event Action<GameObject> OnPlayerCollided;
    public void CoinCollected(GameObject coin)
    {
        OnCoinCollected?.Invoke(coin);
    }

    public void PlayerCollided(GameObject player)
    {
        OnPlayerCollided?.Invoke(player);
    }

    
}
