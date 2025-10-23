using System.Collections;
using UnityEngine;

public class ReverseControlBoss : BossBase
{
    [Header("Reverse Control")]
    public ParticleSystem reverseEffect;
    public float fallHeight = 10f;         
    public float hoverHeight = 3f;          // độ cao khi boss dung
    public float followDistance = 10f;      // khoảng cách trước player
    public float fallSmoothTime = 0.5f;     // độ mượt khi rơi
    public float preSpawnWarningTime = 1.5f;
    public float followSmooth = 5f;         // độ mượt khi follow sau khi rơi

    private Player playerComp;
    private GameplayUI gameplayUI;
    private bool applied;
    private bool isFalling = false;
    private bool hasLanded = false;

    // smoothDamp data
    private float currentY;
    private float yVelocity;

    protected override void Awake()
    {
        base.Awake();
        gameplayUI = UIManager.Instance.GetActiveUI<GameplayUI>(UIName.GameplayUI);

    }

    protected override IEnumerator SpawnBehavior()
    {
        gameplayUI?.ShowWarning();

        yield return new WaitForSeconds(preSpawnWarningTime);

        StartCoroutine(FallSmooth());
    }

    private IEnumerator FallSmooth()
    {
        if (player == null) yield break;

        isFalling = true;
        hasLanded = false;

        currentY = fallHeight + hoverHeight;

        while (true)
        {
            Vector3 targetPos = player.position + player.forward * followDistance;

            if (!hasLanded)
            {
                currentY = Mathf.SmoothDamp(currentY, hoverHeight, ref yVelocity, fallSmoothTime);

                if (Mathf.Abs(currentY - hoverHeight) < 0.05f)
                {
                    currentY = hoverHeight;
                    hasLanded = true;
                    isFalling = false;

                    
                    gameplayUI?.HideWarning();

                    
                    if (reverseEffect) reverseEffect.Play();
                    if (playerComp != null && !applied)
                    {
                        playerComp.ReverseInput(true);
                        applied = true;
                        
                    }
                }
            }

            
            Vector3 finalPos = targetPos + Vector3.up * currentY;
            transform.position = Vector3.Lerp(transform.position, finalPos, followSmooth * Time.deltaTime);
            transform.LookAt(player.position + Vector3.up * 1.5f);

            yield return null;
        }
    }

    public override void EndBoss()
    {
        if (playerComp != null && applied)
        {
            playerComp.ReverseInput(false);
            applied = false;
            
        }

        base.EndBoss();
    }

    protected override void OnDestroy()
    {
        if (playerComp != null && applied)
        {
            playerComp.ReverseInput(false);
            applied = false;
        }
        base.OnDestroy();
    }
}
