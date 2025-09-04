using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{
    public Image icon;
    public Image border; // Tambahan: untuk highlight saat dipilih

    private ItemData item;

    public void AddItem(ItemData newItem)
    {
        item = newItem;
        icon.sprite = item.icon;
        icon.enabled = true;
    }

    public void ClearSlot()
    {
        item = null;
        icon.sprite = null;
        icon.enabled = false;
         border.enabled = false;
    }

    public ItemData GetItem()
    {
        return item;
    }
}