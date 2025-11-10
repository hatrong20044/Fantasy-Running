using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopUI : Singleton<ShopUI>
{

    [Header("UI References")]
    [SerializeField] private Button nextButton;
    [SerializeField] private Button prevButton;
    [SerializeField] private Button buyButton;
    [SerializeField] private Button useButton;
    [SerializeField] private TextMeshProUGUI txtUse;
    [SerializeField] private TextMeshProUGUI txtCoinWarning;
    [SerializeField] private TextMeshProUGUI txtTotalCoin;
    [SerializeField] private TextMeshProUGUI txtBuyPrice;


    private SkinManager skinManager;

    private void Start()
    {
        skinManager = FindObjectOfType<SkinManager>();

        nextButton.onClick.AddListener(skinManager.NextSkin);
        prevButton.onClick.AddListener(skinManager.PrevSkin);
        buyButton.onClick.AddListener(skinManager.BuyCurrentSkin);
        useButton.onClick.AddListener(skinManager.UseCurrentSkin);

        UpdateSkinUI();
    }

    public void UpdateSkinUI()
    {
        var item = skinManager.CurrentItem;
        var currentUsedSkin = PlayerPrefs.GetString(
            "SelectedSkin",
            skinManager.CurrentItem.itemName
        );

        txtTotalCoin.text = $"{GameData.Instance.ToTalCoins}";

    
        buyButton.gameObject.SetActive(!item.isPurchased);
        useButton.gameObject.SetActive(item.isPurchased);

        if (item.itemName == currentUsedSkin)
        {
            txtUse.text = "Using";
            txtUse.color = new Color(0.5f, 0.9f, 0.35f);

        }
        else
        {
            txtUse.text = "Use";
            txtUse.color = new Color(1f, 1f, 0.4f);
        }

        if (txtBuyPrice != null)
        {
            if (!item.isPurchased)
                txtBuyPrice.text = $"Buy - {item.price}";
            else
                txtBuyPrice.text = "Bought";
        }
    }





    public void ShowNotEnoughCoin()
    {
        StopAllCoroutines();
        StartCoroutine(FadeCoinWarning());
    }

    private IEnumerator FadeCoinWarning()
    {
        txtCoinWarning.text = "Not Enough Coin !";
        txtCoinWarning.alpha = 1;

        yield return new WaitForSeconds(1.5f);

        float t = 0;
        while (t < 1)
        {
            t += Time.deltaTime / 1f;
            txtCoinWarning.alpha = Mathf.Lerp(1, 0, t);
            yield return null;
        }
    }

    public void CloseShop()
    {
        if (skinManager != null)
        {
            skinManager.LoadShopData();
            skinManager.ShowCurrentSkinAfterClose(); 
        }

        UIManager.Instance.ShowUI(UIName.MainMenu);
        UIManager.Instance.HideUI(UIName.Shop);
    }

}
