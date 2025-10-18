
using UnityEngine;
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
       
    }
    public void DisPlayCoin()
    {
        coinDisplay.GetComponent<TMPro.TMP_Text>().text = "Coin: " + coin;
    }
    public int GetCoin()
    {
        return coin;
    }
}