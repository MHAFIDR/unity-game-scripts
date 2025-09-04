using System.Collections.Generic;
using UnityEngine;

public class Inventory
{
    public List<ItemData> items = new List<ItemData>();
    private int inventorySize;

    public Inventory(int size)
    {
        inventorySize = size;
    }

    public bool Add(ItemData item)
    {
        if (items.Count >= inventorySize)
        {
            Debug.Log("Inventory is full.");
            return false;
        }
        items.Add(item);
        return true;
    }

    public void Remove(ItemData item)
    {
        items.Remove(item);
    }
}