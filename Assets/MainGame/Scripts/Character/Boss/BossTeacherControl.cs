using System.Collections;
using UnityEngine;
using TMPro;

public class BossTeacherControl : BossBase
{
    [Header("Teacher Movement")]
    public float undergroundDepth = -5f;
    public float hoverHeight = 2f;
    public float spawnDistanceAhead = 25f;
    public float followDistance = 10f;
    public float riseUpSmoothTime = 0.2f;
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

    [Header("Question Count")]
    public int numberQ = 20;

    [Header("Electric Shock Settings")]
    public float shockDuration = 1f;
    public ParticleSystem electricEffect;
    public Transform staffTip;

    private Player playerComp;
    private GameplayUI _gameplayUI;

    private bool canAttack = false;
    private bool isAttacking = false;
    private int remainingQuestions;
    private bool waitingForChestSelection = false;
    private bool chestWasSelected = false;

    private float currentY;
    private float yVelocity;
    private bool hasRisen = false;
    private Vector3 fixedSpawnPosition;
    private bool hasSetSpawnPosition = false;

    private GameplayUI gameplayUI
    {
        get
        {
            if (_gameplayUI == null && UIManager.Instance != null)
            {
                _gameplayUI = UIManager.Instance.GetActiveUI<GameplayUI>(UIName.GameplayUI);
            }
            return _gameplayUI;
        }
    }

    protected override void Awake()
    {
        base.Awake();
        playerComp = FindObjectOfType<Player>();
        remainingQuestions = numberQ;
    }

    protected override IEnumerator SpawnBehavior()
    {
        yield return new WaitForSeconds(preSpawnWarningTime);
        StartCoroutine(RiseUpSmooth());
    }

    private IEnumerator RiseUpSmooth()
    {
        if (player == null) yield break;

        currentY = undergroundDepth;
        hasRisen = false;

        if (!hasSetSpawnPosition)
        {
            fixedSpawnPosition = new Vector3(0f, 0f, player.position.z + spawnDistanceAhead);
            hasSetSpawnPosition = true;
            transform.position = fixedSpawnPosition + Vector3.up * undergroundDepth;
        }

        ParticleSystem fx = Instantiate(spawnEffect, fixedSpawnPosition, Quaternion.identity);
        fx.Play();
        Destroy(fx.gameObject, 2f);

        while (!hasRisen)
        {
            currentY = Mathf.SmoothDamp(currentY, hoverHeight, ref yVelocity, riseUpSmoothTime);
            transform.position = fixedSpawnPosition + Vector3.up * currentY;
            transform.LookAt(player.position + Vector3.up * 1.5f);

            if (Mathf.Abs(currentY - hoverHeight) < 1f)
            {
                currentY = hoverHeight;
                hasRisen = true;

                gameplayUI?.HideWarning();
                ChangeAnim("Rise");
                auraEffect?.Play();

                StartCoroutine(BossActivity());
            }

            yield return null;
        }

        while (true)
        {
            Vector3 targetPos = player.position + player.forward * followDistance;
            targetPos.x = 0f;
            Vector3 finalPos = targetPos + Vector3.up * currentY;

            transform.position = Vector3.Lerp(transform.position, finalPos, followSmooth * Time.deltaTime);
            transform.LookAt(player.position + Vector3.up * 1.5f);
            yield return null;
        }
    }

    private IEnumerator BossActivity()
    {
        yield return new WaitForSeconds(firstAttackDelay);
        canAttack = true;

        while (remainingQuestions > 0 && canAttack)
        {
            Chest.ResetSelectionFlag();
            yield return StartCoroutine(PerformAttack());

            waitingForChestSelection = true;
            chestWasSelected = false;

            Debug.Log("⏳ Đang chờ player chọn chest...");
            yield return new WaitUntil(() => chestWasSelected);
            waitingForChestSelection = false;

            remainingQuestions--;
            Debug.Log($"📊 Còn {remainingQuestions} câu hỏi");

            yield return new WaitForSeconds(1f);
        }

        Debug.Log("🎉 Boss đã hết câu hỏi! Biến mất...");
        yield return StartCoroutine(FlyUpAndDisappear());
        EndBoss();
    }

    private IEnumerator PerformAttack()
    {
        if (isAttacking) yield break;
        isAttacking = true;
        ChangeAnim("Spawn");

        Debug.Log("🎯 Boss bắt đầu attack!");

        chestSpawner.SpawnQuestionGates();
        chestSpawner.SpawnChestWave();
        chestSpawner.ShowQuestion();

        Debug.Log("✅ Đã gọi spawn chest");

        yield return new WaitForSeconds(GetAnimationLength("Spawn"));
        isAttacking = false;
        ChangeAnim("Idle");
    }

    public void OnChestSelected()
    {
        if (waitingForChestSelection)
        {
            chestWasSelected = true;
            Debug.Log("✅ Player đúng! Chuyển câu hỏi tiếp theo...");
            chestSpawner.HideQuestionGates();
        }
    }

    public void OnPlayerAnswerWrong()
    {
        Debug.Log("❌ Player sai! Ẩn question!");
        chestSpawner.HideQuestionGates();
        chestWasSelected = true;
    }

    public void ElectricShockPlayer(GameObject playerObj)
    {
        Debug.Log("⚡ GIẬT ĐIỆN!");
        ChangeAnim("Attack");

        if (electricEffect != null && staffTip != null)
        {
            Vector3 startPos = staffTip.position;
            Vector3 targetPos = playerObj.transform.position + Vector3.up * 1f;
            Quaternion rot = Quaternion.LookRotation(targetPos - startPos);

            ParticleSystem effect = Instantiate(electricEffect, startPos, rot);
            StartCoroutine(MoveElectricEffect(effect.transform, startPos, targetPos, 0.5f));
            Destroy(effect.gameObject, 2f);
        }

        OnPlayerAnswerWrong();
        StartCoroutine(ShowGameOverAfterDelay(playerObj, 1f));
    }

    private IEnumerator MoveElectricEffect(Transform effect, Vector3 start, Vector3 target, float duration)
    {
        float t = 0;
        while (t < 1)
        {
            t += Time.deltaTime / duration;
            effect.position = Vector3.Lerp(start, target, t);
            yield return null;
        }
    }

    private IEnumerator ShowGameOverAfterDelay(GameObject playerObj, float delay)
    {
        yield return new WaitForSeconds(delay);

        PlayerCollision playerCol = playerObj.GetComponent<PlayerCollision>();
        playerCol?.GameOver();
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

    private void ChangeAnim(string animName)
    {
        if (currentAnim == animName) return;
        currentAnim = animName;
        bossAnim.CrossFadeInFixedTime(animName, 0.1f);
    }

    private IEnumerator FlyUpAndDisappear()
    {
        float t = 0;
        float duration = 0.8f;
        Vector3 start = transform.position;
        Vector3 end = start + Vector3.up * 10f;

        while (t < 1)
        {
            t += Time.deltaTime / duration;
            transform.position = Vector3.Lerp(start, end, t);
            yield return null;
        }
    }

    public override void EndBoss()
    {
        canAttack = false;
        StopAllCoroutines();

        if (chestSpawner != null)
            chestSpawner.DestroyQuestionGates();

        base.EndBoss();
    }
}