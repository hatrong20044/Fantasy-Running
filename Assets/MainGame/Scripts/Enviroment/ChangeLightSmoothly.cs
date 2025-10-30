using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeLightSmoothly : MonoBehaviour
{
    public Light directionalLight;        // Kéo Directional Light vào đây
    public Color targetColor = Color.red; // Màu đích
    public float duration = 2f;           // Thời gian chuyển: 2 giây

    public Color startColor;
    private bool isTransitioning = false;
    public Player player;
    private bool hasTriggered = false;
    [SerializeField] private float triggerPoint = 20f;

    void Start()
    {
        startColor = directionalLight.color; // Lưu màu ban đầu
    }

    private void Update()
    {
        if(player.transform.position.z > triggerPoint)
        {
            OnStartChangeLight();
        }
    }

    IEnumerator ChangeLightColorBack()
    {
        float elapsed = 0f;
        Color current = directionalLight.color;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            directionalLight.color = Color.Lerp(current, startColor, t);
            yield return null;
        }

        directionalLight.color = startColor;
        hasTriggered = false;
    }

    public void StartColorChangeBack()
    {
        if (!isTransitioning)
            StartCoroutine(ChangeLightColorBack());
    }

    private void OnStartChangeLight()
    {
        if (hasTriggered) return;

        hasTriggered = true;
        StartColorChange();
    }

    // Gọi hàm này để bắt đầu chuyển màu
    public void StartColorChange()
    {
        if (!isTransitioning)
            StartCoroutine(ChangeColorOverTime());
    }

    IEnumerator ChangeColorOverTime()
    {
        isTransitioning = true;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration; // từ 0 → 1

            // Dùng Color.Lerp để chuyển mượt
            directionalLight.color = Color.Lerp(startColor, targetColor, t);

            yield return null; // Chờ frame tiếp theo
        }

        // Đảm bảo màu cuối đúng 100%
        directionalLight.color = targetColor;
        isTransitioning = false;
    }
}
