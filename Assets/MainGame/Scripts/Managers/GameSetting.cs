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

    [Header("Slide Settings")]
    public float slideDuration = 1f;
    [Tooltip("Chiều cao khi slide (cả Controller và visual Collider)")]
    public float slideHeight = 1.4f;
    [Tooltip("Center Y khi slide")]
    public float slideCenter = -0.4f;

    [Header("Coin Pattern Settings")]
    [SerializeField] private float coinSpacing = 2f; // Khoảng cách giữa các coin trong pattern
   // [SerializeField] private int minVerticalCoins = 3; 
   // [SerializeField] private int maxVerticalCoins = 5;

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
   // public int MinVerticalCoins => minVerticalCoins;
   // public int MaxVerticalCoins => maxVerticalCoins;

    public float SlideDuration => slideDuration;
    public float SlideHeight => slideHeight;
    public float SlideCenter => slideCenter;
}
