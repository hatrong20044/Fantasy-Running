
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Rendering;
using static System.Net.Mime.MediaTypeNames;

public class CoinManager : MonoBehaviour
{
    public static CoinManager Instance;
    [SerializeField] private int gamePlayCoins = 0;
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

    private void Start()
    {
        // Đăng kí sự kiện khi thu thập coin
        EventManager.Instance.OnCoinCollected += HandleCoinCollected;
    }

    private void OnDestroy()
    {
        // Hủy đăng kí sự kiện
        EventManager.Instance.OnCoinCollected -= HandleCoinCollected;
    }
    public void AddCoin(int mount = 1)
    {
        gamePlayCoins += mount;
       
    }

    public void HandleCoinCollected(GameObject coin)
    {
        this.AddCoin();
    }

    public int Coins => gamePlayCoins;

}