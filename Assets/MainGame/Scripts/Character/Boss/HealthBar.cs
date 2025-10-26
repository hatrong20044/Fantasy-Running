using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Image ProgressImage;
    [SerializeField] private float DefaultSpeed = 1f;
    [SerializeField] private Gradient ColorGradient;
    [SerializeField] private UnityEvent<float> OnProgress;
    [SerializeField] UnityEvent OnCompleted;

    private void Start()
    {
        if (ProgressImage.type != Image.Type.Filled)
        {
            Debug.LogError($"{name}'s ProgressImage is not of type \"Filled\" so it cannot be used as a progress bar. Disabling this Progress Bar.");
            enabled = false;
#if UNITY_EDITOR
            EditorGUIUtility.PingObject(this.gameObject);
#endif
        }
    }

    private Coroutine AnimationCoroutine;

    public void SetProgress(float Progress)
    {
        SetProgress(Progress, DefaultSpeed);
    }

    public void SetProgress(float Progress, float Speed)
    {
        if (Progress < 0 || Progress > 1)
        {
            Debug.LogWarning($"Invalid progress passed, expected value is between 0 and 1, got {Progress}. Clamping.");
            Progress = Mathf.Clamp01(Progress);
        }
        if (Progress != ProgressImage.fillAmount)
        {
            if (AnimationCoroutine != null)
            {
                StopCoroutine(AnimationCoroutine);
            }

            ActivateHierarchy(transform.root.gameObject);

            Debug.Log($"{name} activeSelf={gameObject.activeSelf}, activeInHierarchy={gameObject.activeInHierarchy}");
            Debug.Log($"Canvas activeInHierarchy={transform.parent.gameObject.activeInHierarchy}");
            Debug.Log($"Boss root activeInHierarchy={transform.root.gameObject.activeInHierarchy}");
            AnimationCoroutine = StartCoroutine(AnimateProgress(Progress, Speed));
        }
    }

    public void ActivateHierarchy(GameObject root)
    {
        if (root == null) return;

        // Kích hoạt đối tượng root
        root.SetActive(true);

        // Duyệt qua tất cả các con và kích hoạt chúng
        foreach (Transform child in root.transform)
        {
            Debug.Log(child.gameObject.active);
            ActivateHierarchy(child.gameObject);
        }
    }

    private IEnumerator AnimateProgress(float Progress, float Speed)
    {
        float time = 0;
        float initialProgress = ProgressImage.fillAmount;

        while (time < 1)
        {
            ProgressImage.fillAmount = Mathf.Lerp(initialProgress, Progress, time);
            time += Time.deltaTime * Speed;

            ProgressImage.color = ColorGradient.Evaluate(1 - ProgressImage.fillAmount);

            OnProgress?.Invoke(ProgressImage.fillAmount);
            yield return null;
        }

        ProgressImage.fillAmount = Progress;
        ProgressImage.color = ColorGradient.Evaluate(1 - ProgressImage.fillAmount);

        OnProgress?.Invoke(Progress);
        OnCompleted?.Invoke();
    }
}
