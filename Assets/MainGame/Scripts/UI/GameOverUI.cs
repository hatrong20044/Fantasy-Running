
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverUI : MonoBehaviour
{
    public static event System.Action OnPlayPressed;
    [SerializeField] private Image gameOverBox;
    [SerializeField] private Button closeBtn;
    [SerializeField] private TMPro.TMP_Text score;
    [SerializeField] private TMPro.TMP_Text coins;

    private void Awake()
    {
    
        if(gameOverBox)
            gameOverBox.gameObject.SetActive(true);
        this.closeBtn.onClick.AddListener(HandleCloseButton);
    }

    private void Start()
    {
        this.DisplayResult();
    }

    public void HandleCloseButton()
    {
        OnPlayPressed?.Invoke();
        GameData.Instance.ToTalCoins += CoinManager.Instance.Coins;
        // ✅ Load lại scene hiện tại
        UIManager.Instance.HideAllUI();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        
    }

    public void DisplayResult()
    {
        this.score.text = "Score: " + Mathf.FloorToInt(PlayerProgress.Instance.DistanceTravelled).ToString();
        this.coins.text = "Coins: " + CoinManager.Instance.Coins.ToString();
    }
}
