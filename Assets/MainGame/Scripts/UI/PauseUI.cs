
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseUI : MonoBehaviour
{


    [SerializeField] private Image pauseBox;
    [SerializeField] private Image notifiBox;

    [SerializeField] private Button yesRestart;

    private void Awake()
    {
        pauseBox.gameObject.SetActive(true);
        notifiBox.gameObject.SetActive(false);
        yesRestart.onClick.AddListener(RestartGame);
    }
    public void OpenPauseUI()
    {
        UIManager.Instance.ShowUI(UIName.Pause);
    }

    public void ClosePauseUI()
    {
        pauseBox.gameObject.SetActive(false);
        UIManager.Instance.ShowUI(UIName.GameOver);
    }

    public void OnClickRestartButton()
    {
        pauseBox.gameObject.SetActive(false);
        notifiBox.gameObject.SetActive(true);  
    }
    public void HideNotifyBox()
    {
        notifiBox.gameObject.SetActive(false);
    }

    public void RestartGame()
    {
        this.HideNotifyBox();
        PauseManager.Instance.ResumeAll();
        UIManager.Instance.HideAllUI();
        UIManager.Instance.Check = false;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
   
}
