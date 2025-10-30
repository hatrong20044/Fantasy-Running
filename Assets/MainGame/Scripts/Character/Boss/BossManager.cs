using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class BossSpawnInfo
{
    public BossBase bossPrefab;
    [Tooltip("Khoang cach player can chay")]
    public float nextBossDistance = 750f;
    [Tooltip("Delay")]
    public float spawnDelay = 0f;
    [Tooltip("Thời gian boss tồn tại)")]
    public float activeTime = 5f;
    public bool persistUntilDefeated = false; // Cờ mới để xác định boss không có thời gian giới hạn
}

public class BossManager : MonoBehaviour
{
    [Header("Config")]
    public List<BossSpawnInfo> bossList = new List<BossSpawnInfo>();
    public Transform player;
    public BossSpawnInfo nextBoss { get; private set; } // Biến để theo dõi boss tiếp theo

    public bool isFirstBoss = true;
    public float triggerDistance = 100f;
    public static BossManager instance;
    private int currentIndex = 0;
    public bool isSpawningOrActive = false;
    private PlayerProgress progress;
    public BossBase currentBoss;

    private void Awake()
    {
        instance = this;
        if (player != null)
            progress = player.GetComponent<PlayerProgress>();

        UpdateNextBoss();
    }

    private void Start()
    {
        
    }

    private void Update()
    {
        if (isSpawningOrActive || progress == null || currentIndex >= bossList.Count)
            return;

        BossSpawnInfo info = bossList[currentIndex];
        float dist = player.transform.position.z;

        if (dist >= triggerDistance)
            StartCoroutine(SpawnBossRoutine(info));
    }

    private IEnumerator SpawnBossRoutine(BossSpawnInfo info)
    {   
        if (info.spawnDelay > 0f)
            yield return new WaitForSeconds(info.spawnDelay);
        isSpawningOrActive = true;
        currentBoss = Instantiate(info.bossPrefab);
        currentBoss.Init(player);

        currentBoss.OnBossFinished += OnBossFinished;
        currentBoss.Activate();
        if (!info.persistUntilDefeated && info.activeTime > 0f)
        {
            yield return new WaitForSeconds(info.activeTime);
            if (currentBoss != null) currentBoss.EndBoss();
        }
    }

    public void OnBossFinished(BossBase boss)
    {
        if (currentBoss != null)
        {
            currentBoss.OnBossFinished -= OnBossFinished;
            currentBoss = null;
        }

        currentIndex++;
        if (currentIndex >= bossList.Count)
        {
            currentIndex = 0; // Quay lại đầu danh sách nếu hết
        }

        isSpawningOrActive = false;
        UpdateNextBoss(); // Cập nhật nextBoss sau khi boss hiện tại kết thúc
    }

    public void ForceEndCurrentBoss()
    {
        if (currentBoss != null)
            currentBoss.EndBoss();
    }

    private void UpdateNextBoss()
    {
        if (bossList.Count == 0)
        {
            nextBoss = null; // Nếu danh sách rỗng, đặt nextBoss là null
        }
        else
        {
            // Nếu currentIndex vượt quá danh sách, quay lại đầu
            int nextIndex = currentIndex >= bossList.Count ? 0 : currentIndex;
            nextBoss = bossList[nextIndex]; // Gán nextBoss
            if (nextBoss != null)
            {
                if (isFirstBoss)
                {
                    SkillSpawner.Instance.setZ(triggerDistance);
                    isFirstBoss = false;
                    return;
                }
                float dist = player.transform.position.z;
                triggerDistance = bossList[currentIndex].nextBossDistance + dist;
                Debug.Log(triggerDistance);
                SkillSpawner.Instance.setZ(triggerDistance);
            }
        }
    }
}
