using System.Collections;
using UnityEngine;
using TMPro;

public class BossTeacherControl : BossBase
{
    [Header("Teacher Control")]
    public float undergroundDepth = -5f;
    public float hoverHeight = 2f;
    public float spawnDistanceAhead = 25f;
    public float followDistance = 10f;
    public float riseUpSmoothTime = 0.8f;
    public float preSpawnWarningTime = 1f;
    public float followSmooth = 5f;

    [Header("Attack Settings")]
    public ChestSpawner chestSpawner;
    public float firstAttackDelay = 1f;
    public float idleTimeAfterAttack = 15f;

    [Header("Animation")]
    public Animator bossAnim;
    public string currentAnim;
    public ParticleSystem auraEffect;
    public ParticleSystem spawnEffect;

    [Header("Number Question")]
    public int numberQ = 20;

    [Header("Question Gates")]
    public GameObject questionGatePrefab;
    public float gateLeftX = -3.24f;
    public float gateRightX = 3.24f;
    public float gateFixedY = 7.36f;
    public float gateDropHeight = 10f;
    public float gateDropDuration = 0.2f;
    public float gateFlyUpHeight = 15f;
    public float gateFlyUpDuration = 0.2f;

    [Header("Electric Shock Settings")]
    public float shockDuration = 1f;
    public ParticleSystem electricEffect;
    public Transform staffTip; //

    private Player playerComp;
    private bool canAttack = false;
    private bool isAttacking = false;
    private int remainingQuestions;
    private bool waitingForChestSelection = false;
    private bool chestWasSelected = false;

    private float currentY;
    private float yVelocity;
    private bool isRising = false;
    private bool hasRisen = false;
    private Vector3 fixedSpawnPosition;
    private bool hasSetSpawnPosition = false;

    private QuestionGate leftGate;
    private QuestionGate rightGate;

    

    protected override void Awake()
    {
        base.Awake();
        playerComp = FindObjectOfType<Player>();
        remainingQuestions = numberQ;

        // 🔧 FIX: Không cần khởi tạo gameplayUI ở đây
        // Sẽ được khởi tạo tự động khi sử dụng (lazy initialization)
    }

    // 🔧 FIX: Method để Chest kiểm tra có đang chờ đáp án không
    public bool IsWaitingForAnswer()
    {
        return !waitingForChestSelection || chestWasSelected;
    }

    protected override IEnumerator SpawnBehavior()
    {

        yield return new WaitForSeconds(preSpawnWarningTime);
        StartCoroutine(RiseUpSmooth());
    }

    private IEnumerator RiseUpSmooth()
    {
        if (player == null) yield break;

        isRising = true;
        hasRisen = false;
        currentY = undergroundDepth;

        if (!hasSetSpawnPosition)
        {
            fixedSpawnPosition = new Vector3(0f, 0f, player.position.z + spawnDistanceAhead);
            hasSetSpawnPosition = true;
            transform.position = fixedSpawnPosition + Vector3.up * undergroundDepth;
        }

        Vector3 effectPos = new Vector3(0f, 0.05f, fixedSpawnPosition.z);
        ParticleSystem effectInstance = Instantiate(spawnEffect, effectPos, Quaternion.identity);
        effectInstance.Play();
        Destroy(effectInstance.gameObject, 2f);

        while (true)
        {
            if (!hasRisen)
            {
                currentY = Mathf.SmoothDamp(currentY, hoverHeight, ref yVelocity, riseUpSmoothTime);
                transform.position = fixedSpawnPosition + Vector3.up * currentY;
                transform.LookAt(player.position + Vector3.up * 1.5f);

                if (Mathf.Abs(currentY - hoverHeight) < 0.05f)
                {
                    spawnEffect.Play();
                    currentY = hoverHeight;
                    hasRisen = true;
                    isRising = false;


                    ChangeAnim("Rise");
                    auraEffect.Play();

                    // Spawn 2 gate Question
                    SpawnQuestionGates();

                    StartCoroutine(BossActivity());
                }
            }
            else
            {
                Vector3 targetPos = player.position + player.forward * followDistance;
                targetPos.x = 0f;
                Vector3 finalPos = targetPos + Vector3.up * currentY;

                transform.position = Vector3.Lerp(transform.position, finalPos, followSmooth * Time.deltaTime);
                transform.LookAt(player.position + Vector3.up * 1.5f);

                // Update vị trí của 2 cổng (di chuyển theo boss)
                UpdateGatePositions();
            }

            yield return null;
        }
    }

    private void SpawnQuestionGates()
    {
        Vector3 leftPos = new Vector3(gateLeftX, gateFixedY + gateDropHeight, fixedSpawnPosition.z);
        Vector3 rightPos = new Vector3(gateRightX, gateFixedY + gateDropHeight, fixedSpawnPosition.z);

        Debug.Log($"🚪 Spawning gates at: Left={leftPos}, Right={rightPos}");

        GameObject leftObj = Instantiate(questionGatePrefab, leftPos, Quaternion.identity);
        GameObject rightObj = Instantiate(questionGatePrefab, rightPos, Quaternion.identity);

        Debug.Log($"✅ Gates spawned: Left={leftObj != null}, Right={rightObj != null}");

        leftGate = leftObj.GetComponent<QuestionGate>();
        rightGate = rightObj.GetComponent<QuestionGate>();

        Debug.Log($"✅ Gate components: Left={leftGate != null}, Right={rightGate != null}");
    }

    private void UpdateGatePositions()
    {
        if (leftGate != null)
        {
            Vector3 pos = new Vector3(gateLeftX, leftGate.transform.position.y, transform.position.z);
            leftGate.transform.position = Vector3.Lerp(leftGate.transform.position, pos, Time.deltaTime * 3f);
        }

        if (rightGate != null)
        {
            Vector3 pos = new Vector3(gateRightX, rightGate.transform.position.y, transform.position.z);
            rightGate.transform.position = Vector3.Lerp(rightGate.transform.position, pos, Time.deltaTime * 3f);
        }
    }

    private IEnumerator DropDown()
    {
        float t = 0;
        while (t <0.2)
        {
            t += Time.deltaTime / gateDropDuration;
            float newY = Mathf.Lerp(gateFixedY + gateDropHeight, gateFixedY, t);

            if (leftGate != null)
                leftGate.transform.position = new Vector3(gateLeftX, newY, fixedSpawnPosition.z);

            if (rightGate != null)
                rightGate.transform.position = new Vector3(gateRightX, newY, fixedSpawnPosition.z);

            yield return null;
        }
    }

    private IEnumerator FlyUpAndDisappear()
    {
        float t = 0;
        while (t < 0.2)
        {
            t += Time.deltaTime / gateFlyUpDuration;
            float newY = Mathf.Lerp(gateFixedY, gateFixedY + gateFlyUpHeight, t);

            if (leftGate != null)
                leftGate.transform.position = new Vector3(gateLeftX, newY, fixedSpawnPosition.z);

            if (rightGate != null)
                rightGate.transform.position = new Vector3(gateRightX, newY, fixedSpawnPosition.z);

            yield return null;
        }

        if (leftGate != null) leftGate.HideQuestion();
        if (rightGate != null) rightGate.HideQuestion();
    }

    private IEnumerator BossActivity()
    {
        yield return new WaitForSeconds(firstAttackDelay);
        canAttack = true;

        while (remainingQuestions > 0 && canAttack)
        {
            // 🔧 FIX: Reset flag trước mỗi wave
            Chest.ResetSelectionFlag();

            // Thực hiện attack TRƯỚC (spawn chest + drop gates)
            yield return StartCoroutine(PerformAttack());

            // SAU ĐÓ mới đợi player chọn chest
            waitingForChestSelection = true;
            chestWasSelected = false;

            Debug.Log("⏳ Đang chờ player chọn chest...");
            yield return new WaitUntil(() => chestWasSelected);

            waitingForChestSelection = false;
            remainingQuestions--;

            Debug.Log($"📊 Còn {remainingQuestions} câu hỏi");

            // Delay giữa các lần attack
            yield return new WaitForSeconds(1f);
        }

        // 🔧 FIX: Khi hết câu hỏi → bay lên và biến mất
        Debug.Log("🎉 Boss đã hết câu hỏi! Biến mật...");
        yield return StartCoroutine(FlyUpAndDisappear());

        yield return new WaitForSeconds(0.5f);

        // 🔧 Gọi EndBoss để xử lý logic kết thúc
        EndBoss();
    }

    private IEnumerator PerformAttack()
    {
        if (isAttacking) yield break;

        isAttacking = true;
        ChangeAnim("Spawn");
        Debug.Log("🎯 Boss bắt đầu attack!");

        // Spawn chests TRƯỚC để cập nhật currentQuestion
        chestSpawner.SpawnChestWave();

        // Đợi 1 frame cho chắc chắn currentQuestion được gán
        yield return null;

        // Bây giờ lấy question
        string question = chestSpawner.GetCurrentQuestionText();
        Debug.Log($"📘 [BossTeacher] Câu hỏi: {question}");

        // Drop gates xuống
        yield return StartCoroutine(DropDown());

        // Hiển thị câu hỏi trên gates
        if (leftGate != null) leftGate.ShowQuestion(question);
        if (rightGate != null) rightGate.ShowQuestion(question);

        yield return new WaitForSeconds(GetAnimationLength("Spawn"));

        isAttacking = false;
        ChangeAnim("Idle");
    }

    public void OnPlayerAnswerWrong()
    {
        Debug.Log("❌ Player sai! Gates bay lên!");
        StartCoroutine(FlyUpAndDisappear());

        // 🔧 FIX: Set flag để kết thúc vòng chờ
        chestWasSelected = true;
    }

    public void OnChestSelected()
    {
        if (waitingForChestSelection)
        {
            chestWasSelected = true;
            Debug.Log("✅ Player đúng! Chuyển câu hỏi tiếp theo...");
        }
    }

    public void ElectricShockPlayer(GameObject playerObj)
    {
        Debug.Log("⚡ GIẬT ĐIỆN!");
        ChangeAnim("Attack");

        if (electricEffect != null && staffTip != null)
        {
            // 🎯 Vị trí đầu trượng và vị trí player
            Vector3 startPos = staffTip.position;
            Vector3 targetPos = playerObj.transform.position + Vector3.up * 1f;

            // Tạo hiệu ứng tại đầu trượng, xoay về phía player
            Quaternion rot = Quaternion.LookRotation(targetPos - startPos);
            ParticleSystem effect = Instantiate(electricEffect, startPos, rot);

            // Bay từ trượng tới player
            StartCoroutine(MoveElectricEffect(effect.transform, startPos, targetPos, 0.5f));

            // Xóa sau 2 giây
            Destroy(effect.gameObject, 2f);
        }

        OnPlayerAnswerWrong();

        // Sau 1 giây hiển thị UI GameOver
        StartCoroutine(ShowGameOverAfterDelay(playerObj, 1f));
    }

    // Hiệu ứng bay từ trượng đến player
    private IEnumerator MoveElectricEffect(Transform effect, Vector3 start, Vector3 target, float duration)
    {
        float t = 0;
        while (t < 1)
        {
            t += Time.deltaTime / duration;
            effect.position = Vector3.Lerp(start, target, t);
            yield return null;
        }

        // Khi tới nơi, có thể phát nổ nhỏ hoặc tạo thêm particle tại player
        // Ví dụ:
        // Instantiate(hitEffectPrefab, target, Quaternion.identity);
    }

    public float GetAnimationLength(string animName)
    {
        AnimatorClipInfo[] clipInfo = bossAnim.GetCurrentAnimatorClipInfo(0);
        foreach (var clip in clipInfo)
        {
            if (clip.clip.name == animName)
                return clip.clip.length;
        }
        return 1f;
    }

    private IEnumerator ShowGameOverAfterDelay(GameObject playerObj, float delay)
    {
        yield return new WaitForSeconds(delay);

        PlayerCollision playerCol = playerObj.GetComponent<PlayerCollision>();
        if (playerCol != null)
        {
            playerCol.GameOver();
        }
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

        if (leftGate != null) Destroy(leftGate.gameObject);
        if (rightGate != null) Destroy(rightGate.gameObject);

        base.EndBoss();
    }

    protected override void OnDestroy()
    {
        if (leftGate != null) Destroy(leftGate.gameObject);
        if (rightGate != null) Destroy(rightGate.gameObject);
        base.OnDestroy();
    }
}