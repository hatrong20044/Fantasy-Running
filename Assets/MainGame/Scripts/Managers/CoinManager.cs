
using UnityEngine;
using UnityEngine.Rendering;
using static System.Net.Mime.MediaTypeNames;

public class CoinManager : MonoBehaviour
{
    public static CoinManager Instance;
    [SerializeField] private int coin = 0;
    [SerializeField] GameObject coinDisplay;
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
        CoinEvent.Instance.OnCoinCollected += HandleCoinCollected;
    }

    private void OnDestroy()
    {
        // Hủy đăng kí sự kiện
        CoinEvent.Instance.OnCoinCollected -= HandleCoinCollected;
    }
    public void AddCoin(int mount = 1)
    {
        coin += mount;
       
    }
    public void DisplayCoin()
    {
        coinDisplay.GetComponent<TMPro.TMP_Text>().text = "Coin: " + coin;
    }


    public void HandleCoinCollected(GameObject coin)
    {
        this.AddCoin();
        this.DisplayCoin();
    }

}