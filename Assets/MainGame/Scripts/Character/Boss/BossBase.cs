using UnityEngine;

public abstract class BossBase : MonoBehaviour
{
    public float activeTime = 10f;
    protected bool isActive;
    protected float timer;

    protected Player player; 

    public void Initialize(Player playerRef)
    {
        player = playerRef;
    }

    protected virtual void Start()
    {
        isActive = true;
        timer = 0f;
    }

    protected virtual void Update()
    {
        if (!isActive) return;

        timer += Time.deltaTime;
        if (activeTime > 0 && timer >= activeTime)
        {
            Deactivate();
        }
    }

    protected virtual void Deactivate()
    {
        isActive = false;
        gameObject.SetActive(false);
    }

    public abstract void PerformBehavior();
}
