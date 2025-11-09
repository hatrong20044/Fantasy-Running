using HighScoreSaving;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
[CreateAssetMenu(fileName = "HighScoreDB", menuName = "High Score Save/HighScoreDB ")]
public class HighScoreData : ScriptableObject
{
    public List<Infor> infor;
    public int maxCount = 10;

    private static string SavePath => Path.Combine(Application.persistentDataPath + "highscore.json");

    private class SaveWrapper {public List<Infor> list; }

    public void LoadScores()
    {
        if (File.Exists(SavePath))
        {
            string json = File.ReadAllText(SavePath);
            SaveWrapper wrapper = JsonUtility.FromJson<SaveWrapper>(json);
            infor = wrapper?.list?? new List<Infor>();
        }
        else
        {
            infor = new List<Infor>();
        }
      //  Debug.Log("Save path =>" + SavePath);
    }

    public void SaveScores()
    {
        SaveWrapper wapper = new SaveWrapper { list = infor };
        string json = JsonUtility.ToJson(wapper, true);
        File.WriteAllText(SavePath, json);
    }

    public void IsHighScore(Infor user)
    {
        if (infor.Count == 0)
        {
            infor.Add(user);
            this.SaveScores();
            return;
        }
        infor.Add(user);
        infor.Sort((a, b) => b.highScore.CompareTo(a.highScore));
        if(infor.Count > maxCount)
        {
            infor.RemoveRange(maxCount, infor.Count - maxCount);
        }
        this.SaveScores();
       // Debug.Log("Save path =>" + SavePath);
    }

    public int count()
    {
        return infor.Count;
    }

}
