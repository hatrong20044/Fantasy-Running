using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitUI : MonoBehaviour
{
    public void OpenExitUI()
    {
        UIManager.Instance.ShowUI(UIName.Exit);
    }
    public void HideExitUI()
    {
        UIManager.Instance.HideUI(UIName.Exit);
    }

    public void ConfirmExit()
    {
        Debug.Log("Thoát game...");

#if UNITY_EDITOR
        
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

}
