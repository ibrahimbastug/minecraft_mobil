using UnityEngine;

[CreateAssetMenu(fileName = "NewItem", menuName = "Game/Inventory Item")]
public class InventoryItem : ScriptableObject
{
    public string itemName = "New Item";
    public Sprite icon; // Eþyayý göstermek için bir simge
    public int maxStackSize = 64; // Bir yýðýnda taþýnabilecek maksimum miktar
}
