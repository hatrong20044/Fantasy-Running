using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleMovement : MonoBehaviour
{
    [SerializeField] private float speed = 30f;
    [SerializeField] private float warningDuration = 1f; // Th?i gian c?nh báo
    [SerializeField] private GameObject warningImage; // Tham chi?u ??n Warning Image con

    private bool isMoving = false;

    void Start()
    {
        // ?n c?nh báo ban ??u
        if (warningImage != null)
        {
            warningImage.gameObject.SetActive(false);
        }
    }

    public void Act()
    {
        StartCoroutine(this.ActivateWarning());
    }

    private IEnumerator ActivateWarning()
    {
        Debug.Log("ActivateWarning started");
        if (warningImage != null)
        {
            Debug.Log("Showing warning image");
            warningImage.gameObject.SetActive(true);
            yield return StartCoroutine(BlinkWarning());
            Debug.Log("BlinkWarning finished");
            warningImage.gameObject.SetActive(false);
        }
        isMoving = true; // B?t ??u di chuy?n sau c?nh báo
    }

    private IEnumerator BlinkWarning()
    {
        for (int i = 0; i < 6; i++) // Nh?p nháy 6 l?n (3 l?n on/off)
        {
            warningImage.SetActive(true);
            yield return new WaitForSeconds(0.1f);
            warningImage.SetActive(false);
            yield return new WaitForSeconds(0.1f);
        }
    }

    void Update()
    {
        if (isMoving)
        {
            // Di chuy?n xe v? phía tr??c (gi? s? h??ng là -Z)
            transform.Translate(Vector3.forward * speed * Time.deltaTime);
        }
    }

    public void resetMoving()
    {
        this.isMoving = false;
    }

    // Xóa xe khi ra kh?i màn hình (tùy ch?n)
    void OnBecameInvisible()
    {
        Destroy(gameObject);
    }
}
