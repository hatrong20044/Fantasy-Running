using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class BossTeacherControl : BossBase
{
    [Header("Teacher Control")]
    public float undergroundDepth = -5f;
    public float hoverHeight = 2f;
    public float followDistance = 10f;
    public float riseUpSmoothTime = 0.8f;
    public float preSpawnWarningTime = 1f;
    public float followSmooth = 5f;

    [Header("Attack Settings")]
    public ChestSpawner chestSpawner;
    public float firstAttackDelay = 1f;
    public float idleTimeAfterAttack = 2f;  // ⭐ Thời gian idle sau khi attack xong

    [Header("Animation")]
    public Animator bossAnim;
    public string currentAnim;
    public ParticleSystem auraEffect;

    [Header("Number Question")]
    public int numberQ = 20;

    private Player playerComp;
    private GameplayUI gameplayUI;
    private bool canAttack = false;
    private bool isAttacking = false;  // ⭐ Flag để track trạng thái attacking
    private int remainingQuestions;

    // smoothDamp data
    private float currentY;
    private float yVelocity;
    private bool isRising = false;
    private bool hasRisen = false;

    protected override void Awake()
    {
        base.Awake();
        playerComp = FindObjectOfType<Player>();
        gameplayUI = UIManager.Instance.GetActiveUI<GameplayUI>(UIName.GameplayUI);
        remainingQuestions = numberQ;
    }

    protected override IEnumerator SpawnBehavior()
    {
        gameplayUI?.ShowWarning();
        yield return new WaitForSeconds(preSpawnWarningTime);
        StartCoroutine(RiseUpSmooth());
    }

    private IEnumerator RiseUpSmooth()
    {
        if (player == null) yield break;

        isRising = true;
        hasRisen = false;
        currentY = undergroundDepth;

        while (true)
        {
            Vector3 targetPos = player.position + player.forward * followDistance;

            if (!hasRisen)
            {
                currentY = Mathf.SmoothDamp(currentY, hoverHeight, ref yVelocity, riseUpSmoothTime);

                if (Mathf.Abs(currentY - hoverHeight) < 0.05f)
                {
                    currentY = hoverHeight;
                    hasRisen = true;
                    isRising = false;
                    gameplayUI?.HideWarning();

                    // ⭐ Bắt đầu với Idle animation
                    ChangeAnim("Rise");
                    auraEffect.Play();
                    StartCoroutine(BossActivity());
                }
            }

            Vector3 finalPos = targetPos + Vector3.up * currentY;
            transform.position = Vector3.Lerp(transform.position, finalPos, followSmooth * Time.deltaTime);
            transform.LookAt(player.position + Vector3.up * 1.5f);

            yield return null;
        }
    }
    private IEnumerator BossActivity()
    {
        // Delay ban đầu
        yield return new WaitForSeconds(firstAttackDelay);
        canAttack = true;

        // Loop attack
        while (remainingQuestions > 0 && canAttack)
        {
            // 1️⃣ IDLE STATE
            ChangeAnim("Idle");
            yield return new WaitForSeconds(idleTimeAfterAttack);

            yield return StartCoroutine(PerformAttack());

            remainingQuestions--;
        }

        // Hết câu hỏi
        ChangeAnim("Idle");
        Debug.Log("Boss đã hết câu hỏi!");
    }

    // ⭐ Coroutine riêng cho 1 lần attack
    private IEnumerator PerformAttack()
    {
        if (isAttacking) yield break;

        isAttacking = true;
        // Chơi animation Attack
        ChangeAnim("Spawn");
        Debug.Log("🎯 Boss bắt đầu attack!");
        chestSpawner.SpawnChestWave();
        

        // ⭐ Đợi animation Attack play xong
        // Cách 1: Dùng Animation Event (khuyên dùng)
        // Cách 2: Dùng fixed time (backup)
        yield return new WaitForSeconds(GetAnimationLength("Spawn"));

        isAttacking = false;
        Debug.Log("Boss attack xong!");
    }

    // ⭐ GỌI HÀM NÀY TỪ ANIMATION EVENT (khuyên dùng)
    // Thêm Animation Event vào frame spawn chest trong animation Attack
    //public void OnAttackSpawnChest()
    //{
    //    Debug.Log("📦 Animation Event: Spawn chest!");

    //    if (chestSpawner != null)
    //    {
    //        chestSpawner.SpawnChestWave();
    //    }
    //    else
    //    {
    //        Debug.LogWarning("⚠️ ChestSpawner chưa được gán!");
    //    }
    //}

    // ⭐ Helper: Lấy độ dài animation
    private float GetAnimationLength(string animName)
    {
        AnimatorClipInfo[] clipInfo = bossAnim.GetCurrentAnimatorClipInfo(0);
        foreach (var clip in clipInfo)
        {
            if (clip.clip.name == animName)
            {
                return clip.clip.length;
            }
        }

        // Fallback: return thời gian mặc định
        return 1f;
    }

    private void ChangeAnim(string animName)
    {
        if (currentAnim == animName) return;
        currentAnim = animName;
        bossAnim.CrossFadeInFixedTime(animName, 0.1f);
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
}