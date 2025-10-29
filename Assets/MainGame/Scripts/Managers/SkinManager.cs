using UnityEngine;

public class SkinManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Player player;    
    [SerializeField] private Transform skinHolder;
    [SerializeField] private ShopDatabase shopDatabase;

    private int currentIndex = 0;
    private GameObject currentSkinInstance;

    private void Start()
    {

        ShowSkin(currentIndex);
    }

    private void ShowSkin(int index)
    {
        if (currentSkinInstance != null)
            Destroy(currentSkinInstance);

        var data = shopDatabase.items[index];
        if (data == null || data.prefab == null)
        {
            return;
        }

        
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
    }

    public void PrevSkin()
    {
        currentIndex = (currentIndex - 1 + shopDatabase.items.Length) % shopDatabase.items.Length;
        ShowSkin(currentIndex);
    }
}
