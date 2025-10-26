using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class BossTeacherControl : BossBase
{
    [Header("Teacher Control")]
    public float undergroundDepth = -5f;
    public float hoverHeight = 2f;
    public float spawnDistanceAhead = 25f;  // ⭐ Khoảng cách spawn phía trước player
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

    private Player playerComp;
    private GameplayUI gameplayUI;
    private bool canAttack = false;
    private bool isAttacking = false;
    private int remainingQuestions;

    // ⭐ Flag để chờ player chọn chest
    private bool waitingForChestSelection = false;
    private bool chestWasSelected = false;

    [Header("Electric Shock Settings")]
    public float shockDuration = 1f;
    public ParticleSystem electricEffect;

    // smoothDamp data
    private float currentY;
    private float yVelocity;
    private bool isRising = false;
    private bool hasRisen = false;
    
    // ⭐ Vị trí spawn cố định
    private Vector3 fixedSpawnPosition;
    private bool hasSetSpawnPosition = false;

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

        // ⭐ Tính vị trí spawn cố định: X=0, Z=player.z+25
        if (!hasSetSpawnPosition)
        {
            fixedSpawnPosition = new Vector3(0f, 0f, player.position.z + spawnDistanceAhead);
            hasSetSpawnPosition = true;

            // ⭐ Đặt boss xuống dưới đất tại vị trí spawn
            transform.position = fixedSpawnPosition + Vector3.up * undergroundDepth;
        }
        Vector3 effectPos = new Vector3(0f, 0.05f, fixedSpawnPosition.z);
        ParticleSystem effectInstance = Instantiate(spawnEffect, effectPos, Quaternion.identity);
        effectInstance.Play();
        Destroy(effectInstance.gameObject, 2f); // ⭐ Tự tắt sau 2 giây

        while (true)
        {
            if (!hasRisen)
            {
                // ⭐ Bay lên từ dưới đất TẠI CHỖ
                

                currentY = Mathf.SmoothDamp(currentY, hoverHeight, ref yVelocity, riseUpSmoothTime);

                // ⭐ Đứng yên tại vị trí spawn (X=0)
                transform.position = fixedSpawnPosition + Vector3.up * currentY;
                transform.LookAt(player.position + Vector3.up * 1.5f);

                if (Mathf.Abs(currentY - hoverHeight) < 0.05f)
                {
                    // ⭐ Thêm hiệu ứng spawn ở mặt đất
                    

                    spawnEffect.Play();
                    currentY = hoverHeight;
                    hasRisen = true;
                    isRising = false;
                    gameplayUI?.HideWarning();

                    ChangeAnim("Rise");
                    auraEffect.Play();
                    StartCoroutine(BossActivity());
                }
            }
            else
            {
                // ⭐ Sau khi bay lên đủ cao, follow player
                Vector3 targetPos = player.position + player.forward * followDistance;
                targetPos.x = 0f;  // ⭐ Giữ X=0 khi follow
                Vector3 finalPos = targetPos + Vector3.up * currentY;

                transform.position = Vector3.Lerp(transform.position, finalPos, followSmooth * Time.deltaTime);
                transform.LookAt(player.position + Vector3.up * 1.5f);
            }

            yield return null;
        }
    }
    private IEnumerator BossActivity()
    {
        yield return new WaitForSeconds(firstAttackDelay);
        canAttack = true;

        while (remainingQuestions > 0 && canAttack)
        {
            if(remainingQuestions == 5)
            {
                ChangeAnim("Idle");
                yield return new WaitForSeconds(idleTimeAfterAttack);
                yield return StartCoroutine(PerformAttack());
                remainingQuestions--;
            }
            else
            {
                waitingForChestSelection = true;
                chestWasSelected = false;
                
                yield return new WaitUntil(() => chestWasSelected);
                
                waitingForChestSelection = false;
                yield return StartCoroutine(PerformAttack());
            }
        }

        ChangeAnim("Idle");
        Debug.Log("Boss đã hết câu hỏi!");
    }

    private IEnumerator PerformAttack()
    {
        if (isAttacking) yield break;

        isAttacking = true;
        ChangeAnim("Spawn");
        Debug.Log("🎯 Boss bắt đầu attack!");
        chestSpawner.SpawnChestWave();

        yield return new WaitForSeconds(GetAnimationLength("Spawn"));

        isAttacking = false;
        ChangeAnim("Idle");
        Debug.Log("Boss attack xong!");
    }

    /// <summary>
    /// ⭐ HÀM NÀY ĐƯỢC GỌI TỪ CHEST KHI PLAYER CHỌN
    /// </summary>
    public void OnChestSelected()
    {
        if (waitingForChestSelection)
        {
            chestWasSelected = true;
            Debug.Log("🎯 Boss nhận tín hiệu: Player đã chọn chest!");
        }
    }

    public float GetAnimationLength(string animName)
    {
        AnimatorClipInfo[] clipInfo = bossAnim.GetCurrentAnimatorClipInfo(0);
        foreach (var clip in clipInfo)
        {
            if (clip.clip.name == animName)
            {
                return clip.clip.length;
            }
        }
        return 1f;
    }

    public void ElectricShockPlayer(GameObject playerObj)
    {
        Debug.Log("⚡ BOSS GIẬT ĐIỆN PLAYER!");

        ChangeAnim("Attack");

        if (electricEffect != null)
        {
            ParticleSystem effect = Instantiate(electricEffect, playerObj.transform.position, Quaternion.identity);
            Destroy(effect.gameObject, 2f);
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
        base.EndBoss();
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
    }
}