using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class GameData : MonoBehaviour
{
    public static GameData Instance;

    [Header("Coin")]
    [SerializeField] private int totalCoins = 15000;

    [Header("UserName")]
    [SerializeField] private string userName = "You";
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
        userName = PlayerPrefs.GetString(GameSetting.USER_NAME);
    }
    private void Reset()
    {
        PlayerPrefs.SetInt(GameSetting.COINS_KEY, totalCoins);
        PlayerPrefs.SetString(GameSetting.USER_NAME, userName);
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

    public string UserName
    {
        get => this.userName;
        set
        {
            userName = value;
            PlayerPrefs.SetString (GameSetting.USER_NAME, userName);
            PlayerPrefs.Save();
        }

    }


}
