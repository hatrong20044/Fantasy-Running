using HighScoreSaving;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class RankDisplayer : MonoBehaviour
{
    [SerializeField] private HighScoreUI table;
    private void Start()
    {
        this.DisplayResult();
    }

    public void DisplayResult()
    {

        this.table.DisplayTable();
    }


    public void OpenHighScore()
    {
        UIManager.Instance.ShowUI(UIName.HighScore);
    }

    public void CloseHighScore()
    {
        UIManager.Instance.HideUI(UIName.HighScore);
    }
}
