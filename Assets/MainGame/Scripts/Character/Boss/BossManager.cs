using UnityEngine;
using System.Collections;

public class BossManager : MonoBehaviour
{
    [System.Serializable]
    public class BossSpawnInfo
    {
        public BossBase bossPrefab;
        public float triggerDistance; 
    }

    public BossSpawnInfo[] bossList;
    public Transform player;
    public float spawnHeight = 10f;
    public float followDistance = 50f; 
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

      
        BossBase boss = Instantiate(info.bossPrefab);
        boss.Initialize(player.GetComponent<Player>());
        Debug.Log($"⚡ Spawn Boss: {boss.name} - Player: {player.name}");

        Vector3 spawnPos = player.position + Vector3.forward * followDistance + Vector3.up * spawnHeight;
        boss.transform.position = spawnPos;
      
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

        boss.transform.LookAt(player); 
        boss.PerformBehavior();

        isSpawning = false;
    }
}
