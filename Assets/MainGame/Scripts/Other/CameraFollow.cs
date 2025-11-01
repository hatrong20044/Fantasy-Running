using UnityEngine;
using System.Collections;

public class CameraFollow : MonoBehaviour
{
    public Transform target;

    [Header("Settings")]
    public Vector3 followOffset = new Vector3(0, 4, -6); // phía sau player
    public Vector3 introOffset = new Vector3(0, 2, 3);   // phía trước player
    public float smoothTime = 0.1f;

    private Vector3 velocity;
    private bool isFollowing = false;

    private void OnEnable()
    {
        Home.OnPlayPressed += HandlePlayPressed;
    }

    private void OnDisable()
    {
        Home.OnPlayPressed -= HandlePlayPressed;
    }

    private IEnumerator Start()
    {
        yield return null;

        if (target)
        {
            SetIntroPosition();
        }
    }

    private void SetIntroPosition()
    {
        if (target == null) return;

        // 1. Lấy vị trí introOffset
        Vector3 localOffset = new Vector3(introOffset.x, introOffset.y, introOffset.z);

        // 2. Đặt vị trí
        transform.position = localOffset;

        // 3. Đặt góc quay tự do (x, y, z)
        transform.rotation = Quaternion.Euler(10f, 51f, 0f);
    }


    private void LateUpdate()
    {
        if (!isFollowing || target == null) return;

        Vector3 desiredPos = target.position + followOffset;
        transform.position = Vector3.SmoothDamp(transform.position, desiredPos, ref velocity, smoothTime);
        transform.LookAt(target.position + Vector3.up * 2f);
    }

    private void HandlePlayPressed()
    {
        StartCoroutine(SwitchToFollow());
    }

    private IEnumerator SwitchToFollow()
    {
        Vector3 startPos = transform.position;
        Quaternion startRot = transform.rotation;

        Vector3 endPos = target.position + followOffset;
        Quaternion endRot = Quaternion.LookRotation(target.position + Vector3.up * 2f - endPos);

        float duration = 1.2f;
        float t = 0;

        while (t < duration)
        {
            t += Time.deltaTime;
            float normalized = Mathf.SmoothStep(0, 1, t / duration);

            transform.position = Vector3.Lerp(startPos, endPos, normalized);
            transform.rotation = Quaternion.Slerp(startRot, endRot, normalized);

            yield return null;
        }

        isFollowing = true;
    }
}
