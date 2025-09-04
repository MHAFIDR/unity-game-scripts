using UnityEngine;

[CreateAssetMenu(fileName = "New Timed Bomb", menuName = "Inventory/Timed Bomb")]
public class TimedBombItem : ItemData
{
    public GameObject bombPrefab; // Prefab bom fisik yang akan diletakkan

    public override void Use()
    {
        PlayerInteraction.instance.UseBombItem(this);
    }
}