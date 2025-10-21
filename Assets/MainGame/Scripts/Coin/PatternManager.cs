using System.Collections.Generic;
using UnityEngine;

public class PatternManager : MonoBehaviour
{
    [System.Serializable]
    public class CoinPattern
    {
        public string namePattern;
        public Vector3[] positions;
    }

    [SerializeField] private List<CoinPattern> patterns;
    [SerializeField] private float forwardSpeed; // tốc độ player
    [SerializeField] private float laneDistance;
    [SerializeField] private float jumpForce; // nhảy cao
    [SerializeField] private float gravity;
    [SerializeField] private float jumpForwardBoost; // nhảy xa
    [SerializeField] private float coinSpacing;
    [SerializeField] private int maxVertical = 5; 
    [SerializeField] private int minVertical = 3;
    private void Awake()
    {
        this.InitializePatterns();

    }
    private void Reset()
    {
        this.Init();
    }
    private void Init()
    {
        laneDistance = GameSetting.Instance.LaneDistance;
        coinSpacing = GameSetting.Instance.CoinSpacing;
        jumpForce = GameSetting.Instance.JumpForce;
        gravity = GameSetting.Instance.Gravity;
        jumpForwardBoost = GameSetting.Instance.JumpForwardBoost;
        forwardSpeed = GameSetting.Instance.ForwardSpeed;
    }

    private void InitializePatterns()
    {
        if (patterns.Count == 0)
        {
            patterns = new List<CoinPattern>();
            CreateVerticalPos();
            CreateHorizontalPos();
            CreateParabolaPos();
        }
    }

    public Vector3[] VerticalPosition(int index)
    {
        Vector3[] pos = new Vector3[index];
        for(int i = 0; i < index; i++)
        {
            pos[i] = new Vector3(0,1, i * coinSpacing);
        }
        return pos;
    }

    public void CreateVerticalPos()
    {
        for (int i = minVertical; i <= maxVertical; i++)
        {
            string namePattern = "Vertical" + i;
            Vector3[] pos = VerticalPosition(i);
            patterns.Add(new CoinPattern { namePattern = namePattern, positions = pos });
        }    
    }

    public void CreateHorizontalPos()
    {
        patterns.Add(new CoinPattern
        {
            namePattern = "Horizontal1",
            positions = new Vector3[]
                    {
                        new Vector3(-laneDistance, 1, 0),
                        new Vector3(0,1,0),
                        new Vector3(laneDistance,1,0)
                    }

        });

        patterns.Add(new CoinPattern
        {
            namePattern = "Horizontal2",
            positions = new Vector3[]
                    {
                        new Vector3(-laneDistance, 1, 0),
                        new Vector3(-laneDistance + 1, 1, 0),
                        new Vector3(0,1,0),
                        new Vector3(1.5f,1,0),
                        new Vector3(laneDistance,1,0)
                    }

        });
    }
    public void CreateParabolaPos()
    {
        patterns.Add(new CoinPattern
        {
            namePattern = "Parabola5",
            positions = CalculateParabolaPositions(6, false)
        });
    }

    // Tính tọa độ coin theo quỹ đạo parabol khớp với nhảy của player
    private Vector3[] CalculateParabolaPositions(int coinCount, bool zigzagLanes = false)
    {
        Vector3[] pos = new Vector3[coinCount];

        // Tính thời gian nhảy
        float timeToPeak = -jumpForce / gravity; // Thời gian đến đỉnh
        float totalJumpTime = 2f * timeToPeak; // Tổng thời gian nhảy (lên + xuống)
        float timeStep = totalJumpTime / (coinCount - 1); // Khoảng thời gian giữa các coin
        float forwardSpeedWithBoost = forwardSpeed + jumpForwardBoost; // Tốc độ Z khi nhảy

        for (int i = 0; i < coinCount; i++)
        {
            float t = i * timeStep; // Thời gian tại mỗi coin

            // Tính Y theo quỹ đạo parabol: y = y0 + v0*t + (1/2)*g*t^2
            float y = 1f + jumpForce * t + 0.5f * gravity * t * t;

            // Tính Z theo tốc độ chạy khi nhảy
            float z = forwardSpeedWithBoost * t;

            // Tính X: Cố định X = 0 hoặc xen kẽ các lane (-laneDistance, 0, laneDistance)
            float x = zigzagLanes ? (i % 3 == 0 ? -laneDistance : i % 3 == 1 ? 0 : laneDistance) : 0;

            pos[i] = new Vector3(x, y, z);
        }

        return pos;
    }

    public CoinPattern GetRandomPattern()
    {
        return patterns[Random.Range(0, patterns.Count)];
    }
}