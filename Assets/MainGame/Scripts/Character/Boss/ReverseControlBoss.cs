using UnityEngine;

public class ReverseControlBoss : BossBase
{
    public float descendSpeed = 3f;
    public float followDistance = 5f;
    public float hoverHeight = 3f;

    protected override void Start()
    {
        base.Start();
        if (player != null)
        {
            player.ReverseInput(true);
           
        }
    }


    void LateUpdate()
    {
        if (!isActive || player == null) return;

        Vector3 targetPos = player.transform.position + Vector3.back * followDistance + Vector3.up * hoverHeight;
        transform.position = Vector3.Lerp(transform.position, targetPos, descendSpeed * Time.deltaTime);
        transform.LookAt(player.transform);
    }

    protected override void Deactivate()
    {
        base.Deactivate();
        if (player != null)
        {
            player.ReverseInput(false);
            
        }
    }

    public override void PerformBehavior()
    {
        // Boss này chỉ đảo điều khiển
    }
}
