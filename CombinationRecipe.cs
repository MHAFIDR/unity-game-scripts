using System.Collections.Generic; // Tambahkan ini untuk menggunakan List
using UnityEngine;

[CreateAssetMenu(fileName = "New Recipe", menuName = "Inventory/Recipe")]
public class CombinationRecipe : ScriptableObject
{
    // Mengganti dua bahan menjadi sebuah list yang fleksibel
    public List<ItemData> requiredIngredients; 
    public ItemData resultItem;
}