using UnityEngine;

[CreateAssetMenu(fileName = "ShopDatabase", menuName = "Shop/Database")]
public class ShopDatabase : ScriptableObject
{
    public ShopItemData[] items;
}
