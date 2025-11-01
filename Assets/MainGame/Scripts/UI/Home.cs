using UnityEngine.UI;
using UnityEngine;

public class Home : MonoBehaviour
{
    public static event System.Action OnPlayPressed;
    [SerializeField] private Button playButton;

    [Header("Total Coins")]
    [SerializeField] private Image boxTmp;
    [SerializeField] private Image totalCoinsBox;
    [SerializeField] private TMPro.TMP_Text totalCoins;


    private float baseRight;
    private void Awake()
    {
        if (playButton)
            playButton.onClick.AddListener(HandlePlayButton);
        if (totalCoinsBox)
            totalCoinsBox.gameObject.SetActive(true);

        ///////////////////////////////

        baseRight = boxTmp.GetComponent<RectTransform>().offsetMax.x;
    }
    private void Start()
    {
        this.UpdateTotalCoins();
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

    public void UpdateTotalCoins()
    {
        int coinTmp = GameData.Instance.ToTalCoins;
        this.totalCoins.text = coinTmp.ToString();

        int digits = CountDigits(coinTmp);
        RectTransform rect = boxTmp.GetComponent<RectTransform>();
        float newRight = baseRight + digits * 30f;

        // Giới hạn để không quá dài
        newRight = Mathf.Clamp(newRight, baseRight, baseRight + 185f);

        rect.offsetMax = new Vector2(newRight, rect.offsetMax.y);
    }

    public int CountDigits(int n)
    {
        int count = 0;
        while (n >= 10)
        {
            n /= 10;
            count++;
        }
        return count;
    }

}
