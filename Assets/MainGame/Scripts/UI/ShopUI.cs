using UnityEngine;
using UnityEngine.UI;

public class ShopUI : MonoBehaviour
{
    [SerializeField] private Button nextButton;
    [SerializeField] private Button prevButton;

    private SkinManager skinManager;

    private void Start()
    {
        skinManager = FindObjectOfType<SkinManager>();
        nextButton.onClick.AddListener(skinManager.NextSkin);
        prevButton.onClick.AddListener(skinManager.PrevSkin);
    }
    public void CloseShop()
    {
        UIManager.Instance.ShowUI(UIName.MainMenu);
        UIManager.Instance.HideUI(UIName.Shop);
    }
}
