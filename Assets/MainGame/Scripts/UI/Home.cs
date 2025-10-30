using UnityEngine.UI;
using UnityEngine;

public class Home : MonoBehaviour
{
    public static event System.Action OnPlayPressed;
    [SerializeField] private Button playButton;

    private void Awake()
    {
        if (playButton)
            playButton.onClick.AddListener(HandlePlayButton); 
    }

    private void HandlePlayButton()
    {
        SoundManager.Instance.PlaySFX(SoundType.ButtonClick);
        OnPlayPressed?.Invoke();
        UIManager.Instance.ShowUI(UIName.GameplayUI);
        UIManager.Instance.HideUI(UIName.MainMenu);
    }

    
    public void OpenShop()
    {
        UIManager.Instance.ShowUI(UIName.Shop);
        UIManager.Instance.HideUI(UIName.MainMenu);
    }
}
