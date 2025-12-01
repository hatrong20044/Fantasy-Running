
using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
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
        [SerializeField] private List<GameObject> displayer;

        private void Awake()
        {
            data.LoadScores();
        }
        public void UpdateTable(int score)
        {
            Infor user = new(GameData.Instance.UserName, score);
            data.IsHighScore(user);
        }

        public void DisplayContent()
        {
            for(int i = 0; i < data.count(); i++)
            {
                string userName = data.infor[i].userName;
                int score = data.infor[i].highScore;
                displayer[i].transform.Find("name").GetComponent<TMPro.TMP_Text>().text = userName;
                displayer[i].transform.Find("score").GetComponent<TMPro.TMP_Text>().text = score.ToString();
            }
            for(int i = data.count(); i < data.maxCount; i++)
            {
                displayer[i].SetActive(false);
            }
            this.MaxPoint();
        }

        public void MaxPoint()
        {
            for(int i = 0; i < data.count(); i++)
            {
                string userName = data.infor[i].userName;
                if(userName == GameData.Instance.UserName)
                {
                    Color color;
                    ColorUtility.TryParseHtmlString("#F5EC5E", out color);
                    displayer[i].GetComponent<Image>().color = color;
                    return;
                }
            }
        }

        //public void DisplayTable()
        //{

        //    if (data == null || data.infor == null || data.infor.Count == 0)
        //    {
        //        displayer.text = "Chưa có dữ liệu điểm cao!";
        //        return;
        //    }

        //    System.Text.StringBuilder sb = new System.Text.StringBuilder();

        //    for (int i = 0; i < data.infor.Count; i++)
        //    {
        //        var entry = data.infor[i];
        //        sb.AppendLine($"{i + 1}. {entry.userName} - {entry.highScore}");
        //    }

        //    displayer.text = sb.ToString();
        //}
    }
}

