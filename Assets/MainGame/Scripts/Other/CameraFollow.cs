using UnityEngine;
using System.Collections;

public class CameraFollow : MonoBehaviour
{
    public Transform target;

    [Header("Settings")]
    public Vector3 followOffset = new Vector3(0, 4, -6);
    public Vector3 introOffset = new Vector3(0, 2, 3);

    [Header("Smooth Settings")]
    public float smoothTime = 0.1f;
    [Range(0, 20)]
    public float followXSpeed = 2f;
    public bool lockLookAt = true;

    private Vector3 velocity;
    private bool isFollowing = false;

    private void OnEnable()
    {
        Home.OnPlayPressed += HandlePlayPressed;
        EventManager.Instance.OnGameStarted += HandlePlayPressed;
    }

    private void OnDisable()
    {
        Home.OnPlayPressed -= HandlePlayPressed;
        EventManager.Instance.OnGameStarted -= HandlePlayPressed;
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
        Vector3 localOffset = new Vector3(introOffset.x, introOffset.y, introOffset.z);
        transform.position = localOffset;
        transform.rotation = Quaternion.Euler(10.67f, 67.1f, -0.289f);
    }

    private void LateUpdate()
    {
        if (!isFollowing || target == null) return;
        Vector3 targetPos = target.position + followOffset;
        float newZ = Mathf.SmoothDamp(transform.position.z, targetPos.z, ref velocity.z, smoothTime);
        float newY = Mathf.SmoothDamp(transform.position.y, targetPos.y, ref velocity.y, smoothTime);

        float newX = Mathf.Lerp(transform.position.x, targetPos.x, Time.deltaTime * followXSpeed);

        transform.position = new Vector3(newX, newY, newZ);



        if (!lockLookAt)
        {

            transform.LookAt(target.position + Vector3.up * 2f);
        }
        else
        {

            Quaternion targetRot = Quaternion.LookRotation(Vector3.forward + Vector3.down * 0.2f); // Hơi cúi xuống
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * 2f);
        }
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


        Quaternion endRot;
        if (lockLookAt)
        {
            endRot = Quaternion.LookRotation(Vector3.forward + Vector3.down * 0.4f);
        }
        else
        {
            endRot = Quaternion.LookRotation(target.position + Vector3.up * 2f - endPos);
        }

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