
using HighScoreSaving;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverUI : MonoBehaviour
{

    [SerializeField] private Image gameOverBox;
    [SerializeField] private Button closeBtn;
    [SerializeField] private TMPro.TMP_Text score;
    [SerializeField] private TMPro.TMP_Text coins;
    [SerializeField] private HighScoreUI table;

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

        GameData.Instance.ToTalCoins += CoinManager.Instance.Coins;
        // ✅ Load lại scene hiện tại
        UIManager.Instance.HideAllUI();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        PauseManager.Instance.ResumeAll();
        
    }

    public void OpneGameOverUI()
    {
        UIManager.Instance.ShowUI(UIName.GameOver);
    }

    public void DisplayResult()
    {
        int scoreCur = Mathf.FloorToInt(PlayerProgress.Instance.DistanceTravelled);
        this.score.text = "Score: " + scoreCur.ToString();
        this.coins.text = "Coins: " + CoinManager.Instance.Coins.ToString();
        this.table.UpdateTable(scoreCur);
        this.table.DisplayTable();
    }
}
