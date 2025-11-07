using UnityEngine;

public class SkinManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Player player;
    [SerializeField] private Transform skinHolder;
    [SerializeField] private ShopDatabase shopDatabase;
    private string selectedSkinName;

    private int currentIndex = 0;
    private GameObject currentSkinInstance;

    public ShopItemData CurrentItem => shopDatabase.items[currentIndex];

    private void Start()
    {
        LoadShopData();
        if (!shopDatabase.items[0].isPurchased)
            shopDatabase.items[0].isPurchased = true;

        selectedSkinName = PlayerPrefs.GetString("SelectedSkin", shopDatabase.items[0].itemName);
        currentIndex = System.Array.FindIndex(shopDatabase.items, x => x.itemName == selectedSkinName);
        if (currentIndex < 0) currentIndex = 0;

        ShowSkin(currentIndex);
    }



    private void ShowSkin(int index)
    {
        if (currentSkinInstance != null)
            Destroy(currentSkinInstance);

        var data = shopDatabase.items[index];
        if (data == null || data.prefab == null)
            return;

        currentSkinInstance = Instantiate(data.prefab, skinHolder);
        currentSkinInstance.transform.localPosition = Vector3.zero;
        currentSkinInstance.transform.localRotation = Quaternion.identity;

        Animator skinAnim = currentSkinInstance.GetComponentInChildren<Animator>(true);
        if (player != null && skinAnim != null)
        {
            player.Anim = skinAnim;
        }
    }

    public void NextSkin()
    {
        currentIndex = (currentIndex + 1) % shopDatabase.items.Length;
        ShowSkin(currentIndex);
        ShopUI.Instance.UpdateSkinUI();
    }

    public void PrevSkin()
    {
        currentIndex = (currentIndex - 1 + shopDatabase.items.Length) % shopDatabase.items.Length;
        ShowSkin(currentIndex);
        ShopUI.Instance.UpdateSkinUI();
    }

    public void BuyCurrentSkin()
    {
        var item = CurrentItem;
        if (item.isPurchased)
            return;

        if (GameData.Instance.ToTalCoins >= item.price)
        {
            GameData.Instance.ToTalCoins -= item.price;
            item.isPurchased = true;

            SaveShopData(); 

            ShopUI.Instance.UpdateSkinUI();
        }
        else
        {
            ShopUI.Instance.ShowNotEnoughCoin();
        }
    }


    public void UseCurrentSkin()
    {
        var item = CurrentItem;
        if (!item.isPurchased) return;

        selectedSkinName = item.itemName;
        PlayerPrefs.SetString("SelectedSkin", selectedSkinName);
        PlayerPrefs.Save();


        ShopUI.Instance.UpdateSkinUI(); 
    }


    public void SaveShopData()
    {
        for (int i = 0; i < shopDatabase.items.Length; i++)
        {
            var item = shopDatabase.items[i];
            if (i == 0) continue; 

            PlayerPrefs.SetInt(item.itemName, item.isPurchased ? 1 : 0);
        }

        PlayerPrefs.Save();
    }


    public void LoadShopData()
    {
        for (int i = 0; i < shopDatabase.items.Length; i++)
        {
            var item = shopDatabase.items[i];
            if (i == 0)
            {
                item.isPurchased = true;
                continue;
            }

            item.isPurchased = PlayerPrefs.GetInt(item.itemName, 0) == 1;
        }
    }
    public void ShowCurrentSkinAfterClose()
    {
        // Lấy skin hiện đang được "Use"
        string selectedSkinName = PlayerPrefs.GetString("SelectedSkin", shopDatabase.items[0].itemName);

        int index = System.Array.FindIndex(shopDatabase.items, x => x.itemName == selectedSkinName);
        if (index < 0) index = 0;

        currentIndex = index;
        ShowSkin(currentIndex);
    }


}
