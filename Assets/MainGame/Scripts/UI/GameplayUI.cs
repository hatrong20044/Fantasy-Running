using UnityEngine.UI;
using UnityEngine;

public class GameplayUI : MonoBehaviour
{
    [Header("Boss Warning")]
    [SerializeField] private Image warningImage;
    
    [Header("Coins and Score")]
    [SerializeField] private Image coinsAndScoreBox;
    [SerializeField] private Image coinBox;
    [SerializeField] private TMPro.TMP_Text score;
    [SerializeField] private TMPro.TMP_Text coins;

    private float baseRight;
    private void Awake()
    {

        if (warningImage)
            warningImage.gameObject.SetActive(false);

        if(coinsAndScoreBox)
            coinsAndScoreBox.gameObject.SetActive(true);

        ///////////////////////////////

        baseRight = coinBox.GetComponent<RectTransform>().offsetMax.x;
    }

    private void Update()
    {
       
        this.UpdateGamePlayCoins();
        this.UpdateScore();
    }

    public void UpdateScore()
    {
        this.score.text = Mathf.FloorToInt(PlayerProgress.Instance.DistanceTravelled).ToString("D6");
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
    public void UpdateGamePlayCoins()
    {
        int coinTmp = CoinManager.Instance.Coins;
        coins.text = coinTmp.ToString();
        
        int digits = CountDigits(coinTmp);
        RectTransform rect = coinBox.GetComponent<RectTransform>();
        float newRight = baseRight + digits * 30f;

        // Giới hạn để không quá dài
        newRight = Mathf.Clamp(newRight, baseRight, baseRight + 155f);

        rect.offsetMax = new Vector2(newRight,rect.offsetMax.y);
    }

    public int CountDigits(int n)
    {
        int count = 0;
        while(n >= 10)
        {
            n /= 10;
            count++;
        }
        return count;
    }
    
    public void PauseButton()
    {
        PauseManager.Instance.PauseAll();
        UIManager.Instance.ShowUI(UIName.Pause);
    }
    public void OnClickResumeButton()
    {
        PauseManager.Instance.ResumeAll();
        EventManager.Instance.LockGameplayInput(); 
    }

}
