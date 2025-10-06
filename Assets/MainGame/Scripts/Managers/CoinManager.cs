
using UnityEngine;

public class CoinManager : MonoBehaviour
{
    public static CoinManager Instance;
    [SerializeField] private int coin = 0;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    public void AddCoin(int mount = 1)
    {
        coin += mount;
        Debug.Log("Coin: " + coin);
        // Save game;
    }

    public int GetCoin()
    {
        return coin;
    }
}