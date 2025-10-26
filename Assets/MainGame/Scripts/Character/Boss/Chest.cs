using UnityEngine;
using TMPro;

public class Chest : MonoBehaviour
{
    [Header("Pool Settings")]
    public string poolTag = "Chest";

    [Header("UI Components")]
    public TMP_Text answerText; // ⭐ KÉO TEXTBOARD VÀO ĐÂY

    [Header("Lifetime")]
    public float autoReturnDelay = 10f; // Tự động biến mất sau 10s

    [Header("Answer Data (Runtime - Không edit)")]
    public int answerIndex = 0;          // 0=Trái, 1=Giữa, 2=Phải
    public bool isCorrectAnswer = false;
    private string answerContent = "";

    private float spawnTime;

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

        // Clear text
        if (answerText != null)
        {
            answerText.text = "";
        }
    }

    /// <summary>
    /// ⭐ Setup đáp án cho chest (được gọi từ ChestSpawner)
    /// </summary>
    public void SetupAnswer(int index, string content, bool correct)
    {
        answerIndex = index;
        answerContent = content;
        isCorrectAnswer = correct;

        // Hiển thị đáp án lên TextBoard
        if (answerText != null)
        {
            answerText.text = content;
        }
        else
        {
            Debug.LogWarning("⚠️ AnswerText (TextMeshPro) chưa được gán!");
        }

        Debug.Log($"Chest setup: Lane={GetLaneName()} | Answer={content} | Correct={correct}");
    }

    private void Update()
    {
        // Tự động return về pool sau thời gian nhất định
        if (Time.time - spawnTime > autoReturnDelay)
        {
            ReturnToPool();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Khi player chạm vào chest
        if (other.CompareTag("Player"))
        {
            OnPlayerSelectChest(other.gameObject);
        }
    }

    private void OnPlayerSelectChest(GameObject player)
    {
        Debug.Log($"💥 Player chọn: {answerContent} (Lane: {GetLaneName()})");

        // ⭐ Xử lý logic chọn đáp án
        if (isCorrectAnswer)
        {
            OnCorrectAnswer(player);
        }
        else
        {
            OnWrongAnswer(player);
        }

        // Xóa chest sau khi chọn
        ReturnToPool();
    }

    /// <summary>
    /// ⭐ Player chọn đúng
    /// </summary>
    private void OnCorrectAnswer(GameObject player)
    {
        Debug.Log($"✅ ĐÚNG RỒI! Answer: {answerContent}");

        // TODO: Thêm logic của bạn ở đây:
        // - Cộng điểm
        // - Play effect đúng
        // - Play sound
        // - Cộng coin

        // Example:
        // GameManager.Instance.AddScore(10);
        // SoundManager.Instance.PlayCorrectSound();
        // EffectManager.Instance.SpawnCorrectEffect(transform.position);
    }

    /// <summary>
    /// ⭐ Player chọn sai
    /// </summary>
    private void OnWrongAnswer(GameObject player)
    {
        Debug.Log($"❌ SAI RỒI! Answer: {answerContent}");

        // TODO: Thêm logic của bạn ở đây:
        // - Trừ điểm
        // - Damage player
        // - Play effect sai
        // - Play sound

        // Example:
        // player.GetComponent<Player>()?.TakeDamage(10);
        // SoundManager.Instance.PlayWrongSound();
        // EffectManager.Instance.SpawnWrongEffect(transform.position);
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
        if (ObjectPool.Instance != null)
        {
            ObjectPool.Instance.ReturnToPoolQuynh(poolTag, gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Optional: Tự động return khi ra khỏi camera
    private void OnBecameInvisible()
    {
        if (Time.time - spawnTime > 1f) // Đảm bảo chest đã spawn ít nhất 1s
        {
            Invoke(nameof(ReturnToPool), 2f);
        }
    }

    private void OnDisable()
    {
        CancelInvoke(); // Clear invoke khi disable
    }
}