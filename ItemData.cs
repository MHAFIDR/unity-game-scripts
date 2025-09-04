using UnityEngine;

// Menambahkan atribut ini agar kita bisa membuat asset dari class ini di menu Unity
[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
public class ItemData : ScriptableObject
{
    // Informasi dasar item
    public string itemName = "New Item";
    public string description = "Item Description";
    public Sprite icon = null;

    // Fungsi virtual yang bisa di-override oleh item spesifik (misal: Potion)
    public virtual void Use()
    {
        // Logika dasar saat item digunakan
        Debug.Log("Using " + itemName);
    }
}