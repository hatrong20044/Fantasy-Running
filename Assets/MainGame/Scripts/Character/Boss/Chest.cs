using UnityEngine;

public class Chest : MonoBehaviour
{
    [Header("Pool Settings")]
    public string poolTag = "Chest";

    [Header("Lifetime")]
    public float autoReturnDelay = 10f; // Tự động biến mất sau 10s nếu không ai chọn

    [Header("Answer Settings (Optional)")]
    public bool isCorrectAnswer = false; // Đánh dấu chest đúng/sai (dùng cho quiz)
    public int answerIndex = 0;          // 0=Trái, 1=Giữa, 2=Phải

    private float spawnTime;

    private void OnEnable()
    {
        ResetChest();
    }

    public void ResetChest()
    {
        spawnTime = Time.time;
        // Reset state khác nếu cần (animation, material, etc.)
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
        Debug.Log($"💥 Player chọn chest {answerIndex} (Lane: {GetLaneName()})");

        // TODO: Xử lý logic chọn đáp án
        if (isCorrectAnswer)
        {
            Debug.Log("✅ Đúng rồi!");
            // Cộng điểm, effect đúng, etc.
        }
        else
        {
            Debug.Log("❌ Sai rồi!");
            // Trừ điểm, effect sai, damage player, etc.
        }

        // Xóa chest sau khi chọn
        ReturnToPool();
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
        // Return sau 2s khi ra khỏi camera (tránh chest nằm mãi)
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