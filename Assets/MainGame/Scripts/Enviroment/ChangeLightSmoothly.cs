using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeLightSmoothly : MonoBehaviour
{
    public Light directionalLight;        // Kéo Directional Light vào đây
    public Color targetColor = Color.red; // Màu đích
    public float duration = 5f;           // Thời gian chuyển: 5 giây

    private float startIntensity;
    private Color startColor;
    public float targetIntensity = 1.12f;
    private bool isTransitioning = false;
    public Player player;
    private bool hasTriggered = false;
    [SerializeField] private float triggerPoint = 20f;

    [Header("Skybox Settings")]
    public Material skyboxMaterial;
    private float startTransitionValue = 0.8f;

    private void Awake()
    {
        skyboxMaterial.SetFloat("_CubemapTransition", startTransitionValue);
    }

    void Start()
    {
        startIntensity = directionalLight.intensity;
        startColor = directionalLight.color;
        startTransitionValue = skyboxMaterial.GetFloat("_CubemapTransition");
    }

    private void Update()
    {
        if (player.transform.position.z > triggerPoint)
        {
            OnStartChangeLight();
        }
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

    private IEnumerator ChangeColorOverTime()
    {
        isTransitioning = true;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration; // từ 0 → 1

            // Dùng Color.Lerp để chuyển mượt
            directionalLight.color = Color.Lerp(startColor, targetColor, t);

            // Fade Intensity
            directionalLight.intensity = Mathf.Lerp(startIntensity, targetIntensity, t);

            float targetTransition = 0f; // Chuyển SANG cubemap thứ 2 (blend=1)
            float currentTransition = Mathf.Lerp(startTransitionValue, targetTransition, t);
            skyboxMaterial.SetFloat("_CubemapTransition", currentTransition);

            yield return null; // Chờ frame tiếp theo
        }

        // Đảm bảo màu cuối đúng 100%
        directionalLight.intensity = targetIntensity;
        directionalLight.color = targetColor;
        skyboxMaterial.SetFloat("_CubemapTransition", 0f);
        isTransitioning = false;
    }
}