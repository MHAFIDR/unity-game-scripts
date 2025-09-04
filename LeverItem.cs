using UnityEngine;

[CreateAssetMenu(fileName = "New Lever", menuName = "Inventory/Lever")]
public class LeverItem : ItemData
{
    public override void Use()
    {
        PlayerController.instance.UseLeverFromInventory(this);
    }
}