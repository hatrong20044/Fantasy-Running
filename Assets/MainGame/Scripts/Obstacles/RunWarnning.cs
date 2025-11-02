using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunWarnning : MonoBehaviour
{
    public float warningDuration = 1f; // Th?i gian c?nh báo
    [SerializeField] private GameObject warningImage; // Tham chi?u ??n Warning Image con

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
        if (warningImage != null)
        {
            warningImage.gameObject.SetActive(true);
            yield return StartCoroutine(BlinkWarning());
            warningImage.gameObject.SetActive(false);
        }
    }

    private IEnumerator BlinkWarning()
    {
        float blinkInterval = warningDuration / 6f; // Chia đều cho 6 lần nhấp nháy (3 lần on/off)
        for (int i = 0; i < 6; i++) // Nhấp nháy 6 lần (3 lần bật/tắt)
        {
            warningImage.SetActive(i % 2 == 0); // Bật khi i là chẵn, tắt khi i là lẻ
            yield return new WaitForSeconds(blinkInterval); // Chờ thời gian được tính
        }
    }

    public void SetTimeWarning(float time)
    {
        warningDuration = time;
    }
}
