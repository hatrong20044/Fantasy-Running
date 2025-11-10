
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseUI : MonoBehaviour
{
    [SerializeField] private Image pauseBox;
    [SerializeField] private Image notifiBox;

    [SerializeField] private Button yesRestart;

    public void Start()
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
        
        UIManager.Instance.HideUI(UIName.Pause);
        EventManager.Instance.LockGameplayInput();

    }
    public void OnClickRestartButton()
    {
        pauseBox.gameObject.SetActive(false);
        notifiBox.gameObject.SetActive(true);  
    }

    public void ResumeGame()
    {
        PauseManager.Instance.ResumeAll();
    }

    public void RestartGame()
    {
        this.ClosePauseUI();
        PauseManager.Instance.ResumeAll();
        UIManager.Instance.HideAllUI();
        UIManager.Instance.Check = false;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void OnResumeButtonClick()
    {
        this.ResumeGame();
        this.ClosePauseUI();
    }
   
}
