using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class ChestSpawner : MonoBehaviour
{
    [Header("📚 Question Database")]
    public QuestionDatabase questionDatabase;

    [Header("🧰 Prefabs")]
    public GameObject chestPrefab;
    public GameObject questionGatePrefab;

    [Header("👤 References")]
    public Transform player;

    [Header("📍 Spawn Distance")]
    [Tooltip("Khoảng cách Gate luôn cách player")]
    public float gateDistanceFromPlayer = 15f;
    [Tooltip("Khoảng cách spawn rương (Chest) tính từ vị trí player")]
    public float chestSpawnDistanceZ = 25f;

    [Header("📦 Chest Settings")]
    public float chestHeight = 0.5f;
    public float undergroundDepth = -2f;
    public float chestRiseDuration = 1f;
    public AnimationCurve chestRiseCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Header("🚪 Question Gate Settings")]
    public float gateLeftX = -3.24f;
    public float gateRightX = 3.24f;
    public float gateFixedY = 7.36f;

    private Question currentQuestion;
    private QuestionGate leftGate;
    private QuestionGate rightGate;
    private bool gatesSpawned = false;

    private static readonly float[] LANE_X = { -2.5f, 0f, 2.5f };

    private void Start()
    {
        if (player == null)
            player = FindObjectOfType<Player>()?.transform;

        if (questionDatabase != null)
        {
            questionDatabase.Initialize();
        }
    }

    private void Update()
    {
        // 👉 Cập nhật vị trí Gate liên tục theo player
        if (gatesSpawned && player != null)
        {
            float targetZ = player.position.z + gateDistanceFromPlayer;

            if (leftGate != null)
                leftGate.transform.position = new Vector3(gateLeftX, gateFixedY, targetZ);

            if (rightGate != null)
                rightGate.transform.position = new Vector3(gateRightX, gateFixedY, targetZ);
        }
    }

    // =========================================================
    // 🟢 1️⃣  SPAWN QUESTION GATES (chỉ gọi 1 lần duy nhất)
    // =========================================================
    public void SpawnQuestionGates()
    {
        if (gatesSpawned) return; // 👈 Đã spawn rồi thì thôi

        if (!ValidateReferences()) return;
        if (questionGatePrefab == null)
        {
            return;
        }

        float spawnZ = player.position.z + gateDistanceFromPlayer;

        Vector3 leftPos = new Vector3(gateLeftX, gateFixedY, spawnZ);
        Vector3 rightPos = new Vector3(gateRightX, gateFixedY, spawnZ);

        GameObject leftObj = Instantiate(questionGatePrefab, leftPos, Quaternion.identity);
        GameObject rightObj = Instantiate(questionGatePrefab, rightPos, Quaternion.identity);

        leftGate = leftObj.GetComponent<QuestionGate>();
        rightGate = rightObj.GetComponent<QuestionGate>();

        gatesSpawned = true;
    }

    // =========================================================
    // 🟡 2️⃣  SPAWN CHESTS
    // =========================================================
    public void SpawnChestWave()
    {
        if (!ValidateReferences()) return;

        currentQuestion = questionDatabase.GetRandomQuestion();
        if (currentQuestion == null)
        {
            return;
        }

        List<AnswerData> answers = new List<AnswerData>
        {
            new AnswerData(0, currentQuestion.answer0, currentQuestion.correctAnswerIndex == 0),
            new AnswerData(1, currentQuestion.answer1, currentQuestion.correctAnswerIndex == 1),
            new AnswerData(2, currentQuestion.answer2, currentQuestion.correctAnswerIndex == 2)
        };

        ShuffleAnswersList(answers);
        float spawnZ = player.position.z + chestSpawnDistanceZ;

        for (int i = 0; i < 3; i++)
        {
            Vector3 startPos = new Vector3(LANE_X[i], undergroundDepth, spawnZ);
            Vector3 endPos = new Vector3(LANE_X[i], chestHeight, spawnZ);

            GameObject chestObj = ObjectPool.Instance.GetFromPoolQuynh("Chest");

            if (chestObj != null)
            {
                chestObj.transform.position = startPos;
                chestObj.transform.rotation = Quaternion.identity;

                Chest chest = chestObj.GetComponent<Chest>();
                if (chest != null)
                {
                    chest.SetupAnswer(i, answers[i].content, answers[i].isCorrect);
                }

                StartCoroutine(AnimateChestRiseUp(chestObj, startPos, endPos));
            }
        }
    }

    private IEnumerator AnimateChestRiseUp(GameObject chest, Vector3 startPos, Vector3 endPos)
    {
        float elapsed = 0f;

        while (elapsed < chestRiseDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / chestRiseDuration);
            float curveValue = chestRiseCurve.Evaluate(t);
            chest.transform.position = Vector3.Lerp(startPos, endPos, curveValue);
            yield return null;
        }

        chest.transform.position = endPos;
    }

    // =========================================================
    // 🔵 3️⃣  HIỂN THỊ / ẨN CÂU HỎI TRÊN GATE
    // =========================================================
    public void ShowQuestion()
    {
        string questionText = GetCurrentQuestionText();
        if (leftGate != null) leftGate.ShowQuestion(questionText);
        if (rightGate != null) rightGate.ShowQuestion(questionText);
    }

    public void HideQuestionGates()
    {
        if (leftGate != null) leftGate.HideQuestion();
        if (rightGate != null) rightGate.HideQuestion();
    }

    // =========================================================
    // 🔧 UTILITIES
    // =========================================================
    private string GetCurrentQuestionText()
    {
        return currentQuestion?.questionText ?? "";
    }

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

    private bool ValidateReferences()
    {
        if (player == null)
        {
            return false;
        }

        if (questionDatabase == null)
        {
            return false;
        }

        return true;
    }

    public void DestroyQuestionGates()
    {
        if (leftGate != null)
        {
            Destroy(leftGate.gameObject);
            leftGate = null;
        }

        if (rightGate != null)
        {
            Destroy(rightGate.gameObject);
            rightGate = null;
        }

        gatesSpawned = false;
    }

    private struct AnswerData
    {
        public int index;
        public string content;
        public bool isCorrect;

        public AnswerData(int idx, string text, bool correct)
        {
            index = idx;
            content = text;
            isCorrect = correct;
        }
    }
}