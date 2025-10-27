using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletMovement : Movement
{
    public float flightDuration = 1.25f; // thời gian bay
    public float height = 3f; // đỉnh parabol

    private Vector3 startPos;
    private Vector3 targetPos;
    private float timer;

    // khởi tạo vị trí bắt đầu di chuyển và vị trí đích
    public void Launch(Vector3 start, Vector3 target)
    {
        this.startPos = start;
        this.targetPos = target;

        timer = 0f;
        transform.position = startPos;
    }

    protected override void Move()
    {
        if(this.targetPos == null) return;

        timer += Time.deltaTime;
        float t = Mathf.Clamp01(timer / flightDuration);

        // Nội suy tuyến tính giữa start và target để lấy ra đường cong cho đạn
        Vector3 horizontal = Vector3.Lerp(this.startPos, this.targetPos, t);

        // Thêm độ cao parabol
        float parabolicY = Mathf.Sin(t * Mathf.PI) * height;
        transform.position = new Vector3(horizontal.x, horizontal.y + parabolicY, horizontal.z);
    }
}
