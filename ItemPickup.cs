using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    public ItemData itemData; 
    
    public void PickupItem()
    {
        
        bool berhasilDitambahkan = InventoryManager.instance.playerInventory.Add(itemData);

        if (berhasilDitambahkan)
        {
            Debug.Log(itemData.itemName + " berhasil dipungut.");
           
            InventoryManager.instance.UpdateUI();
           
            Destroy(gameObject);
        }
        else
        {
            Debug.Log("Inventaris penuh! " + itemData.itemName + " tidak bisa dipungut.");
        }
    }
}