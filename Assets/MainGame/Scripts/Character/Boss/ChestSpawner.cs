using System.Collections;
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
    public float spawnDistanceZ = 20f;
    public float chestHeight = 0.5f;

    [Header("🎬 Animation Settings")]
    [Tooltip("Độ sâu dưới đất khi bắt đầu spawn")]
    public float undergroundDepth = -2f;
    [Tooltip("Thời gian chest nổi lên (giây)")]
    public float riseUpDuration = 1f;
    [Tooltip("Loại easing cho animation")]
    public AnimationCurve riseCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

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
    /// ✅ FIX: Spawn 3 rương với đáp án đúng duy nhất
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

        // ✅ FIX: Tạo danh sách đáp án với flag đúng/sai
        List<AnswerData> answers = new List<AnswerData>
        {
            new AnswerData(0, currentQuestion.answer0, currentQuestion.correctAnswerIndex == 0),
            new AnswerData(1, currentQuestion.answer1, currentQuestion.correctAnswerIndex == 1),
            new AnswerData(2, currentQuestion.answer2, currentQuestion.correctAnswerIndex == 2)
        };

        // ✅ FIX: Shuffle cả struct (cả nội dung và flag đúng/sai)
        ShuffleAnswersList(answers);

        float spawnZ = player.position.z + spawnDistanceZ;

        // Debug log để kiểm tra
        Debug.Log($"📝 Question: {currentQuestion.questionText}");
        Debug.Log($"   Correct Answer Index in Question: {currentQuestion.correctAnswerIndex}");
        Debug.Log($"   After Shuffle:");
        for (int i = 0; i < answers.Count; i++)
        {
            Debug.Log($"      Lane {i}: {answers[i].content} (Correct: {answers[i].isCorrect})");
        }

        // Spawn 3 chest với đáp án đã shuffle
        for (int laneIndex = 0; laneIndex < 3; laneIndex++)
        {
            // ✅ Vị trí bắt đầu: dưới đất
            Vector3 startPos = new Vector3(LANE_X[laneIndex], undergroundDepth, spawnZ);
            // ✅ Vị trí kết thúc: trên mặt đất
            Vector3 endPos = new Vector3(LANE_X[laneIndex], chestHeight, spawnZ);

            GameObject chestObj = ObjectPool.Instance.GetFromPoolQuynh(chestPoolTag);
            if (chestObj == null)
            {
                Debug.LogWarning($"⚠️ Pool '{chestPoolTag}' hết chest!");
                continue;
            }

            // ✅ Đặt chest ở vị trí dưới đất trước
            chestObj.transform.position = startPos;
            chestObj.transform.rotation = Quaternion.identity;

            // ✅ FIX: Lấy đáp án đã shuffle
            AnswerData answerData = answers[laneIndex];

            // Setup chest với data đúng
            Chest chestScript = chestObj.GetComponent<Chest>();
            if (chestScript != null)
            {
                chestScript.SetupAnswer(
                    laneIndex,                  // Lane index (0=Trái, 1=Giữa, 2=Phải)
                    answerData.content,         // Nội dung đáp án
                    answerData.isCorrect        // Flag đúng/sai
                );
            }

            // ✅ Bắt đầu animation nổi lên
            StartCoroutine(AnimateChestRiseUp(chestObj, startPos, endPos));
        }

        Debug.Log($"   Remaining: {questionDatabase.GetRemainingCount()} questions");
    }

    /// <summary>
    /// 🎬 Animation cho chest nổi lên từ dưới đất
    /// </summary>
    private IEnumerator AnimateChestRiseUp(GameObject chest, Vector3 startPos, Vector3 endPos)
    {
        float elapsed = 0f;

        while (elapsed < riseUpDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / riseUpDuration);

            // ✅ Dùng curve để có animation mượt hơn
            float curveValue = riseCurve.Evaluate(t);

            chest.transform.position = Vector3.Lerp(startPos, endPos, curveValue);

            yield return null;
        }

        // ✅ Đảm bảo chest ở đúng vị trí cuối
        chest.transform.position = endPos;
    }

    /// <summary>
    /// ✅ FIX: Shuffle list AnswerData (Fisher-Yates)
    /// </summary>
    private void ShuffleAnswersList(List<AnswerData> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int random = Random.Range(0, i + 1);
            AnswerData temp = list[i];
            list[i] = list[random];
            list[random] = temp;
        }
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

    /// <summary>
    /// ✅ Struct để lưu thông tin đáp án
    /// </summary>
    private struct AnswerData
    {
        public int originalIndex;   // Index gốc trong question (0,1,2)
        public string content;      // Nội dung đáp án
        public bool isCorrect;      // Flag đúng/sai

        public AnswerData(int index, string text, bool correct)
        {
            originalIndex = index;
            content = text;
            isCorrect = correct;
        }
    }
}