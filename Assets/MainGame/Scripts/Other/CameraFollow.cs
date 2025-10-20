using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Target")]
    public Transform target;

    [Header("Settings")]
    public Vector3 offset = new Vector3(0, 4, -6);
    public float smoothTimeX = 0.15f;   
    public float smoothTimeY = 0.1f;    
    public float smoothTimeZ = 0.05f;   

    private Vector3 velocity = Vector3.zero;

    private void LateUpdate()
    {
        if (target == null) return;

        Vector3 desiredPos = target.position + offset;

        Vector3 smoothedPos = transform.position;

        smoothedPos.x = Mathf.SmoothDamp(transform.position.x, desiredPos.x, ref velocity.x, smoothTimeX);
        smoothedPos.y = Mathf.SmoothDamp(transform.position.y, desiredPos.y, ref velocity.y, smoothTimeY);
        smoothedPos.z = Mathf.SmoothDamp(transform.position.z, desiredPos.z, ref velocity.z, smoothTimeZ);

        transform.position = smoothedPos;

       
        transform.LookAt(target.position + Vector3.up * 2f);
    }
}
