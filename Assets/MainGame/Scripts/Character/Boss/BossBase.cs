using System.Collections;
using UnityEngine;

public abstract class BossBase : MonoBehaviour
{
    [Header("Base Settings")]
    public float disappearDelay = 0.5f;

    protected Animator animator;
    protected Transform player;
    protected bool isActive;

    public event System.Action<BossBase> OnBossFinished;

    protected virtual void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public virtual void Init(Transform playerRef)
    {
        player = playerRef;
    }

    public virtual void Activate()
    {
        isActive = true;
        if (animator) animator.SetTrigger("Appear");

        StartCoroutine(SpawnBehavior());
    }

    /// <summary>
    /// Hành vi spawn riêng cho từng boss — override ở lớp con
    /// </summary>
    protected virtual IEnumerator SpawnBehavior() { yield break; }

    public virtual void EndBoss()
    {
        isActive = false;
        if (animator) animator.SetTrigger("Disappear");
        StartCoroutine(CoFinish());
    }

    private IEnumerator CoFinish()
    {
        yield return new WaitForSeconds(disappearDelay);
        OnBossFinished?.Invoke(this);
        Destroy(gameObject);
    }

    protected virtual void OnDestroy() { }
}
