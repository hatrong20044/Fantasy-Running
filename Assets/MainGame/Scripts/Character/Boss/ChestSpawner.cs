using System.Collections.Generic;
using UnityEngine;

public class ChestSpawner : MonoBehaviour
{
    [Header("📚 Question Database")]
    [Tooltip("Kéo file QuestionDatabase vào đây")]
    public QuestionDatabase questionDatabase;

    [Header("Pool Settings")]
    public string chestPoolTag = "Chest";

    [Header("Spawn Settings")]
    public float spawnDistanceZ = 15f;
    public float chestHeight = 0.5f;

    [Header("References")]
    public Transform player;

    // 3 lanes cố định
    private static readonly float[] LANE_X = { -2.5f, 0f, 2.5f };

    private Question currentQuestion;

    private void Start()
    {
        if (player == null)
        {
            player = FindObjectOfType<Player>()?.transform;
        }

        // Initialize database
        if (questionDatabase != null)
        {
            questionDatabase.Initialize();
            Debug.Log($"✅ QuestionDatabase initialized with {questionDatabase.allQuestions.Count} questions");
        }
        else
        {
            Debug.LogError("❌ QuestionDatabase chưa được gán!");
        }
    }

    /// <summary>
    /// Spawn 3 rương với 1 câu hỏi (mỗi rương 1 đáp án)
    /// </summary>
    public void SpawnChestWave()
    {
        if (!ValidateReferences()) return;

        // Lấy câu hỏi random từ database
        currentQuestion = questionDatabase.GetRandomQuestion();

        if (currentQuestion == null)
        {
            Debug.LogError("❌ Không lấy được câu hỏi!");
            return;
        }

        // Shuffle đáp án (random vị trí)
        List<int> shuffledIndices = ShuffleAnswers();

        float spawnZ = player.position.z + spawnDistanceZ;

        // Spawn 3 chest
        for (int laneIndex = 0; laneIndex < 3; laneIndex++)
        {
            Vector3 spawnPos = new Vector3(LANE_X[laneIndex], chestHeight, spawnZ);

            GameObject chestObj = ObjectPool.Instance.GetFromPoolQuynh(chestPoolTag);
            if (chestObj == null)
            {
                Debug.LogWarning($"⚠️ Pool '{chestPoolTag}' hết chest!");
                continue;
            }

            chestObj.transform.position = spawnPos;
            chestObj.transform.rotation = Quaternion.identity;

            // Get answer cho lane này
            int answerIndex = shuffledIndices[laneIndex];
            string answerText = currentQuestion.GetAnswer(answerIndex);
            bool isCorrect = (answerIndex == currentQuestion.correctAnswerIndex);

            // Setup chest
            Chest chestScript = chestObj.GetComponent<Chest>();
            if (chestScript != null)
            {
                chestScript.SetupAnswer(answerIndex, answerText, isCorrect);
            }
        }

        Debug.Log($"📝 Question: {currentQuestion.questionText}");
        Debug.Log($"   Remaining: {questionDatabase.GetRemainingCount()} questions");
    }

    /// <summary>
    /// Shuffle đáp án để random vị trí (Fisher-Yates)
    /// </summary>
    private List<int> ShuffleAnswers()
    {
        List<int> indices = new List<int> { 0, 1, 2 };

        for (int i = indices.Count - 1; i > 0; i--)
        {
            int random = Random.Range(0, i + 1);
            (indices[i], indices[random]) = (indices[random], indices[i]);
        }

        return indices;
    }

    /// <summary>
    /// Validate references
    /// </summary>
    private bool ValidateReferences()
    {
        if (player == null)
        {
            Debug.LogError("❌ Player reference null!");
            return false;
        }

        if (ObjectPool.Instance == null)
        {
            Debug.LogError("❌ ObjectPool chưa khởi tạo!");
            return false;
        }

        if (questionDatabase == null)
        {
            Debug.LogError("❌ QuestionDatabase chưa được gán!");
            return false;
        }

        return true;
    }

    /// <summary>
    /// Get current question text (để hiển thị UI nếu cần)
    /// </summary>
    public string GetCurrentQuestionText()
    {
        return currentQuestion?.questionText ?? "";
    }
}