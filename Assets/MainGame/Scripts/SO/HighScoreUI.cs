
using System;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

namespace HighScoreSaving
{
    [Serializable]
    public struct Infor
    {
        public string userName;
        public int highScore;
        public Infor(string name, int score)
        {
            userName = name;
            highScore = score;
        }
    }
    public class HighScoreUI : MonoBehaviour
    {

        [Header("High Score Data")]
        [SerializeField] private HighScoreData data;
        [SerializeField] private TMPro.TMP_Text displayer;

        public HighScoreData GetData()
        {
            return this.data;
        }
        public void UpdateTable(int score)
        {
            Infor user = new(GameData.Instance.UserName, score);
            data.IsHighScore(user);
        }
        public void DisplayTable()
        {

            if (data == null || data.infor == null || data.infor.Count == 0)
            {
                displayer.text = "Chưa có dữ liệu điểm cao!";
                return;
            }

            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            for (int i = 0; i < data.infor.Count; i++)
            {
                var entry = data.infor[i];
                sb.AppendLine($"{i + 1}. {entry.userName} - {entry.highScore}");
            }

            displayer.text = sb.ToString();
        }
    }
}

