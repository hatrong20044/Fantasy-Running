using UnityEngine;
using System.Collections;

public class BossManager : MonoBehaviour
{
    [System.Serializable]
    public class BossSpawnInfo
    {
        public BossBase bossPrefab;
        public float triggerDistance; // Khoảng cách player cần đi để boss này xuất hiện
    }

    public BossSpawnInfo[] bossList;
    public Transform player;
    public float spawnHeight = 10f;
    public float followDistance = 50f; // 👈 boss xuất hiện phía trước mặt player
    public float descendSpeed = 3f;

    private int currentBossIndex = 0;
    private bool isSpawning = false;

    private PlayerProgress progress;

    void Start()
    {
        progress = player.GetComponent<PlayerProgress>();
    }

    void Update()
    {
        if (isSpawning || currentBossIndex >= bossList.Length) return;

        if (progress.DistanceTravelled >= bossList[currentBossIndex].triggerDistance)
        {
            StartCoroutine(SpawnBossRoutine(bossList[currentBossIndex]));
            currentBossIndex++;
        }
    }

    IEnumerator SpawnBossRoutine(BossSpawnInfo info)
    {
        isSpawning = true;

        // Tạo boss
        BossBase boss = Instantiate(info.bossPrefab);
        boss.Initialize(player.GetComponent<Player>());
        Debug.Log($"⚡ Spawn Boss: {boss.name} - Player: {player.name}");

        // ✅ Spawn TRƯỚC MẶT player trên trục Z
        Vector3 spawnPos = player.position + Vector3.forward * followDistance + Vector3.up * spawnHeight;
        boss.transform.position = spawnPos;

        // ✅ Di chuyển boss từ trên cao xuống vị trí trước mặt player
        Vector3 targetPos = player.position + Vector3.forward * followDistance;
        while (Vector3.Distance(boss.transform.position, targetPos) > 0.1f)
        {
            boss.transform.position = Vector3.MoveTowards(
                boss.transform.position,
                targetPos,
                descendSpeed * Time.deltaTime
            );
            yield return null;
        }

        boss.transform.LookAt(player); // 👈 quay mặt về phía người chơi
        boss.PerformBehavior();

        isSpawning = false;
    }
}
