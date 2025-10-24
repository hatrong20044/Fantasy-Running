using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BossManager : MonoBehaviour
{
    [System.Serializable]
    public class BossSpawnInfo
    {
        public BossBase bossPrefab;
        [Tooltip("Khoang cach player can chay")]
        public float triggerDistance = 100f;
        [Tooltip("Delay")]
        public float spawnDelay = 0f;
        [Tooltip("Thời gian boss tồn tại)")]
        public float activeTime = 5f;
    }

    [Header("Config")]
    public List<BossSpawnInfo> bossList = new List<BossSpawnInfo>();
    public Transform player;

    private int currentIndex = 0;
    private bool isSpawningOrActive = false;
    private PlayerProgress progress;
    private BossBase currentBoss;

    private void Start()
    {
        if (player != null)
            progress = player.GetComponent<PlayerProgress>();
    }

    private void Update()
    {
        if (isSpawningOrActive || progress == null || currentIndex >= bossList.Count)
            return;

        BossSpawnInfo info = bossList[currentIndex];
        float dist = progress.GetDistance();

        if (dist >= info.triggerDistance)
            StartCoroutine(SpawnBossRoutine(info));
    }

    private IEnumerator SpawnBossRoutine(BossSpawnInfo info)
    {
        isSpawningOrActive = true;

        if (info.spawnDelay > 0f)
            yield return new WaitForSeconds(info.spawnDelay);

        currentBoss = Instantiate(info.bossPrefab);
        currentBoss.Init(player);

        currentBoss.OnBossFinished += OnBossFinished;
        currentBoss.Activate();

        if (info.activeTime > 0f)
        {
            yield return new WaitForSeconds(info.activeTime);
            if (currentBoss != null) currentBoss.EndBoss();
        }
    }

    private void OnBossFinished(BossBase boss)
    {
        if (currentBoss != null)
        {
            currentBoss.OnBossFinished -= OnBossFinished;
            currentBoss = null;
        }

        currentIndex++;
        isSpawningOrActive = false;
    }

    public void ForceEndCurrentBoss()
    {
        if (currentBoss != null)
            currentBoss.EndBoss();
    }
}
