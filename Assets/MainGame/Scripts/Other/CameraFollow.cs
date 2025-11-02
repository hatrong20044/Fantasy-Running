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
        EventManager.Instance.OnGameStarted += HandlePlayPressed;
    }

    private void OnDisable()
    {
        Home.OnPlayPressed -= HandlePlayPressed;
        EventManager.Instance.OnGameStarted -= HandlePlayPressed;
    }

    private void Start()
    {
        if (target)
        {
            
            Vector3 introPos = target.position + target.transform.forward * introOffset.z + Vector3.up * introOffset.y;
            transform.position = introPos;

            transform.LookAt(target.position + Vector3.up * 1.5f);
        }
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
