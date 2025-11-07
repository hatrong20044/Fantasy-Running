using HighScoreSaving;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "HighScoreDB", menuName = "High Score Save/HighScoreDB ")]
public class HighScoreData : ScriptableObject
{
    public List<Infor> infor;
    public int maxCount = 10;
    public void IsHighScore(Infor user)
    {
        if (infor.Count == 0)
        {
            infor.Add(user);
            return;
        }
        infor.Add(user);
        infor.Sort((a, b) => b.highScore.CompareTo(a.highScore));
        if(infor.Count > maxCount)
        {
            infor.RemoveRange(maxCount, infor.Count - maxCount);
        }
    }

    public int count()
    {
        return infor.Count;
    }

}
