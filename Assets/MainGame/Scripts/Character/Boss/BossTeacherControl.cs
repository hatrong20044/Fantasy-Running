using System.Collections;
using UnityEngine;

public class BossTeacherControl : BossBase
{
    [Header("Teacher Control")]
    public float undergroundDepth = -5f;
    public float hoverHeight = 2f;
    public float followDistance = 10f;
    public float riseUpSmoothTime = 0.8f;
    public float preSpawnWarningTime = 1.5f;
    public float followSmooth = 5f;

    [Header("Attack Settings")]
    public Animator bossAnimator;
    public ChestSpawner chestSpawner;
    public float firstAttackDelay = 2f;         // Delay trước attack đầu tiên (sau khi xuất hiện)
    public float attackCooldown = 5f;           // Thời gian giữa các lần vung gậy
    public string attackAnimationTrigger = "Attack"; // Tên trigger trong Animator

    private Player playerComp;
    private GameplayUI gameplayUI;
    private bool applied;
    private bool isRising = false;
    private bool hasRisen = false;
    private bool canAttack = false;
    private float lastAttackTime;

    // smoothDamp data
    private float currentY;
    private float yVelocity;

    protected override void Awake()
    {
        base.Awake();
        playerComp = FindObjectOfType<Player>();
        gameplayUI = UIManager.Instance.GetActiveUI<GameplayUI>(UIName.GameplayUI);
    }

    protected override IEnumerator SpawnBehavior()
    {
        // Hiển thị cảnh báo
        gameplayUI?.ShowWarning();

        yield return new WaitForSeconds(preSpawnWarningTime);

        // Bắt đầu nổi lên từ dưới đất
        StartCoroutine(RiseUpSmooth());
    }

    private IEnumerator RiseUpSmooth()
    {
        if (player == null) yield break;

        isRising = true;
        hasRisen = false;

        // Khởi tạo vị trí Y dưới mặt đất
        currentY = undergroundDepth;

        while (true)
        {
            // Tính vị trí target theo player
            Vector3 targetPos = player.position + player.forward * followDistance;

            if (!hasRisen)
            {
                // Nổi lên từ dưới đất đến độ cao hover
                currentY = Mathf.SmoothDamp(currentY, hoverHeight, ref yVelocity, riseUpSmoothTime);

                // Kiểm tra đã đạt độ cao mục tiêu chưa
                if (Mathf.Abs(currentY - hoverHeight) < 0.05f)
                {
                    currentY = hoverHeight;
                    hasRisen = true;
                    isRising = false;

                    // Ẩn cảnh báo
                    gameplayUI?.HideWarning();

                

                    // Bắt đầu attack sequence
                    StartCoroutine(AttackSequence());
                }
            }

            // Cập nhật vị trí boss
            Vector3 finalPos = targetPos + Vector3.up * currentY;
            transform.position = Vector3.Lerp(transform.position, finalPos, followSmooth * Time.deltaTime);

            // Quay về phía player
            transform.LookAt(player.position + Vector3.up * 1.5f);

            yield return null;
        }
    }

    private IEnumerator AttackSequence()
    {
        // Delay trước attack đầu tiên (Boss vừa xuất hiện xong)
        Debug.Log($"🎯 Boss đã xuất hiện! Sẽ vung gậy sau {firstAttackDelay}s...");
        yield return new WaitForSeconds(firstAttackDelay);

        canAttack = true;
        lastAttackTime = Time.time;

        // Loop attack liên tục
        while (hasRisen && canAttack)
        {
            if (Time.time >= lastAttackTime + attackCooldown)
            {
                PerformAttack();
                lastAttackTime = Time.time;
            }

            yield return null;
        }
    }

    private void PerformAttack()
    {
        Debug.Log("⚔️ Boss đang vung gậy!");

        // Trigger animation
        if (bossAnimator != null)
        {
            bossAnimator.SetTrigger(attackAnimationTrigger);
        }
        else
        {
            Debug.LogWarning("⚠️ Chưa gán Animator cho Boss! Spawn chest ngay lập tức.");
            // Nếu chưa có animation, spawn chest ngay
            OnAttackSpawnChest();
        }
    }

    /// <summary>
    /// GỌI TỪ ANIMATION EVENT trong animation vung gậy
    /// Thêm Animation Event tại frame gậy đập xuống
    /// Function name: OnAttackSpawnChest
    /// </summary>
    public void OnAttackSpawnChest()
    {
        Debug.Log("📦 Animation Event: Spawn chest!");

        if (chestSpawner != null)
        {
            chestSpawner.SpawnChestWave();
        }
        else
        {
            Debug.LogWarning("⚠️ ChestSpawner chưa được gán!");
        }
    }


   

    public override void EndBoss()
    {
        canAttack = false;
        StopAllCoroutines();

        base.EndBoss();
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
    }

    // Debug: Test attack bằng phím T
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T) && hasRisen)
        {
            Debug.Log("🧪 TEST: Force attack!");
            PerformAttack();
        }
    }
}