using UnityEngine;

[CreateAssetMenu(fileName = "ShopItemData", menuName = "Shop/ItemData")]
public class ShopItemData : ScriptableObject
{
    public string itemName;
    public int price;
    public GameObject prefab;  
}
