using UnityEngine.UI;
using UnityEngine;

public class GameplayUI : MonoBehaviour
{
    public static event System.Action OnPlayPressed;
    [SerializeField] private Button playButton;
    [Header("Boss Warning")]
    [SerializeField] private Image warningImage; 

    private void Awake()
    {
        if (playButton)
            playButton.onClick.AddListener(HandlePlayButton);

        if (warningImage)
            warningImage.gameObject.SetActive(false);
    }

    private void HandlePlayButton()
    {
        OnPlayPressed?.Invoke();
        playButton.gameObject.SetActive(false);
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
}
