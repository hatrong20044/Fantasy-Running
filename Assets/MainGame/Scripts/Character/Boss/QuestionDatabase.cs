using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Question
{
    [TextArea(2, 4)]
    public string questionText;

    [Header("3 Đáp án (Lane Trái → Giữa → Phải)")]
    public string answer0;
    public string answer1;
    public string answer2;

    [Header("Đáp án đúng")]
    [Range(0, 2)]
    public int correctAnswerIndex;

    public string GetAnswer(int index)
    {
        switch (index)
        {
            case 0: return answer0;
            case 1: return answer1;
            case 2: return answer2;
            default: return "";
        }
    }
}

[CreateAssetMenu(fileName = "New Question Database", menuName = "Game/Question Database", order = 1)]
public class QuestionDatabase : ScriptableObject
{
    [Header("Tất cả câu hỏi")]
    public List<Question> allQuestions = new List<Question>();

    [Header("Runtime Data")]
    [SerializeField] private List<Question> remainingQuestions = new List<Question>();

    private bool isInitialized = false;

    public void Initialize()
    {
        ResetPool();
        isInitialized = true;
    }

    public Question GetRandomQuestion()
    {
        if (!isInitialized || remainingQuestions.Count == 0)
        {
            ResetPool();
        }

        int randomIndex = Random.Range(0, remainingQuestions.Count);
        Question selected = remainingQuestions[randomIndex];
        remainingQuestions.RemoveAt(randomIndex);

        return selected;
    }

    public void ResetPool()
    {
        remainingQuestions = new List<Question>(allQuestions);
        Debug.Log($"♻️ Reset pool: {allQuestions.Count} questions");
    }

    public int GetRemainingCount()
    {
        return remainingQuestions.Count;
    }
}