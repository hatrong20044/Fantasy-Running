using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class GameSetting : MonoBehaviour
{
    public static GameSetting Instance { get; private set; }

    // Thông số chung của game
    [Header("Movement Settings")]
    [SerializeField] private float forwardSpeed = 5f;
    [SerializeField] private float laneDistance = 2.5f;
    [SerializeField] private float laneChangeSpeed = 10f;
    [SerializeField] private float jumpForce = 8f;
    [SerializeField] private float gravity = -20f; 
    [SerializeField] private float jumpForwardBoost = 2f;
    [SerializeField] private List<string> activeObstacles = new List<string>(){"Obstacle", "Obstacle1", "Obstacle2", "Obstacle3", "Obstacle4"};
    public const string COINS_KEY = "Coins";
    public const string NEXT_REWARD_INDEX = "Next_Reward_Index";
    public const string TIMEDATE_REWARD_DELAY = "TimeDate_Reward_Delay";
    public const string MUSIC_VOLUME = "Music_Volume";
    public const string SFX_VOLUME = "Sfx_Volume";
    public const string USER_NAME = "User_Name";

    [Header("Slide Settings")]
    public float slideDuration = 1f;
    [Tooltip("Chiều cao khi slide (cả Controller và visual Collider)")]
    public float slideHeight = 1.4f;
    [Tooltip("Center Y khi slide")]
    public float slideCenter = -0.4f;

    [Header("Coin Pattern Settings")]
    [SerializeField] private float coinSpacing = 2f; // Khoảng cách giữa các coin trong pattern
                                                     
    private void Awake()
    {
        // Thiết lập Singleton
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject); // Hủy nếu đã có instance khác
        }
    }

    public float ForwardSpeed => forwardSpeed;
    public float LaneDistance => laneDistance;
    public float LaneChangeSpeed => laneChangeSpeed;
    public float JumpForce => jumpForce;
    public float JumpForwardBoost => jumpForwardBoost;
    public float Gravity => gravity;
    public float JumpForwarBoot => jumpForwardBoost;
    public float CoinSpacing => coinSpacing;
    public float SlideDuration => slideDuration;
    public float SlideHeight => slideHeight;
    public float SlideCenter => slideCenter;
    public List<string> ActiveObstacles => this.activeObstacles;

   
}
