using UnityEngine.UI;
using UnityEngine;

public class GameplayUI : MonoBehaviour
{
    public static event System.Action OnPlayPressed;
    [Header("Boss Warning")]
    [SerializeField] private Image warningImage; 

    private void Awake()
    {

        if (warningImage)
            warningImage.gameObject.SetActive(false);
    }

    
    public void ShowWarning()
    {
        if (warningImage != null)
            warningImage.gameObject.SetActive(true);
    }

    public void HideWarning()
    {
        if (warningImage != null)
            warningImage.gameObject.SetActive(false);
    }
    public void OpenShop()
    {
        UIManager.Instance.ShowUI(UIName.Shop);
        UIManager.Instance.HideUI(UIName.GameplayUI);
    }
    public void PauseButton()
    {
        PauseManager.Instance.PauseAll();
    }
    public void OnClickResumeButton()
    {
        PauseManager.Instance.ResumeAll();
    }
}
