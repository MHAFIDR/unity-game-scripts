using UnityEngine;

public class ChargingStation : MonoBehaviour
{
    public ItemData unchargedBattery;
    public ItemData chargedBattery;
    public AudioClip chargeSound;

    // Fungsi ini akan dipanggil oleh PlayerInteraction
    public void ChargeBattery()
    {
        // Cek apakah pemain punya baterai kosong
        if (InventoryManager.instance.playerInventory.items.Contains(unchargedBattery))
        {
            // Hapus baterai kosong
            InventoryManager.instance.playerInventory.Remove(unchargedBattery);
            // Tambahkan baterai penuh
            InventoryManager.instance.playerInventory.Add(chargedBattery);
            // Perbarui UI
            InventoryManager.instance.UpdateUI();

            if (chargeSound != null)
            {
                AudioSource.PlayClipAtPoint(chargeSound, transform.position);
            }
            Debug.Log("Baterai berhasil diisi!");
        }
        else
        {
            Debug.Log("Tidak ada baterai kosong di inventaris.");
        }
    }
}