using UnityEngine;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class QuestionGenerator : MonoBehaviour
{
    [Header("🎯 Target Database")]
    [Tooltip("Kéo QuestionDatabase vào đây")]
    public QuestionDatabase targetDatabase;

    [Header("⚙️ Settings")]
    public int minNumber = 1;
    public int maxNumber = 20;

    [Header("📊 Question Count")]
    public int additionCount = 20;
    public int subtractionCount = 20;
    public int multiplicationCount = 20;
    public int divisionCount = 20;

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

        Undo.RecordObject(targetDatabase, "Generate Questions");
        targetDatabase.allQuestions.Clear();

        Debug.Log("🔄 Generating questions...");

        GenerateAddition(additionCount);
        GenerateSubtraction(subtractionCount);
        GenerateMultiplication(multiplicationCount);
        GenerateDivision(divisionCount);

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

    private void GenerateMultiplication(int count)
    {
        for (int i = 0; i < count; i++)
        {
            int a = Random.Range(2, 13);
            int b = Random.Range(2, 13);
            int correct = a * b;
            AddQuestion($"{a} × {b} = ?", correct);
        }
    }

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
    /// ✅ ULTIMATE FIX: Đảm bảo 3 đáp án HOÀN TOÀN UNIQUE
    /// </summary>
    private void AddQuestion(string questionText, int correctAnswer)
    {
        // Tạo HashSet để đảm bảo unique (bao gồm cả đáp án đúng)
        HashSet<int> allAnswers = new HashSet<int> { correctAnswer };

        int attempts = 0;
        int maxAttempts = 100;

        // Tạo đáp án sai gần với đáp án đúng
        while (allAnswers.Count < 3 && attempts < maxAttempts)
        {
            int offset = Random.Range(-wrongAnswerRange, wrongAnswerRange + 1);

            if (offset == 0) // Bỏ qua offset 0
            {
                attempts++;
                continue;
            }

            int wrongAnswer = correctAnswer + offset;

            // Chỉ thêm nếu > 0 và chưa tồn tại
            if (wrongAnswer > 0)
            {
                allAnswers.Add(wrongAnswer);
            }

            attempts++;
        }

        // Fallback: Nếu chưa đủ 3, dùng số xa hơn
        attempts = 0;
        while (allAnswers.Count < 3 && attempts < maxAttempts)
        {
            int farOffset = Random.Range(wrongAnswerRange * 2, wrongAnswerRange * 5);
            if (Random.value > 0.5f) farOffset = -farOffset;

            int wrongAnswer = correctAnswer + farOffset;

            if (wrongAnswer > 0)
            {
                allAnswers.Add(wrongAnswer);
            }

            attempts++;
        }

        // Fallback cuối: Dùng công thức đảm bảo unique
        if (allAnswers.Count < 3)
        {
            Debug.LogWarning($"⚠️ Fallback for {correctAnswer}");

            if (allAnswers.Count == 1) // Chỉ có đáp án đúng
            {
                allAnswers.Add(correctAnswer + 10);
                allAnswers.Add(correctAnswer + 20);
            }
            else if (allAnswers.Count == 2) // Còn thiếu 1
            {
                allAnswers.Add(correctAnswer - 15 > 0 ? correctAnswer - 15 : correctAnswer + 15);
            }
        }

        // Convert sang List và shuffle
        List<int> answersList = new List<int>(allAnswers);
        ShuffleList(answersList);

        // Tìm index của đáp án đúng sau shuffle
        int correctIndex = answersList.IndexOf(correctAnswer);

        // Tạo Question
        Question q = new Question
        {
            questionText = questionText,
            answer0 = answersList[0].ToString(),
            answer1 = answersList[1].ToString(),
            answer2 = answersList[2].ToString(),
            correctAnswerIndex = correctIndex
        };

        // ✅ VERIFY: Kiểm tra trước khi add
        if (q.answer0 == q.answer1 || q.answer0 == q.answer2 || q.answer1 == q.answer2)
        {
            Debug.LogError($"❌ BUG DETECTED: Duplicate answers! Q: {questionText} | Answers: [{q.answer0}, {q.answer1}, {q.answer2}]");
            return; // Không add câu hỏi lỗi
        }

        targetDatabase.allQuestions.Add(q);
    }

    private void ShuffleList(List<int> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);
            int temp = list[i];
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }

    
#endif
}