using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Introduction : MonoBehaviour
{

   public void OpenIntroUI()
    {
        UIManager.Instance.ShowUI(UIName.Introduction);
    }

    public void CloseIntroUI()
    {
        UIManager.Instance.HideUI(UIName.Introduction);
    }

}
