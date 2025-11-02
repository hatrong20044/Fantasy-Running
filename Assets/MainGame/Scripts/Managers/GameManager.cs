
using System.Collections;
using UnityEngine;


public class GameManager : MonoBehaviour
{
    private void Start()
    {

        // Chỉ hiện MainMenu lần đầu tiên khởi động game
        if (UIManager.Instance.Check)
        {
            UIManager.Instance.ShowUI(UIName.MainMenu);
            Debug.Log("Hiện MainMenu lần đầu");
        }
        else
        {
            // Restart từ Pause → Hiện GameplayUI
            UIManager.Instance.ShowUI(UIName.GameplayUI);
            StartCoroutine(DelayStartGame());
            UIManager.Instance.Check = true;
        }
    }
    

    private IEnumerator DelayStartGame()
    {
        yield return null;
        EventManager.Instance.GameStarted();
        
    }
}

