using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class GameData : MonoBehaviour
{
    public static GameData Instance;

    [Header("Coin")]
    [SerializeField] private int totalCoins = 0;
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
        totalCoins = PlayerPrefs.GetInt(GameSetting.COINS_KEY, 0);
    }
    private void Reset()
    {
        PlayerPrefs.SetInt(GameSetting.COINS_KEY, totalCoins);
    }
    public int ToTalCoins
    {
        get => this.totalCoins;
        set
        {
            totalCoins = value;
            PlayerPrefs.SetInt(GameSetting.COINS_KEY, totalCoins);
            PlayerPrefs.Save();

        }
    }
}
