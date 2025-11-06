using UnityEngine;
using TMPro;
using System.Collections;

public class Chest : MonoBehaviour
{
    [Header("Pool Settings")]
    public string poolTag = "Chest";

    [Header("UI Components")]
    public TMP_Text answerText;

    [Header("Lifetime")]
    public float autoReturnDelay = 10f;

    [Header("Answer Data (Runtime - Không edit)")]
    public int answerIndex = 0;
    public bool isCorrectAnswer = false;
    private string answerContent = "";

    private float spawnTime;
    private bool hasBeenSelected = false;
    private static bool isAnyChestSelected = false;

    [Header("Coin Reward")]
    [SerializeField] private GetCoins getCoins;

    // 🔧 FIX: Public method để reset flag
    public static void ResetSelectionFlag()
    {
        isAnyChestSelected = false;
        Debug.Log("🔄 Reset chest selection flag");
    }

    private void OnEnable()
    {
        ResetChest();
    }

    public void ResetChest()
    {
        spawnTime = Time.time;
        answerIndex = 0;
        isCorrectAnswer = false;
        answerContent = "";
        hasBeenSelected = false;

        // 🔧 FIX: Bật lại collider khi reset
        Collider col = GetComponent<Collider>();
        if (col != null)
            col.enabled = true;

        if (answerText != null)
            answerText.text = "";
    }

    public void SetupAnswer(int index, string content, bool correct)
    {
        answerIndex = index;
        answerContent = content;
        isCorrectAnswer = correct;

        if (answerText != null)
            answerText.text = content;

        Debug.Log($"Chest setup: Lane={GetLaneName()} | Answer={content} | Correct={correct}");
    }

    private void Update()
    {
        if (Time.time - spawnTime > autoReturnDelay)
            ReturnToPool();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // 🔧 FIX: Double-check với lock ngay lập tức
            if (!hasBeenSelected && !isAnyChestSelected)
            {
                // Set flag NGAY để chặn chest khác
                if (isAnyChestSelected) return; // Double check
                isAnyChestSelected = true;

                OnPlayerSelectChest(other.gameObject);
            }
        }
    }

    private void OnPlayerSelectChest(GameObject player)
    {
        // 🔧 FIX: Kiểm tra lại và disable collider NGAY
        if (hasBeenSelected)
            return;

        hasBeenSelected = true;

        // 🔧 Tắt collider NGAY để tránh trigger thêm
        Collider col = GetComponent<Collider>();
        if (col != null)
            col.enabled = false;

        Debug.Log($"💥 Player chọn: {answerContent} (Lane: {GetLaneName()})");

        if (isCorrectAnswer)
        {
            OnCorrectAnswer(player);
        }
        else
        {
            OnWrongAnswer(player);
        }

        DisableAllChestsInWave();
    }

    private void OnCorrectAnswer(GameObject player)
    {
        Debug.Log($"✅ Đúng - {answerContent}");

        BossTeacherControl boss = FindObjectOfType<BossTeacherControl>();
        if (boss != null)
            boss.OnChestSelected();

        // 🔧 FIX: Tìm GetCoins nếu chưa có reference
        if (getCoins == null)
        {
            getCoins = FindObjectOfType<GetCoins>();
            if (getCoins == null)
            {
                Debug.LogError("❌ Không tìm thấy GetCoins trong scene!");
                return;
            }
        }

        // ⏱️ Delay để đảm bảo UI đã sẵn sàng
        StartCoroutine(SpawnCoinsDelayed());
    }

    private IEnumerator SpawnCoinsDelayed()
    {
        yield return new WaitForSeconds(0.1f); // Chờ 1 frame

        if (getCoins == null)
        {
            getCoins = FindObjectOfType<GetCoins>();
        }

        if (getCoins != null)
        {
            Debug.Log("🪙 Calling RewardCoins...");
            getCoins.RewardCoins();
        }
        else
        {
            Debug.LogError("❌ GetCoins not found after search!");
        }
    }

    private void OnWrongAnswer(GameObject player)
    {
        Debug.Log($"❌ Sai - {answerContent}");

        BossTeacherControl boss = FindObjectOfType<BossTeacherControl>();
        if (boss != null)
            boss.ElectricShockPlayer(player);
    }

    private void DisableAllChestsInWave()
    {
        Chest[] allChests = FindObjectsOfType<Chest>();
        foreach (Chest chest in allChests)
        {
            if (chest != null && chest.gameObject.activeInHierarchy)
            {
                // 🔧 FIX: Tắt collider NGAY của tất cả chest
                Collider col = chest.GetComponent<Collider>();
                if (col != null)
                    col.enabled = false;

                chest.Invoke(nameof(chest.ReturnToPool), 0.5f);
            }
        }
    }

    private string GetLaneName()
    {
        switch (answerIndex)
        {
            case 0: return "Trái";
            case 1: return "Giữa";
            case 2: return "Phải";
            default: return "Unknown";
        }
    }

    public void ReturnToPool()
    {
        CancelInvoke();

        if (ObjectPool.Instance != null)
            ObjectPool.Instance.ReturnToPoolQuynh(poolTag, gameObject);
        else
            Destroy(gameObject);
    }

    private void OnBecameInvisible()
    {
        if (Time.time - spawnTime > 1f)
            Invoke(nameof(ReturnToPool), 2f);
    }

    private void OnDisable()
    {
        CancelInvoke();
        hasBeenSelected = false;
    }
}