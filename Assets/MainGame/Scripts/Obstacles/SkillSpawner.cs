using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillSpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    public static SkillSpawner Instance;
    public float laneDistance = 2.5f; // Lấy từ Player.laneDistance
    public float minSpawnDistanceFirstPhase = 200f; // 3 item đầu: 50-75 mét
    public float maxSpawnDistanceFirstPhase = 250f;
    public float minSpawnDistanceSecondPhase = 350f; // 2 item sau: 100-125 mét
    public float maxSpawnDistanceSecondPhase = 400f;
    public int firstPhaseCount = 3; // Số item ở giai đoạn đầu
    public int secondPhaseCount = 2; // Số item ở giai đoạn sau

    private float[] lanePositions; // Vị trí X của các lane
    private Transform player;
    private float currentSpawnZ; // Vị trí Z của skill hiện tại
    private float currentResetZ;
    private int itemsUsed;

    private void Awake()
    {
        Instance = this;
        player = GameObject.FindGameObjectWithTag("Player").transform;
        lanePositions = new float[] { -laneDistance, 0, laneDistance };
        itemsUsed = 0;
    }

    private void SpawnItem()
    { 
        // Chọn ngẫu nhiên một lane
        float xPos = lanePositions[Random.Range(0, lanePositions.Length)];
        GameObject skill = ObjectPool.Instance.GetFromPoolQuynh("Skill");
        Debug.Log(currentResetZ + " " + currentSpawnZ);
        // Tính toán khoảng cách cho item tiếp theo
        if (itemsUsed < firstPhaseCount)
        {
            float range = Random.Range(minSpawnDistanceFirstPhase, maxSpawnDistanceFirstPhase);
            currentSpawnZ += range;
            currentResetZ += range;
        }
        else if (itemsUsed < firstPhaseCount + secondPhaseCount)
        {
            float range = Random.Range(minSpawnDistanceSecondPhase, maxSpawnDistanceSecondPhase);
            currentSpawnZ += range;
            currentResetZ += range;
        }
        skill.transform.position = new Vector3(xPos, skill.transform.position.y, currentSpawnZ);
    }

    public void increaseItemUsed()
    {
        itemsUsed++;
    }

    public void setCurrentSpawnZ(float z)
    {
        this.currentSpawnZ = z;
    }

    public void ResetSkill(float CameraZ)
    {
        if (CameraZ > this.currentResetZ)
        {
            SpawnItem();
        }
    }
}
