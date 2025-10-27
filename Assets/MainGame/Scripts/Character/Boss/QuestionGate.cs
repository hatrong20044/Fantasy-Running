using UnityEngine;
using TMPro;

public class QuestionGate : MonoBehaviour
{
    [Header("UI Question Text")]
    public TMP_Text questionText;  // Gán TMP_Text vào đây trong Inspector

    private bool isVisible = false;

    private void Awake()
    {
        if (questionText == null)
            questionText = GetComponentInChildren<TMP_Text>();
    }

    // 👉 Gọi hàm này khi muốn hiển thị câu hỏi
    public void ShowQuestion(string text)
    {
        if (questionText == null) return;

        questionText.text = text;
        questionText.gameObject.SetActive(true);
        isVisible = true;
    }

    // 👉 Gọi hàm này khi muốn ẩn câu hỏi
    public void HideQuestion()
    {
        if (questionText == null) return;

        questionText.gameObject.SetActive(false);
        isVisible = false;
    }

    // (Tuỳ chọn) Nếu muốn kiểm tra trạng thái
    public bool IsVisible()
    {
        return isVisible;
    }
}
