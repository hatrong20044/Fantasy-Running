using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnRocket : BossBase
{
    public float descendSpeed = 3f;
    public float followDistance = -25f;
    public float hoverHeight = 4f;

    public int maxBullet = 2; // số lượng đạn tối đa
    public Transform CannonPos; // vị trí nòng súng nơi mà đạn sẽ được spawn ra
    public float flightDuration; // thời gian đạn bay, phải đồng bộ với bên Script BulletMovement
    public float laneDistance = 2.5f; // độ rộng của lane, cố định 2.5
    public float timeInterVal = 1.5f; // khoảng thời gian giữa các lần spawn.
    public static SpawnRocket instance;
    public bool isSpawnable = true;

    private void Start()
    {
        Debug.Log("On Start");
        SpawnRocket.instance = this;
        if (player != null)
        {
            StartCoroutine(DelaySpawn());
        }
    }

    private IEnumerator DelaySpawn()
    {
        yield return new WaitForSeconds(1.5f);
        isSpawnable = true;
        StartCoroutine(SpawnLoop());
    }

    private IEnumerator SpawnLoop()
    {
        while (isActive) // nếu boss vẫn còn xuất hiện thì spawn đạn
        {
            Spawn();
            yield return new WaitForSeconds(timeInterVal);
        }
    }

    void LateUpdate()
    {
        if (!isActive || player == null) return;

        Vector3 targetPos = player.transform.position + Vector3.back * followDistance + Vector3.up * hoverHeight;
        transform.position = Vector3.Lerp(transform.position, targetPos, descendSpeed * Time.deltaTime);
        transform.LookAt(player.transform);
        transform.rotation = Quaternion.Euler(0, 90, 0);
    }


    public void Spawn()
    {
        if (isSpawnable)
        {
            Debug.Log("is spawnable");
            StartCoroutine(SpawnWithDelay());
        }
    }

    private IEnumerator SpawnWithDelay()
    {
        List<int> lanes = new List<int> { -1, 0, 1 };// list để phục vụ spawn đạn random ra các đường

        for (int i = 0; i < maxBullet; i++)
        {
            animator.SetTrigger("Shot"); // chuyển animation thành bắn đạn 
            int laneIndex = Random.Range(0, lanes.Count); // chọn lane để spawn đạn

            // lấy ra các đối tượng: đạn, cảnh báo từ pool.
            GameObject bullet = ObjectPool.Instance.GetFromPoolQuynh("Bullet");
            GameObject warning = ObjectPool.Instance.GetFromPoolQuynh("Warning");

            RunWarnning runWarnning = warning.GetComponent<RunWarnning>();
            BulletMovement bulletMovement = bullet.GetComponent<BulletMovement>();

            //đồng bộ với flightDuration của bullet
            flightDuration = bulletMovement.flightDuration;

            //tính toán vị trí đạn rơi trúng người chơi dựa trên tốc độ của người chơi, thời gian đạn bay
            Vector3 targetPos = CalculatePredictedLandingPositions(lanes[laneIndex]);

            bulletMovement.Launch(CannonPos.position, targetPos); // khởi tạo vị trí spawn và target
            bulletMovement.StartMoving(); // bullet di chuyển đến vị trí target

            warning.transform.position = targetPos; // gán vị trí warning tại vị trí bullet sẽ rơi đến
            runWarnning.Act(); // chạy báo động
            lanes.RemoveAt(laneIndex); // xóa phần tử khỏi list lanes để không bị spawn trùng lane

            // Chờ một khoảng thời gian trước khi spawn viên đạn tiếp theo
            yield return new WaitForSeconds(0.2f);
        }
        animator.Play("VLT_09_OpenVault_FlyForward"); // Sửa animation thành bay bình thường 
    }

    private Vector3 CalculatePredictedLandingPositions(int laneIndex)
    {
        Vector3 targetPos = new Vector3();
        // Lấy vận tốc player
        float playerSpeed = player != null ? player.GetComponent<Player>().forwardSpeed : 10f;

        // Tính vị trí Z mà player sẽ đến sau flightDuration
        float playerCurrentZ = player.transform.position.z;
        float predictedZ = playerCurrentZ + (playerSpeed * flightDuration);
        targetPos = new Vector3(laneDistance * laneIndex, 0.1f, predictedZ);
        return targetPos;
    }
}