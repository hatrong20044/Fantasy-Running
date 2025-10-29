using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class GameData : MonoBehaviour
{
    public static GameData Instance;

    [Header("Coin")]
    [SerializeField] private int coins;
   

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
        this.LoadData();
    }
    private void LoadData()
    {
        coins = PlayerPrefs.GetInt(GameSetting.COINS_KEY, 0);
    }
    private void Reset()
    {
        PlayerPrefs.SetInt(GameSetting.COINS_KEY,0);
    }
    public int Coins
    {
        get => this.coins;
        set
        {
            coins = value;
            PlayerPrefs.SetInt(GameSetting.COINS_KEY, coins);
            PlayerPrefs.Save();

        }
    }
}
