using UnityEngine;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

// ⭐ Tool để tự động generate câu hỏi toán học
public class QuestionGenerator : MonoBehaviour
{
    [Header("🎯 Target Database")]
    [Tooltip("Kéo QuestionDatabase vào đây")]
    public QuestionDatabase targetDatabase;

    [Header("⚙️ Settings")]
    public int minNumber = 1;
    public int maxNumber = 20;

    [Header("📊 Question Count")]
    public int additionCount = 20;       // Số câu phép cộng
    public int subtractionCount = 20;    // Số câu phép trừ
    public int multiplicationCount = 20; // Số câu phép nhân
    public int divisionCount = 20;       // Số câu phép chia

    [Header("🎲 Difficulty")]
    [Range(1, 10)]
    [Tooltip("Độ lệch đáp án sai (1=dễ, 10=khó)")]
    public int wrongAnswerRange = 3;

#if UNITY_EDITOR
    [ContextMenu("🚀 Generate All Math Questions")]
    public void GenerateAllQuestions()
    {
        if (targetDatabase == null)
        {
            Debug.LogError("❌ Chưa gán Target Database!");
            EditorUtility.DisplayDialog("Error", "Vui lòng gán QuestionDatabase vào Target Database!", "OK");
            return;
        }

        // Record undo
        Undo.RecordObject(targetDatabase, "Generate Questions");

        // Clear old questions
        targetDatabase.allQuestions.Clear();

        // Generate từng loại
        Debug.Log("🔄 Generating questions...");

        GenerateAddition(additionCount);
        GenerateSubtraction(subtractionCount);
        GenerateMultiplication(multiplicationCount);
        GenerateDivision(divisionCount);

        // Save
        EditorUtility.SetDirty(targetDatabase);
        AssetDatabase.SaveAssets();

        int total = targetDatabase.allQuestions.Count;
        Debug.Log($"✅ Generated {total} questions!");
        EditorUtility.DisplayDialog("Success",
            $"✅ Đã tạo {total} câu hỏi!\n\n" +
            $"➕ Cộng: {additionCount}\n" +
            $"➖ Trừ: {subtractionCount}\n" +
            $"✖️ Nhân: {multiplicationCount}\n" +
            $"➗ Chia: {divisionCount}",
            "OK");
    }

    // ➕ Generate phép cộng
    private void GenerateAddition(int count)
    {
        for (int i = 0; i < count; i++)
        {
            int a = Random.Range(minNumber, maxNumber);
            int b = Random.Range(minNumber, maxNumber);
            int correct = a + b;

            AddQuestion($"{a} + {b} = ?", correct);
        }
    }

    // ➖ Generate phép trừ
    private void GenerateSubtraction(int count)
    {
        for (int i = 0; i < count; i++)
        {
            int a = Random.Range(minNumber + 5, maxNumber);
            int b = Random.Range(minNumber, a);
            int correct = a - b;

            AddQuestion($"{a} - {b} = ?", correct);
        }
    }

    // ✖️ Generate phép nhân
    private void GenerateMultiplication(int count)
    {
        for (int i = 0; i < count; i++)
        {
            int a = Random.Range(2, 13);  // Bảng cửu chương
            int b = Random.Range(2, 13);
            int correct = a * b;

            AddQuestion($"{a} × {b} = ?", correct);
        }
    }

    // ➗ Generate phép chia
    private void GenerateDivision(int count)
    {
        for (int i = 0; i < count; i++)
        {
            int divisor = Random.Range(2, 11);
            int quotient = Random.Range(2, 11);
            int dividend = divisor * quotient;

            AddQuestion($"{dividend} ÷ {divisor} = ?", quotient);
        }
    }

    /// <summary>
    /// Tạo 1 question và add vào database
    /// </summary>
    private void AddQuestion(string questionText, int correctAnswer)
    {
        List<int> wrongs = GenerateWrongAnswers(correctAnswer);

        // Tạo list gồm đúng + sai
        List<int> allAnswers = new List<int>(wrongs);
        int correctIndex = Random.Range(0, 3);
        allAnswers.Insert(correctIndex, correctAnswer); // chèn đúng vào vị trí ngẫu nhiên

        Question q = new Question
        {
            questionText = questionText,
            answer0 = allAnswers[0].ToString(),
            answer1 = allAnswers[1].ToString(),
            answer2 = allAnswers[2].ToString(),
            correctAnswerIndex = correctIndex
        };

        targetDatabase.allQuestions.Add(q);
    }

    /// <summary>
    /// Tạo 3 đáp án unique (gần với đáp án đúng)
    /// </summary>
    private List<int> GenerateWrongAnswers(int correctAnswer)
    {
        HashSet<int> wrongs = new HashSet<int>();
        int attempts = 0;

        while (wrongs.Count < 2 && attempts < 100)
        {
            int offset = Random.Range(-wrongAnswerRange, wrongAnswerRange + 1);
            if (offset == 0)
            {
                attempts++;
                continue;
            }

            int wrong = correctAnswer + offset;

            // Phải khác đúng và > 0
            if (wrong > 0 && wrong != correctAnswer)
                wrongs.Add(wrong);

            attempts++;
        }

        // Nếu vẫn thiếu thì sinh thêm cách xa hơn
        while (wrongs.Count < 2)
        {
            int extra = correctAnswer + Random.Range(2, 10);
            if (extra != correctAnswer)
                wrongs.Add(extra);
        }

        return new List<int>(wrongs);
    }
#endif
}